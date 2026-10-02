using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.Tests.Helpers;
using Xunit;

#if NETFRAMEWORK
namespace System.Runtime.CompilerServices
{
    // Lets the init accessors and records below compile on .NET Framework.
    internal static class IsExternalInit
    {
    }
}
#endif

namespace OptionsPatternValidation.Tests.ServiceCollectionExtensions
{
    /// <summary>Settings shapes seen in applications that wire options up the same
    /// way (nested sections, arrays of derived items, optional sections,
    /// cross-field rules, init-only members and records). Each test binds JSON
    /// through AddValidatedSettings and reads the value, so it covers the
    /// binder and the validator together. These pin what 1.4 callers see today.
    /// Where the behavior is a gap and not a feature, the test says so.</summary>
    public class AddValidatedSettingsConfigShapeTests
    {
        /// <summary>Binds the JSON under a section named after the type, then
        /// reads the options value. Pass only the body of the section.</summary>
        private static T Bind<T>(string sectionBody) where T : class
        {
            var configuration = ConfigurationTestBuilder.BuildFromJsonString(
                "{ \"" + typeof(T).Name + "\": " + sectionBody + " }");

            var services = new ServiceCollection();
            services.AddValidatedSettings<T>(configuration);

            return services.BuildServiceProvider().GetRequiredService<IOptions<T>>().Value;
        }

        /// <summary>Binds with no section at all, so the options hold only their defaults.</summary>
        private static T BindMissingSection<T>() where T : class
        {
            var configuration = ConfigurationTestBuilder.BuildFromJsonString("{ }");

            var services = new ServiceCollection();
            services.AddValidatedSettings<T>(configuration);

            return services.BuildServiceProvider().GetRequiredService<IOptions<T>>().Value;
        }

        private static IReadOnlyList<string> Failures<T>(string sectionBody) where T : class =>
            Assert.Throws<OptionsValidationException>(() => Bind<T>(sectionBody)).Failures.ToList();

        #region Nested sections with defaults

        public class RateLimitPolicy
        {
            [Range(1, 1000)]
            public int PermitLimit { get; set; } = 100;

            [Range(1, 60)]
            public int WindowMinutes { get; set; } = 1;
        }

        public class RateLimitingSettings
        {
            public RateLimitPolicy Global { get; set; } = new RateLimitPolicy();

            public RateLimitPolicy Auth { get; set; } = new RateLimitPolicy();
        }

        [Fact]
        public void Nested_sections_that_are_not_in_the_config_keep_their_valid_defaults()
        {
            var result = Bind<RateLimitingSettings>("{ \"Global\": { \"PermitLimit\": 5 } }");

            Assert.Equal(5, result.Global.PermitLimit);
            Assert.Equal(100, result.Auth.PermitLimit);
        }

        [Fact]
        public void Invalid_value_in_one_nested_section_is_reported_with_that_sections_path()
        {
            var failures = Failures<RateLimitingSettings>("{ \"Auth\": { \"PermitLimit\": 0 } }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Auth.PermitLimit'", failure);
        }

        #endregion

        #region Arrays of derived items

        public enum DeviceType
        {
            Inverter,
            Meter
        }

        public class ModbusDevice
        {
            [Required]
            public string Host { get; set; }

            public ushort Port { get; set; } = 1502;
        }

        public class SolarDevice : ModbusDevice
        {
            [Required]
            [EnumDataType(typeof(DeviceType))]
            public DeviceType Type { get; set; }
        }

        public class MeterDevice : ModbusDevice
        {
            [Range(0, 3)]
            public byte Index { get; set; }
        }

        public class DevicesSettings
        {
            public SolarDevice[] Solar { get; set; } = Array.Empty<SolarDevice>();

            public MeterDevice[] Meters { get; set; } = Array.Empty<MeterDevice>();

            // A read-only view over the same objects. The validator reaches
            // them a second time by this path.
            public IEnumerable<ModbusDevice> All => Solar.Cast<ModbusDevice>().Concat(Meters);
        }

        [Fact]
        public void Valid_arrays_of_derived_items_bind_and_pass()
        {
            var result = Bind<DevicesSettings>(@"{
                ""Solar"": [ { ""Type"": ""Inverter"", ""Host"": ""10.0.0.1"" } ],
                ""Meters"": [ { ""Host"": ""10.0.0.2"", ""Index"": 1 } ]
            }");

            Assert.Equal(DeviceType.Inverter, result.Solar[0].Type);
            Assert.Equal(1, result.Meters[0].Index);
            Assert.Equal(2, result.All.Count());
        }

        [Fact]
        public void Inherited_required_member_is_reported_with_the_array_index()
        {
            var failures = Failures<DevicesSettings>(@"{
                ""Solar"": [
                    { ""Type"": ""Inverter"", ""Host"": ""10.0.0.1"" },
                    { ""Type"": ""Meter"" }
                ]
            }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Solar[1].Host'", failure);
        }

        [Fact]
        public void Items_reached_by_a_read_only_view_are_reported_once_under_the_first_path()
        {
            var failures = Failures<DevicesSettings>("{ \"Meters\": [ { \"Index\": 9 } ] }");

            Assert.Equal(2, failures.Count);
            Assert.Contains(failures, f => f.Contains("'Meters[0].Host'"));
            Assert.Contains(failures, f => f.Contains("'Meters[0].Index'"));
            Assert.DoesNotContain(failures, f => f.Contains("'All"));
        }

        [Fact]
        public void Numeric_enum_value_outside_the_enum_fails_EnumDataType()
        {
            var failures = Failures<DevicesSettings>(
                "{ \"Solar\": [ { \"Type\": \"99\", \"Host\": \"h\" } ] }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Solar[0].Type'", failure);
        }

        public class SingleDeviceSettings
        {
            public SolarDevice Device { get; set; }
        }

        [Fact]
        public void Enum_name_that_does_not_exist_on_a_plain_property_throws_from_the_binder()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => Bind<SingleDeviceSettings>(
                "{ \"Device\": { \"Type\": \"Battery\", \"Host\": \"h\" } }"));

            Assert.IsNotType<OptionsValidationException>(exception);
        }

        /// <summary>Gap in the configuration binder, not in this library (seen with
        /// Microsoft.Extensions.Configuration.Binder 8.0 to 10.0). An array element
        /// with an enum name that does not exist is dropped without an error, so the
        /// validator never sees it and the options pass. The same name on a plain
        /// property throws. If a binder upgrade starts to throw here, update this
        /// test.</summary>
        [Fact]
        public void Array_element_with_an_unknown_enum_name_is_dropped_and_the_options_pass()
        {
            var result = Bind<DevicesSettings>(
                "{ \"Solar\": [ { \"Type\": \"Battery\", \"Host\": \"h\" } ] }");

            Assert.Empty(result.Solar);
        }

        public class ExpectedDevicesSettings
        {
            [MinLength(2)]
            public SolarDevice[] Solar { get; set; } = Array.Empty<SolarDevice>();
        }

        /// <summary>The README suggests a length attribute on the collection to
        /// catch elements the binder dropped.</summary>
        [Fact]
        public void MinLength_on_the_array_catches_an_element_the_binder_dropped()
        {
            var failures = Failures<ExpectedDevicesSettings>(@"{
                ""Solar"": [
                    { ""Type"": ""Inverter"", ""Host"": ""10.0.0.1"" },
                    { ""Type"": ""Batery"", ""Host"": ""10.0.0.2"" }
                ]
            }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Solar'", failure);
        }

        [Fact]
        public void MinLength_on_the_array_passes_when_no_element_was_dropped()
        {
            var result = Bind<ExpectedDevicesSettings>(@"{
                ""Solar"": [
                    { ""Type"": ""Inverter"", ""Host"": ""10.0.0.1"" },
                    { ""Type"": ""Meter"", ""Host"": ""10.0.0.2"" }
                ]
            }");

            Assert.Equal(2, result.Solar.Length);
        }

        #endregion

        #region Optional sections

        public class OptionalEndpoint
        {
            public bool Enabled { get; set; } = true;

            [Required]
            [MaxLength(40)]
            public string Endpoint { get; set; } = "https://example.com/mcp/";

            [MaxLength(5)]
            public string Token { get; set; }
        }

        public class OptionalSectionSettings
        {
            public OptionalEndpoint Github { get; set; }

            [Required]
            public OptionalEndpoint Required { get; set; } = new OptionalEndpoint();
        }

        [Fact]
        public void Optional_nested_section_that_is_absent_stays_null_and_passes()
        {
            var result = Bind<OptionalSectionSettings>("{ }");

            Assert.Null(result.Github);
        }

        [Fact]
        public void Optional_nested_section_that_is_present_is_validated()
        {
            var failures = Failures<OptionalSectionSettings>(
                "{ \"Github\": { \"Endpoint\": \"\", \"Token\": \"too long\" } }");

            Assert.Equal(2, failures.Count);
            Assert.Contains(failures, f => f.Contains("'Github.Endpoint'"));
            Assert.Contains(failures, f => f.Contains("'Github.Token'"));
        }

        [Fact]
        public void Empty_object_for_an_optional_section_leaves_it_null()
        {
            var result = Bind<OptionalSectionSettings>("{ \"Github\": { } }");

            Assert.Null(result.Github);
        }

        [Fact]
        public void Json_null_replaces_a_default_with_null_and_fails_Required()
        {
            var failures = Failures<OptionalSectionSettings>(
                "{ \"Required\": { \"Endpoint\": null } }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Required.Endpoint'", failure);
        }

        [Fact]
        public void Required_nested_section_with_a_default_instance_passes_when_absent()
        {
            var result = BindMissingSection<OptionalSectionSettings>();

            Assert.NotNull(result.Required);
        }

        #endregion

        #region Cross-field rules on the root object

        public enum StorageType
        {
            Memory,
            Sqlite
        }

        public class StorageSettings : IValidatableObject
        {
            [Required]
            public StorageType StorageType { get; set; }

            public string ConnectionString { get; set; }

            [MaxLength(3)]
            public string Name { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (StorageType == StorageType.Sqlite && string.IsNullOrEmpty(ConnectionString))
                {
                    yield return new ValidationResult(
                        $"{nameof(ConnectionString)} is required if {nameof(StorageType)} is Sqlite.",
                        new[] { nameof(ConnectionString) });
                }
            }
        }

        [Fact]
        public void Cross_field_rule_on_the_root_object_fails_the_bound_options()
        {
            var failures = Failures<StorageSettings>("{ \"StorageType\": \"Sqlite\" }");

            var failure = Assert.Single(failures);
            Assert.Contains("'ConnectionString'", failure);
        }

        [Fact]
        public void Cross_field_rule_passes_when_the_other_field_is_set()
        {
            var result = Bind<StorageSettings>(
                "{ \"StorageType\": \"Sqlite\", \"ConnectionString\": \"Data Source=a.db\" }");

            Assert.Equal(StorageType.Sqlite, result.StorageType);
        }

        [Fact]
        public void Required_on_an_enum_never_fails_because_the_default_value_is_not_null()
        {
            var result = BindMissingSection<StorageSettings>();

            Assert.Equal(StorageType.Memory, result.StorageType);
        }

        /// <summary>Same as the DataAnnotations rule: IValidatableObject.Validate
        /// only runs when every attribute on the object has passed. The failure
        /// from the cross-field rule is hidden until the attribute failure is fixed.</summary>
        [Fact]
        public void Cross_field_rule_is_skipped_while_an_attribute_on_the_same_object_fails()
        {
            var failures = Failures<StorageSettings>(
                "{ \"StorageType\": \"Sqlite\", \"Name\": \"too long\" }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Name'", failure);
        }

        #endregion

        #region Lists with defaults

        public class AllowListSettings
        {
            [MinLength(1)]
            public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

            public List<string> Defaults { get; set; } = new List<string> { "a", "b" };
        }

        [Fact]
        public void Missing_section_validates_the_empty_default_array()
        {
            var exception = Assert.Throws<OptionsValidationException>(
                () => BindMissingSection<AllowListSettings>());

            var failure = Assert.Single(exception.Failures);
            Assert.Contains("'AllowedOrigins'", failure);
        }

        [Fact]
        public void Bound_array_is_added_to_a_non_empty_default_list_and_not_swapped_in()
        {
            var result = Bind<AllowListSettings>(
                "{ \"AllowedOrigins\": [ \"https://x\" ], \"Defaults\": [ \"c\" ] }");

            Assert.Equal(new[] { "a", "b", "c" }, result.Defaults);
        }

        #endregion

        #region Init-only members and records

        public class InitSettings
        {
            [Required]
            public string Name { get; init; }

            [Range(1, 10)]
            public int Size { get; init; } = 99;
        }

        [Fact]
        public void Init_only_members_are_bound_and_validated()
        {
            var failures = Failures<InitSettings>("{ \"Size\": 0 }");

            Assert.Equal(2, failures.Count);
            Assert.Contains(failures, f => f.Contains("'Name'"));
            Assert.Contains(failures, f => f.Contains("'Size'"));
        }

        public record Endpoint
        {
            [Required]
            public string Host { get; init; }
        }

        public record EndpointsSettings
        {
            public List<Endpoint> Endpoints { get; init; } = new List<Endpoint>();
        }

        [Fact]
        public void Record_with_init_members_is_bound_and_validated()
        {
            var failures = Failures<EndpointsSettings>("{ \"Endpoints\": [ { \"Port\": 1 } ] }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Endpoints[0].Host'", failure);
        }

        /// <summary>Records compare by value, so two invalid items with the same
        /// content are equal. Records are the usual way to hit the
        /// equal-but-distinct gap, since nobody writes Equals by hand.</summary>
        [Fact(Skip = "Known gap in RecursiveDataAnnotationsValidation 2.1.1: visited objects are tracked by value equality, so the second equal record is skipped. Passes on 2.3.3.")]
        public void Equal_invalid_records_in_a_list_are_each_reported()
        {
            var failures = Failures<EndpointsSettings>(
                "{ \"Endpoints\": [ { \"Port\": 1 }, { \"Port\": 1 } ] }");

            Assert.Equal(2, failures.Count);
            Assert.Contains(failures, f => f.Contains("'Endpoints[0].Host'"));
            Assert.Contains(failures, f => f.Contains("'Endpoints[1].Host'"));
        }

        #endregion

        #region Members that are not properties

        public class FieldSettings
        {
            public const string SectionName = "Fields";

            [Required]
            public string Field = null;

            [Required]
            public string Property { get; set; }
        }

        [Fact]
        public void Constants_and_public_fields_are_not_validated()
        {
            var failures = Failures<FieldSettings>("{ }");

            var failure = Assert.Single(failures);
            Assert.Contains("'Property'", failure);
        }

        #endregion

        #region Attributes seen on real settings

        public class ContactSettings
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Url]
            public string Website { get; set; }

            [Phone]
            public string PhoneNumber { get; set; }

            [MaxLength(10)]
            public string Country { get; set; }
        }

        [Fact]
        public void Optional_attributes_pass_when_the_values_are_absent()
        {
            var result = Bind<ContactSettings>("{ \"Email\": \"a@example.com\" }");

            Assert.Null(result.Website);
            Assert.Null(result.PhoneNumber);
            Assert.Null(result.Country);
        }

        [Theory]
        [InlineData("{ \"Email\": \"not-an-email\" }", "'Email'")]
        [InlineData("{ \"Email\": \"a@example.com\", \"Website\": \"example.com\" }", "'Website'")]
        [InlineData("{ \"Email\": \"a@example.com\", \"PhoneNumber\": \"abc\" }", "'PhoneNumber'")]
        [InlineData("{ \"Email\": \"a@example.com\", \"Country\": \"a very long name\" }", "'Country'")]
        public void Each_attribute_on_a_bound_value_fails_under_its_own_member(string json, string member)
        {
            var failures = Failures<ContactSettings>(json);

            var failure = Assert.Single(failures);
            Assert.Contains(member, failure);
        }

        #endregion
    }
}
