namespace OptionsPatternValidation.Tests.TestSettings.Unvalidated
{
    [SettingsSectionName("Simple")]
    public class SimpleSettings
    {
        public int? IntegerA { get; set; }
        public bool? BooleanB { get; set; }        
    }
}