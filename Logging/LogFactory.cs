using System;
using System.Configuration;
using System.Linq;

namespace Log.It
{
    public static class LogFactory
    {
        private static readonly ILogFactory Factory;

        private const string LoggingSection = "Logging";

        static LogFactory()
        {
            var loggingSection = ConfigurationManager.GetSection(LoggingSection) as LoggingSection;
            if (loggingSection == null)
            {
                throw new ConfigurationErrorsException(
                    $"Could not find {LoggingSection} configuration in configuration file.");
            }

            if (loggingSection.Factory.GetInterfaces().Any(type => type == typeof(ILoggerFactory)) == false)
            {
                throw new ConfigurationErrorsException(
                    $"{loggingSection.Factory.AssemblyQualifiedName} must implement {typeof(ILoggerFactory).FullName}.");
            }

            Factory = ((ILoggerFactory)Activator.CreateInstance(loggingSection.Factory)).Create();
        }

        public static ILogger Create<T>()
        {
            return Factory.Create<T>();
        }

        public static ILogger Create(string logger)
        {
            return Factory.Create(logger);
        }
    }
}