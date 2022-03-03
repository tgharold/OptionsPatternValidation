using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OptionsPatternValidation.ValidationOptions;

namespace OptionsPatternValidation.Extensions
{
    /// <summary>Extension methods for OptionsBuilder&lt;TOptions&gt;.</summary>
    public static class OptionsBuilderDataAnnotationsExtensions
    {
        /// <summary>First layer of the bridge between IServiceCollectionExtensions
        /// and the recursive DataAnnotations validator.</summary>
        public static OptionsBuilder<TOptions> RecursivelyValidateDataAnnotations<TOptions>(
            this OptionsBuilder<TOptions> optionsBuilder
            ) where TOptions : class
        {
            optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
                new RecursiveDataAnnotationValidateOptions<TOptions>(
                    optionsBuilder.Name
                ));
            return optionsBuilder;
        }
    }
}