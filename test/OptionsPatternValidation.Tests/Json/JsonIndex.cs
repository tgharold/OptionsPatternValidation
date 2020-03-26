namespace OptionsPatternValidation.Tests.Json
{
    public static class JsonIndex
    {
        // NOTE: All files must be "EmbeddedResource" files

        /// <summary>JSON for the SimpleSettings class.  None of these have validation.</summary>
        public static class Unvalidated
        {
            public const string Test1 = "Json.Unvalidated.SimpleSettings1.json";
            public const string Test2 = "Json.Unvalidated.SimpleSettings2.json";
        }

        /// <summary>JSON for the SimpleAttributeValidatedSettings class.</summary>
        public static class AttributeValidated
        {
            /// <summary>This JSON will not pass validation.  Some props are missing / out of range.</summary>
            public const string Test1 = "Json.AttributeValidated.Test1.json";

            /// <summary>JSON that will pass all validation.</summary>
            public const string Test2 = "Json.AttributeValidated.Test2.json";
        }
    }
}