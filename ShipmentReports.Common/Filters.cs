using System.Configuration;

namespace ShipmentReports.Common
{
    public class FiltersSection : ConfigurationSection
    {
        [ConfigurationProperty("filtersSettings", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(FiltersSettingsCollection), AddItemName = "filter")]
        public FiltersSettingsCollection FiltersSettings
        {
            get { return (FiltersSettingsCollection)base["filtersSettings"]; }
        }
    }

    public class FiltersSettingsCollection : ConfigurationElementCollection
    {
        public FiltersSettingsCollection()
        {
        }

        public FiltersSettingsElement this[int index]
        {
            get { return (FiltersSettingsElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(FiltersSettingsElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FiltersSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FiltersSettingsElement)element).Pattern;
        }
    }

    /// <summary>
    /// Contains the settings for the filtering logic
    /// </summary>
    public class FiltersSettingsElement : ConfigurationElement
    {
        /// <summary>
        /// Pattern
        /// </summary>
        [ConfigurationProperty("pattern")]
        public string Pattern
        {
            get { return (string)base["pattern"]; }
            set { base["pattern"] = value; }
        }

        /// <summary>
        /// Suffix for the output file
        /// </summary>
        [ConfigurationProperty("suffix")]
        public string Suffix
        {
            get { return (string)base["suffix"]; }
            set { base["suffix"] = value; }
        }
    }
}
