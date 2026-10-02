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
            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Next.Name'", failure);
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

        #region Value equality

        // A C# record, or any class that overrides Equals and GetHashCode,
        // compares by value. These classes do the same by hand. Records need
        // an IsExternalInit shim on net481, so the tests avoid them.

        public class ValueItem : IEquatable<ValueItem>
        {
            [Required]
            public string Value { get; set; }

            public bool Equals(ValueItem other) => other != null && Value == other.Value;

            public override bool Equals(object obj) => Equals(obj as ValueItem);

            public override int GetHashCode() => Value?.GetHashCode() ?? 0;
        }

        public class ValueItemSettings
        {
            public ValueItem First { get; set; }

            public ValueItem Second { get; set; }

            public List<ValueItem> Items { get; set; }
        }

        [Fact(Skip = "Known gap in RecursiveDataAnnotationsValidation 2.1.1: visited objects are tracked by value equality. Passes on 2.3.3.")]
        public void Equal_but_distinct_objects_are_each_validated()
        {
            var settings = new ValueItemSettings
            {
                First = new ValueItem { Value = null },
                Second = new ValueItem { Value = null }
            };

            var result = Validate(settings);

            Assert.False(result.Succeeded);
            Assert.Collection(
                result.Failures,
                f => Assert.Contains("'First.Value'", f),
                f => Assert.Contains("'Second.Value'", f));
        }

        [Fact(Skip = "Known gap in RecursiveDataAnnotationsValidation 2.1.1: visited objects are tracked by value equality. Passes on 2.3.3.")]
        public void Equal_but_distinct_list_items_are_each_validated()
        {
            var settings = new ValueItemSettings
            {
                Items = new List<ValueItem>
                {
                    new ValueItem { Value = null },
                    new ValueItem { Value = null }
                }
            };

            var result = Validate(settings);

            Assert.False(result.Succeeded);
            Assert.Collection(
                result.Failures,
                f => Assert.Contains("'Items[0].Value'", f),
                f => Assert.Contains("'Items[1].Value'", f));
        }

        /// <summary>Hashes and compares its whole graph, the way a record does.</summary>
        public class ValueNode : IEquatable<ValueNode>
        {
            [Required]
            public string Name { get; set; }

            public ValueNode Next { get; set; }

            public bool Equals(ValueNode other) =>
                other != null && Name == other.Name && Equals(Next, other.Next);

            public override bool Equals(object obj) => Equals(obj as ValueNode);

            public override int GetHashCode() =>
                unchecked(((Name?.GetHashCode() ?? 0) * 31) + (Next?.GetHashCode() ?? 0));
        }

        /// <summary>With a validator that tracks visited objects by value, this
        /// test overflows the stack in GetHashCode and kills the test host.
        /// RecursiveDataAnnotationsValidation 2.1.1 does that. The fix is to
        /// track visited objects by reference.</summary>
        [Fact(Skip = "Known gap in RecursiveDataAnnotationsValidation 2.1.1: overflows the stack in GetHashCode and aborts the test run. Passes on 2.3.3.")]
        public void Self_referencing_value_equal_object_terminates()
        {
            var a = new ValueNode { Name = "a" };
            a.Next = a;

            var result = Validate(a);

            Assert.True(result.Succeeded);
        }

        #endregion

        #region Size

        // The validator recurses once per nesting level. On the 512 KB default
        // thread stack on macOS a chain of roughly 900 nodes overflows. Other
        // platforms have different default stack sizes, so the limit differs.
        // The deep tests run on a thread with an explicit large stack. That
        // keeps them about the validator and not about the host's stack size.
        private const int LargeStackBytes = 64 * 1024 * 1024;
        private static readonly TimeSpan LargeStackTimeout = TimeSpan.FromMinutes(1);

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
            }, LargeStackBytes) { IsBackground = true };
            thread.Start();
            Assert.True(thread.Join(LargeStackTimeout), "Validation did not finish in time.");
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
            const int depth = 10_000;
            var root = BuildChain(depth);
            var bottom = root;
            while (bottom.Next != null) bottom = bottom.Next;
            bottom.Name = null;

            var result = RunOnLargeStack(() => Validate(root));

            Assert.False(result.Succeeded);
            var failure = Assert.Single(result.Failures);
            var expectedPath = string.Concat(Enumerable.Repeat("Next.", depth - 1)) + "Name";
            Assert.Contains($"'{expectedPath}'", failure);
        }

        [Fact]
        public void Deeply_nested_graph_reports_every_invalid_level()
        {
            // Every level is invalid, so every error path is rebuilt at each
            // level above it. In 2.1.1 the cost grows with the cube of the
            // depth. This checks the result. Timing is not asserted, because
            // wall-clock limits are flaky on shared CI runners.
            const int depth = 1_000;
            var root = BuildChain(depth);
            for (var node = root; node != null; node = node.Next) node.Name = null;

            var result = RunOnLargeStack(() => Validate(root));

            Assert.False(result.Succeeded);
            Assert.Equal(depth, result.Failures.Count());
            var deepestPath = string.Concat(Enumerable.Repeat("Next.", depth - 1)) + "Name";
            Assert.Contains(result.Failures, f => f.Contains($"'{deepestPath}'"));
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
            Assert.Collection(
                result.Failures,
                f => Assert.Contains("'Items[0].Value'", f),
                f => Assert.Contains($"'Items[{count - 1}].Value'", f));
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
