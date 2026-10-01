using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.ValidationOptions;
using Xunit;

namespace OptionsPatternValidation.Tests.ValidationOptions
{
    /// <summary>Locks in how the validator behaves with unusual option
    /// graphs: cycles, very deep or very wide object graphs, messages
    /// that carry values or control characters, and attributes that throw.
    /// These came out of the OWASP Top 10:2025 audit.</summary>
    public class RecursiveDataAnnotationValidateOptionsHardeningTests
    {
        private static ValidateOptionsResult Validate<T>(T options) where T : class
        {
            var validator = new RecursiveDataAnnotationValidateOptions<T>(null);
            return validator.Validate(Options.DefaultName, options);
        }

        #region Cycles

        public class Node
        {
            [Required]
            public string Name { get; set; }

            public Node Next { get; set; }
        }

        [Fact]
        public void Cyclic_graph_terminates_and_passes_when_valid()
        {
            var a = new Node { Name = "a" };
            var b = new Node { Name = "b", Next = a };
            a.Next = b;

            var result = Validate(a);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Cyclic_graph_terminates_and_reports_invalid_member()
        {
            var a = new Node { Name = "a" };
            var b = new Node { Name = null, Next = a };
            a.Next = b;

            var result = Validate(a);

            Assert.False(result.Succeeded);
            Assert.Contains(result.Failures, f => f.Contains("Name"));
        }

        [Fact]
        public void Self_referencing_object_terminates()
        {
            var a = new Node { Name = "a" };
            a.Next = a;

            var result = Validate(a);

            Assert.True(result.Succeeded);
        }

        #endregion

        #region Size

        // The validator recurses once per nesting level. On a default 512 KB
        // worker stack (macOS) a chain of roughly 900 nodes overflows, so the
        // deep tests run on a thread with an explicit large stack. That keeps
        // them about the validator and not about the host's stack size.
        private const int LargeStackBytes = 64 * 1024 * 1024;

        private static Node BuildChain(int depth)
        {
            var root = new Node { Name = "level-0" };
            var current = root;
            for (var i = 1; i < depth; i++)
            {
                current.Next = new Node { Name = $"level-{i}" };
                current = current.Next;
            }

            return root;
        }

        private static T RunOnLargeStack<T>(Func<T> func)
        {
            T value = default;
            Exception failure = null;
            var thread = new Thread(() =>
            {
                try { value = func(); }
                catch (Exception ex) { failure = ex; }
            }, LargeStackBytes);
            thread.Start();
            thread.Join();
            if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
            return value;
        }

        [Fact]
        public void Deeply_nested_graph_is_validated()
        {
            var root = BuildChain(10_000);

            var result = RunOnLargeStack(() => Validate(root));

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Deeply_nested_graph_reports_invalid_member_at_the_bottom()
        {
            var root = BuildChain(10_000);
            var bottom = root;
            while (bottom.Next != null) bottom = bottom.Next;
            bottom.Name = null;

            var result = RunOnLargeStack(() => Validate(root));

            Assert.False(result.Succeeded);
            Assert.Single(result.Failures);
        }

        public class Item
        {
            [Required]
            public string Value { get; set; }
        }

        public class ListSettings
        {
            public List<Item> Items { get; set; }
        }

        [Fact]
        public void Very_large_list_is_validated()
        {
            const int count = 100_000;
            var settings = new ListSettings
            {
                Items = Enumerable.Range(0, count).Select(i => new Item { Value = "x" }).ToList()
            };

            var result = Validate(settings);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Very_large_list_reports_each_invalid_item_once()
        {
            const int count = 100_000;
            var settings = new ListSettings
            {
                Items = Enumerable.Range(0, count).Select(i => new Item { Value = "x" }).ToList()
            };
            settings.Items[0].Value = null;
            settings.Items[count - 1].Value = null;

            var result = Validate(settings);

            Assert.False(result.Succeeded);
            Assert.Equal(2, result.Failures.Count());
        }

        #endregion

        #region Message content

        /// <summary>A custom attribute that puts the rejected value into its
        /// message. The library does not redact messages.</summary>
        private sealed class LeakyAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                return new ValidationResult($"Bad value '{value}'.", new[] { validationContext.MemberName });
            }
        }

        public class LeakySettings
        {
            [Leaky]
            public string Password { get; set; } = "hunter2";
        }

        [Fact]
        public void Attribute_message_is_copied_verbatim_into_failure()
        {
            var result = Validate(new LeakySettings());

            Assert.False(result.Succeeded);
            var failure = Assert.Single(result.Failures);
            Assert.Equal(
                "Validation failed for members: 'Password' with the error: 'Bad value 'hunter2'.'.",
                failure);
        }

        public class MultilineSettings
        {
            [Required(ErrorMessage = "first line\r\nsecond line\nthird line")]
            public string Name { get; set; }
        }

        [Fact]
        public void Newlines_in_error_message_are_kept_in_a_single_failure()
        {
            var result = Validate(new MultilineSettings());

            Assert.False(result.Succeeded);
            var failure = Assert.Single(result.Failures);
            Assert.Contains("first line\r\nsecond line\nthird line", failure);
        }

        #endregion

        #region Throwing attributes

        private sealed class ThrowingAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                throw new InvalidOperationException("attribute blew up");
            }
        }

        public class ThrowingSettings
        {
            [Throwing]
            public string Name { get; set; } = "x";
        }

        /// <summary>Characterization test: exceptions from user attributes
        /// propagate unwrapped. If the library starts wrapping them, update
        /// this test and add a CHANGELOG entry.</summary>
        [Fact]
        public void Exception_from_attribute_propagates_unwrapped()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => Validate(new ThrowingSettings()));

            Assert.Equal("attribute blew up", exception.Message);
        }

        public class RegexTimeoutSettings
        {
            // Catastrophic backtracking pattern with a short timeout so the test stays fast.
            [RegularExpression("^(a+)+$", MatchTimeoutInMilliseconds = 50)]
            public string Name { get; set; } = new string('a', 64) + "!";
        }

        /// <summary>Characterization test: a regex timeout is not converted
        /// into a validation failure.</summary>
        [Fact]
        public void Regex_timeout_propagates_unwrapped()
        {
            Assert.Throws<RegexMatchTimeoutException>(() => Validate(new RegexTimeoutSettings()));
        }

        #endregion
    }
}
