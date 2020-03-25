using System;
using System.Reflection;

namespace OptionsPatternValidation
{
    /// <summary>Use this when the section name in the appsettings.json file does not match
    /// the name of the options class name.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsSectionNameAttribute : Attribute
    {
        public SettingsSectionNameAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
        
        public string SectionName { get; }
        
        
        /// <summary>Get the section name within appsettings.json.  If the SettingsSectionNameAttribute
        /// is present, use that as the section name in the JSON.  Otherwise fall back to the type name.
        /// </summary>
        internal static string GetSettingsSectionName<T>() where T : class
        {
            return typeof(T).GetCustomAttribute<SettingsSectionNameAttribute>()?.SectionName
                ?? typeof(T).Name;
        }        
    }
}