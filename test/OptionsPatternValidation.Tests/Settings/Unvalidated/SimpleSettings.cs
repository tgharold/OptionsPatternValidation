namespace OptionsPatternValidation.Tests.Settings.Unvalidated
{
    /// <summary>This class has zero validation of anything.</summary>
    [SettingsSectionName("Simple")]
    public class SimpleSettings
    {
        public int? IntegerA { get; set; }
        public bool? BooleanB { get; set; }        
    }
}