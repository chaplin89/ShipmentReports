using System.Configuration;

namespace ShipmentReports.Maker.PDF
{
    public class PDFMakerSection : ConfigurationSection
    {
        [ConfigurationProperty("finalReportSettings", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(FinalReportSettingsCollection), AddItemName = "field")]
        public FinalReportSettingsCollection FinalReportSettings
        {
            get
            {
                return (FinalReportSettingsCollection)base["finalReportSettings"];
            }
        }
    }


    public class FinalReportSettingsCollection : ConfigurationElementCollection
    {
        public FinalReportSettingsCollection()
        {
        }

        public FinalReportSettingsElement this[int index]
        {
            get { return (FinalReportSettingsElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(FinalReportSettingsElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FinalReportSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FinalReportSettingsElement)element).SourceFieldName;
        }
    }

    /// <summary>
    /// Contains the settings for the generation of the output
    /// </summary>
    public class FinalReportSettingsElement : ConfigurationElement
    {
        /// <summary>
        /// Name of the column into the input file
        /// </summary>
        [ConfigurationProperty("sourceFieldName")]
        public string SourceFieldName
        {
            get
            {
                return (string)base["sourceFieldName"];
            }
            set
            {
                base["sourceFieldName"] = value;
            }
        }

        /// <summary>
        /// Name of the column into the output file
        /// </summary>
        [ConfigurationProperty("destinationFieldName")]
        public string DestinationFieldName
        {
            get
            {
                return (string)base["destinationFieldName"];
            }
            set
            {
                base["destinationFieldName"] = value;
            }
        }

        /// <summary>
        /// Used for establish an order into the output file
        /// </summary>
        [ConfigurationProperty("order")]
        public int Order
        {
            get
            {
                return (int)base["orderInOutput"];
            }
            set
            {
                base["orderInOutput"] = value;
            }
        }
    }
}
