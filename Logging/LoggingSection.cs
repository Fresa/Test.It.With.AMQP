using System;
using System.ComponentModel;
using System.Configuration;

namespace Log.It
{
    public class LoggingSection : ConfigurationSection
    {
        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("Factory", IsRequired = true)]
        public Type Factory
        {
            get => this["Factory"] as Type;
            set => this["Factory"] = value;
        }
    }
}