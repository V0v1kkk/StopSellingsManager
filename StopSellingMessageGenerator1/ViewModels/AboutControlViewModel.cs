using System;
using System.Reflection;
using MugenMvvmToolkit.ViewModels;

namespace StopSellingMessageGenerator.ViewModels
{
    /// <summary>
    /// Copyright (c) 2012 Christoph Gattnar
    /// </summary>
    class AboutControlViewModel : ViewModelBase
    {
        #region Fields

        private string _title;
        private string _description;
        private string _version;
        private string _copyright;
        private string _additionalNotes;
        private string _hyperlinkText;
        private Uri _hyperlink;
        private string _publisher;
        private bool _isSemanticVersioning;

        #endregion

        #region Constructors

        public AboutControlViewModel()
        {

            Assembly assembly = Assembly.GetEntryAssembly();
            Version = assembly.GetName().Version.ToString();
            Title = "Просмотрщик стоп-продаж";

            #if NET35 || NET40
			AssemblyCopyrightAttribute copyright = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
			AssemblyDescriptionAttribute description = Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
			AssemblyCompanyAttribute company = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;
            #else
            AssemblyCopyrightAttribute copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            AssemblyDescriptionAttribute description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
            AssemblyCompanyAttribute company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            #endif
            Copyright = copyright.Copyright;
            Description = description.Description;
            Publisher = company.Company;

            AdditionalNotes = "В программе использованы компоненты:" + 
                Environment.NewLine +
                "WPF About Box (http://aboutbox.codeplex.com/) - MIT лицензия" +
                Environment.NewLine +
                "MugenMvvmToolkit (https://github.com/MugenMvvmToolkit/MugenMvvmToolkit) - MS-PL лицензия" +
                Environment.NewLine +
                "FileHelpers (http://www.filehelpers.net/) - MIT лицензия" +
                Environment.NewLine +
                "NLog (http://nlog-project.org/) - BSD лицензия" +
                Environment.NewLine +
                "Extended WPF Toolkit (http://wpftoolkit.codeplex.com/) - MS-PL лицензия"+
                Environment.NewLine +
                "Manager, task icon (https://www.iconfinder.com/icons/81729/manager_task_icon) - Creative Commons (Attribution 3.0 Unported)" +
                Environment.NewLine +
                "Info icon (https://www.iconfinder.com/icons/172483/info_icon) - Creative Commons Attribution-No Derivative Works 3.0 Unported"+
                Environment.NewLine +
                "Settings icon (https://www.iconfinder.com/icons/172549/settings_icon) - Creative Commons Attribution-No Derivative Works 3.0 Unported"+
                Environment.NewLine +
                "Ookii.Dialogs (http://www.ookii.org/software/dialogs/) - Copyright © Sven Groot (Ookii.org) 2009";
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the application title.
        /// </summary>
        /// <value>The application title.</value>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the application info.
        /// </summary>
        /// <value>The application info.</value>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets if Semantic Versioning is used.
        /// </summary>
        /// <see cref="http://semver.org/"/>
        /// <value>The bool that indicats whether Semantic Versioning is used.</value>
        public bool IsSemanticVersioning
        {
            get
            {
                return _isSemanticVersioning;
            }
            set
            {
                _isSemanticVersioning = value;
                OnPropertyChanged("Version");
            }
        }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The application version.</value>
        public string Version
        {
            get
            {
                if (IsSemanticVersioning)
                {
                    var tmp = _version.Split('.');
                    var version = string.Format("{0}.{1}.{2}", tmp[0], tmp[1], tmp[2]);
                    return version;
                }

                return _version;
            }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the publisher.
        /// </summary>
        /// <value>The publisher.</value>
        public string Publisher
        {
            get
            {
                return _publisher;
            }
            set
            {
                if (_publisher != value)
                {
                    _publisher = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the copyright label.
        /// </summary>
        /// <value>The copyright label.</value>
        public string Copyright
        {
            get
            {
                return _copyright;
            }
            set
            {
                if (_copyright != value)
                {
                    _copyright = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the hyperlink text.
        /// </summary>
        /// <value>The hyperlink text.</value>
        public string HyperlinkText
        {
            get
            {
                return _hyperlinkText;
            }
            set
            {
                try
                {
                    Hyperlink = new Uri(value);
                    _hyperlinkText = value;
                    OnPropertyChanged();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public Uri Hyperlink
        {
            get
            {
                return _hyperlink;
            }
            set
            {
                if (_hyperlink != value)
                {
                    _hyperlink = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the further info.
        /// </summary>
        /// <value>The further info.</value>
        public string AdditionalNotes
        {
            get
            {
                return _additionalNotes;
            }
            set
            {
                if (_additionalNotes != value)
                {
                    _additionalNotes = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

    }
}
