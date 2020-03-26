using System.IO;

namespace OptionsPatternValidation.Tests.Helpers
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string str)
        {
            // https://stackoverflow.com/a/5434325
            
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}