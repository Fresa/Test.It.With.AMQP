using Microsoft.VisualStudio.TextTemplating;

namespace Test.It.With.RabbitMQ.Extensions
{
    public static class TextTransformationExtensions
    {
        public static void WriteNewLine(this TextTransformation textTransformation)
        {
            textTransformation.WriteLine(string.Empty);
        }
    }
}