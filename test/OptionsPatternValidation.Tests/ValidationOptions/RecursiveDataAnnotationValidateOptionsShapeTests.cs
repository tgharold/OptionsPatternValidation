using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.ValidationOptions;
using Xunit;

namespace OptionsPatternValidation.Tests.ValidationOptions
{
    /// <summary>Characterization tests for how the validator walks different
    /// object graph shapes: collections, structs, inheritance, framework types
    /// and shared references. Most of the walking is done by the
    /// RecursiveDataAnnotationsValidation package. These tests pin what
    /// callers see today, so that a dependency upgrade cannot change it
    /// without a failing test. Where the behavior is a gap and not a feature,
    /// the test says so. If an upgrade closes a gap, update the test and add a
    /// CHANGELOG entry.</summary>
    public class RecursiveDataAnnotationValidateOptionsShapeTests
    {
        private static ValidateOptionsResult Validate<T>(T options) where T : class
        {
            var validator = new RecursiveDataAnnotationValidateOptions<T>(null);
            return validator.Validate(Options.DefaultName, options);
        }

        public class Item
        {
            [Required]
            public string V { get; set; }
        }

        #region Collections

        public class DictionarySettings
        {
            public Dictionary<string, Item> Map { get; set; } = new Dictionary<string, Item> { ["key"] = new Item() };
        }

        [Fact]
        public void Dictionary_values_are_validated_and_the_path_uses_the_index_and_not_the_key()
        {
            var result = Validate(new DictionarySettings());

            Assert.False(result.Succeeded);
            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Map[0].Value.V'", failure);
        }

        public class ArraySettings
        {
            public Item[] Items { get; set; } = { new Item(), new Item { V = "ok" }, new Item() };
        }

        [Fact]
        public void Array_elements_are_validated()
        {
            var result = Validate(new ArraySettings());

            Assert.Collection(
                result.Failures,
                f => Assert.Contains("'Items[0].V'", f),
                f => Assert.Contains("'Items[2].V'", f));
        }

        public class SetSettings
        {
            public HashSet<Item> Items { get; set; } = new HashSet<Item> { new Item() };
        }

        [Fact]
        public void Set_elements_are_validated()
        {
            var result = Validate(new SetSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Items[0].V'", failure);
        }

        public class LazySettings
        {
            public IEnumerable<Item> Items { get; set; } = Enumerable.Range(0, 2).Select(_ => new Item());
        }

        [Fact]
        public void Lazy_enumerable_elements_are_validated()
        {
            var result = Validate(new LazySettings());

            Assert.Equal(2, result.Failures.Count());
        }

        public class NestedListSettings
        {
            public List<List<Item>> Rows { get; set; } = new List<List<Item>>
            {
                new List<Item> { new Item() }
            };
        }

        /// <summary>Gap: only the first level of a collection is walked. An
        /// invalid item inside a list of lists passes.</summary>
        [Fact]
        public void Items_in_a_list_of_lists_are_not_validated()
        {
            var result = Validate(new NestedListSettings());

            Assert.True(result.Succeeded);
        }

        public class CollectionAttributeSettings
        {
            [MinLength(2)]
            public Item[] Items { get; set; } = { new Item { V = "a" } };
        }

        [Fact]
        public void Attributes_on_the_collection_property_itself_are_checked()
        {
            var result = Validate(new CollectionAttributeSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Items'", failure);
        }

        public class ListAttributeSettings
        {
            [MinLength(2)]
            public List<Item> Items { get; set; } = new List<Item> { new Item { V = "a" } };
        }

#if NETFRAMEWORK
        /// <summary>Gap in .NET Framework: MinLengthAttribute only accepts arrays and
        /// strings, so it throws on a List. Use an array there.</summary>
        [Fact]
        public void MinLength_on_a_list_throws_InvalidCastException()
        {
            Assert.Throws<InvalidCastException>(() => Validate(new ListAttributeSettings()));
        }
#else
        [Fact]
        public void MinLength_on_a_list_is_checked()
        {
            var result = Validate(new ListAttributeSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Items'", failure);
        }
#endif

        #endregion

        #region Structs

        public struct ItemStruct
        {
            [Required]
            public string V { get; set; }
        }

        public class StructPropertySettings
        {
            public ItemStruct Single { get; set; }

            public ItemStruct[] Many { get; set; } = { new ItemStruct() };
        }

        /// <summary>Gap: an invalid struct held directly in a property passes,
        /// but the same struct inside an array is reported.</summary>
        [Fact]
        public void Struct_in_a_property_is_not_validated_but_struct_in_an_array_is()
        {
            var result = Validate(new StructPropertySettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Many[0].V'", failure);
        }

        #endregion

        #region Inheritance and typing

        public class BaseSettings
        {
            [Required]
            public string BaseName { get; set; }
        }

        public class DerivedSettings : BaseSettings
        {
            [Range(1, 5)]
            public int Size { get; set; } = 9;
        }

        public class InheritanceSettings
        {
            public DerivedSettings Child { get; set; } = new DerivedSettings();
        }

        [Fact]
        public void Inherited_and_declared_members_are_both_validated()
        {
            var result = Validate(new InheritanceSettings());

            Assert.Equal(2, result.Failures.Count());
            Assert.Contains(result.Failures, f => f.Contains("'Child.Size'"));
            Assert.Contains(result.Failures, f => f.Contains("'Child.BaseName'"));
        }

        public interface IHasValue
        {
            string V { get; }
        }

        public class ImplementsHasValue : IHasValue
        {
            [Required]
            public string V { get; set; }
        }

        public class DeclaredTypeSettings
        {
            public object AsObject { get; set; } = new Item();

            public IHasValue AsInterface { get; set; } = new ImplementsHasValue();
        }

        [Fact]
        public void Members_are_validated_by_runtime_type_not_declared_type()
        {
            var result = Validate(new DeclaredTypeSettings());

            Assert.Equal(2, result.Failures.Count());
            Assert.Contains(result.Failures, f => f.Contains("'AsObject.V'"));
            Assert.Contains(result.Failures, f => f.Contains("'AsInterface.V'"));
        }

        public class RequiredObjectSettings
        {
            [Required]
            public Item Child { get; set; }
        }

        [Fact]
        public void Required_nested_object_that_is_null_is_reported()
        {
            var result = Validate(new RequiredObjectSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Child'", failure);
        }

        #endregion

        #region Members the validator skips or survives

        public class OddMembers
        {
            [Required]
            public string Name { get; set; }

            // Not checked: attributes on a static property. Not read: private
            // properties. Objects under a public static property are walked.
            [Required]
            public static string StaticValue { get; set; }

            private Item Hidden { get; set; } = new Item();

            // Must not be read: an indexer needs an argument.
            public string this[int index] => "x";

            // Must not break validation: the getter throws.
            public string Boom => throw new InvalidOperationException("getter");

            public bool UsesHidden => Hidden != null;
        }

        public class OddMembersSettings
        {
            public OddMembers Odd { get; set; } = new OddMembers();
        }

        [Fact]
        public void Static_private_indexer_and_throwing_members_do_not_affect_the_result()
        {
            var result = Validate(new OddMembersSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Odd.Name'", failure);
        }

        public class FrameworkTypeSettings
        {
            public Uri Address { get; set; } = new Uri("http://example.com/");

            public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(1);

            public DateTime Created { get; set; } = DateTime.UtcNow;

            public DateTimeOffset CreatedOffset { get; set; } = DateTimeOffset.UtcNow;

            public Version Version { get; set; } = new Version(1, 2);

            public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

            public Encoding Encoding { get; set; } = Encoding.UTF8;

            public Guid Id { get; set; } = Guid.NewGuid();

            public Dictionary<string, string> Labels { get; set; } = new Dictionary<string, string> { ["a"] = "b" };

            [Required]
            public string Name { get; set; }
        }

        [Fact]
        public void Framework_typed_properties_do_not_add_failures_or_throw()
        {
            var result = Validate(new FrameworkTypeSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Name'", failure);
        }

        public class RelativeFrameworkTypeSettings
        {
            public Uri Path { get; set; } = new Uri("/api", UriKind.Relative);

            public Type Kind { get; set; } = typeof(string);

            [Required]
            public string Name { get; set; }
        }

        /// <summary>Reading the properties of a relative Uri or of a Type
        /// throws. RecursiveDataAnnotationsValidation 2.1.1 walked them and
        /// failed with TargetInvocationException. Version 2.3.3 does not walk
        /// these framework types.</summary>
        [Fact]
        public void Relative_uri_and_type_properties_do_not_throw()
        {
            var result = Validate(new RelativeFrameworkTypeSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Name'", failure);
        }

        #endregion

        #region Shared references and multiple errors

        public class SharedReferenceSettings
        {
            public Item First { get; set; }

            public Item Second { get; set; }

            public SharedReferenceSettings()
            {
                First = new Item();
                Second = First;
            }
        }

        /// <summary>One object reachable by two paths is validated once and
        /// reported under the first path it was reached by.</summary>
        [Fact]
        public void Object_reachable_by_two_paths_is_reported_once()
        {
            var result = Validate(new SharedReferenceSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'First.V'", failure);
        }

        public class ValidatableChild : IValidatableObject
        {
            public int Count { get; set; } = -1;

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (Count < 0)
                    yield return new ValidationResult("Count is negative.", new[] { "Count", "Other" });
            }
        }

        public class ValidatableSettings
        {
            public ValidatableChild Child { get; set; } = new ValidatableChild();
        }

        [Fact]
        public void IValidatableObject_on_a_nested_object_reports_all_member_names_with_the_path()
        {
            var result = Validate(new ValidatableSettings());

            var failure = Assert.Single(result.Failures);
            Assert.Contains("'Child.Count,Child.Other'", failure);
        }

        public class MultiErrorSettings
        {
            [Required]
            public string A { get; set; }

            [Required]
            public string B { get; set; }

            [Range(1, 2)]
            public int C { get; set; }
        }

        [Fact]
        public void Each_failing_member_gets_its_own_failure_in_declaration_order()
        {
            var result = Validate(new MultiErrorSettings());

            Assert.Collection(
                result.Failures,
                f => Assert.Contains("'A'", f),
                f => Assert.Contains("'B'", f),
                f => Assert.Contains("'C'", f));
        }

        #endregion
    }
}
