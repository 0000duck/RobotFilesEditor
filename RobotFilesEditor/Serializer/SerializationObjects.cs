using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotFilesEditor.Serializer
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ControlersConfiguration
    {

        private ControlersConfigurationControler []controlerField;

        /// <remarks/>
        public ControlersConfigurationControler []Controler
        {
            get
            {
                return this.controlerField;
            }
            set
            {
                this.controlerField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ControlersConfigurationControler
    {

        private ControlersConfigurationControlerFilesToCopy[] filesToCopyField;

        private ControlersConfigurationControlerDataToCopy []dataToCopyField;

        private ControlersConfigurationControlerFilesToRemove []filesToRemoveField;

        private string controlerTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FilesToCopy")]
        public ControlersConfigurationControlerFilesToCopy[] FilesToCopy
        {
            get
            {
                return this.filesToCopyField;
            }
            set
            {
                this.filesToCopyField = value;
            }
        }

        /// <remarks/>
        public ControlersConfigurationControlerDataToCopy []DataToCopy
        {
            get
            {
                return this.dataToCopyField;
            }
            set
            {
                this.dataToCopyField = value;
            }
        }

        /// <remarks/>
        public ControlersConfigurationControlerFilesToRemove []FilesToRemove
        {
            get
            {
                return this.filesToRemoveField;
            }
            set
            {
                this.filesToRemoveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ControlerType
        {
            get
            {
                return this.controlerTypeField;
            }
            set
            {
                this.controlerTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ControlersConfigurationControlerFilesToCopy
    {

        private ControlersConfigurationControlerFilesToCopyFilesFilter filesFilterField;

        private string destinationFolderField;

        private string programTypeField;

        /// <remarks/>
        public ControlersConfigurationControlerFilesToCopyFilesFilter FilesFilter
        {
            get
            {
                return this.filesFilterField;
            }
            set
            {
                this.filesFilterField = value;
            }
        }

        /// <remarks/>
        public string DestinationFolder
        {
            get
            {
                return this.destinationFolderField;
            }
            set
            {
                this.destinationFolderField = value;
            }
        }

        /// <remarks/>
        public string ProgramType
        {
            get
            {
                return this.programTypeField;
            }
            set
            {
                this.programTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ControlersConfigurationControlerFilesToCopyFilesFilter
    {

        private string[] filesExtensionField;

        private string[] containNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FilesExtension")]
        public string[] FilesExtension
        {
            get
            {
                return this.filesExtensionField;
            }
            set
            {
                this.filesExtensionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ContainName")]
        public string[] ContainName
        {
            get
            {
                return this.containNameField;
            }
            set
            {
                this.containNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ControlersConfigurationControlerDataToCopy
    {

        private ControlersConfigurationControlerDataToCopyFilesFilter filesFilterField;

        private string destinationFolderField;

        private string fileTypeField;

        /// <remarks/>
        public ControlersConfigurationControlerDataToCopyFilesFilter FilesFilter
        {
            get
            {
                return this.filesFilterField;
            }
            set
            {
                this.filesFilterField = value;
            }
        }

        /// <remarks/>
        public string DestinationFolder
        {
            get
            {
                return this.destinationFolderField;
            }
            set
            {
                this.destinationFolderField = value;
            }
        }

        /// <remarks/>
        public string FileType
        {
            get
            {
                return this.fileTypeField;
            }
            set
            {
                this.fileTypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ControlersConfigurationControlerDataToCopyFilesFilter
    {

        private string []filesExtensionField;

        private string[] containNameField;

        /// <remarks/>
        public string []FilesExtension
        {
            get
            {
                return this.filesExtensionField;
            }
            set
            {
                this.filesExtensionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ContainName")]
        public string[] ContainName
        {
            get
            {
                return this.containNameField;
            }
            set
            {
                this.containNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ControlersConfigurationControlerFilesToRemove
    {

        private ControlersConfigurationControlerFilesToRemoveFilesFilter filesFilterField;

        /// <remarks/>
        public ControlersConfigurationControlerFilesToRemoveFilesFilter FilesFilter
        {
            get
            {
                return this.filesFilterField;
            }
            set
            {
                this.filesFilterField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ControlersConfigurationControlerFilesToRemoveFilesFilter
    {

        private string []filesExtensionField;

        /// <remarks/>
        public string []FilesExtension
        {
            get
            {
                return this.filesExtensionField;
            }
            set
            {
                this.filesExtensionField = value;
            }
        }
    }
}

