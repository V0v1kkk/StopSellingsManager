using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces.Presenters;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.Models.EventArg;
using MugenMvvmToolkit.ViewModels;
using NLog;
using StopSellingMessageGenerator.Enums;
using StopSellingMessageGenerator.Interfaces;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.ViewModels
{
    internal class MainViewModel : ViewModelBase, ICloseableViewModel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDataSource _dataSource;
        private readonly IToastPresenter _toastPresenter;
        private readonly IMessageTextGenerator _messageTextGenerator;
        private readonly IWorkFolderOwnerChecker _workFolderOwnerChecker;

        private bool _viewBusy;
        private StopSelling _currentStopSelling;
        private ObservableCollection<StopSelling> _stopSellings;

        private bool _newStopSellingMessageOption;
        private bool _endStopSellingMessageOption;

        #region Properties

        public bool ViewBusy
        {
            get { return _viewBusy; }
            set
            {
                if (value == _viewBusy) return;
                _viewBusy = value;
                OnPropertyChanged();
            }
        }

        public StopSelling CurrentStopSelling
        {
            get { return _currentStopSelling; }
            set
            {
                if (Equals(value, _currentStopSelling)) return;
                _currentStopSelling = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StopSelling> StopSellings
        {
            get { return _stopSellings; }
            set
            {
                if (Equals(value, _stopSellings)) return;
                _stopSellings = value;
                OnPropertyChanged();
            }
        }

        #region ReasonControl

        private string _selectedReason;
        private ObservableCollection<string> _reasons;

        public ObservableCollection<string> Reasons
        {
            get { return _reasons; }
            private set
            {
                if (Equals(value, _reasons)) return;
                _reasons = value;
                OnPropertyChanged();
            }
        }

        public string SelectedReason
        {
            get { return _selectedReason; }
            set
            {
                _selectedReason = value;
                OnPropertyChanged();
            }
        }

        public string NewReason
        {
            set
            {
                if (SelectedReason != null)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(value))
                {
                    Reasons.Add(value);
                    SelectedReason = value;
                }
            }
        }

        #endregion

        #region ResponsibilityControl

        private string _selectedResponsibility;
        private ObservableCollection<string> _responsibilities;

        public ObservableCollection<string> Responsibilities
        {
            get { return _responsibilities; }
            private set
            {
                if (Equals(value, _responsibilities)) return;
                _responsibilities = value;
                OnPropertyChanged();
            }
        }

        public string SelectedResponsibility
        {
            get { return _selectedResponsibility; }
            set
            {
                _selectedResponsibility = value;
                OnPropertyChanged();
            }
        }

        public string NewResponsibility
        {
            set
            {
                if (SelectedResponsibility != null)
                {
                    return;
                }
                if (!string.IsNullOrEmpty(value))
                {
                    Responsibilities.Add(value);
                    SelectedResponsibility = value;
                }
            }
        }

        #endregion

        #region RadioButtons

        public bool NewStopSellingMessageOption
        {
            get { return _newStopSellingMessageOption; }
            set
            {
                if (value == _newStopSellingMessageOption) return;
                _newStopSellingMessageOption = value;
                OnPropertyChanged();
            }
        }

        public bool EndStopSellingMessageOption
        {
            get { return _endStopSellingMessageOption; }
            set
            {
                if (value == _endStopSellingMessageOption) return;
                _endStopSellingMessageOption = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        public MainViewModel(IDataSource dataSource, IToastPresenter toastPresenter, 
                            IMessageTextGenerator messageTextGenerator, IWorkFolderOwnerChecker workFolderOwnerChecker)
        {
            _dataSource = dataSource;
            _toastPresenter = toastPresenter;
            _messageTextGenerator = messageTextGenerator;
            _workFolderOwnerChecker = workFolderOwnerChecker;

            FirstRunCheck();

            WorkFolderExistCheck();

            ViewBusy = true;
            Initialize();

            CreateMessageTextCommand = new RelayCommand(CreateMessageTextExecute, CreateMessageTextCanExecute, this);

            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindowCommandExecute);
            OpenAboutWindowCommand = new RelayCommand(OpenAboutWindowCommandExecute);

            var readFromDiskTimer = new Timer(60000);
            readFromDiskTimer.Elapsed += ReadDataFromDiskOnTimer;
            readFromDiskTimer.Enabled = true;
        }


        #region Commands

        public ICommand CreateMessageTextCommand;

        public ICommand OpenSettingsWindowCommand;
        public ICommand OpenAboutWindowCommand;

        #region CanExecute
        private bool CreateMessageTextCanExecute(object arg)
        {
            return (NewStopSellingMessageOption || EndStopSellingMessageOption) && (CurrentStopSelling != null);
        }

        #endregion

        #region Execute

        private void CreateMessageTextExecute(object o)
        {
            if (CurrentStopSelling == null) return;

            string messageText;

            if (NewStopSellingMessageOption)
            {
                CurrentStopSelling.StopStopSellingTime = DateTime.Now; //set current time
                messageText = _messageTextGenerator.GenerateText(CurrentStopSelling,
                    MessageTypeEnum.StartStopSellingMessage);
            }
            else if (EndStopSellingMessageOption)
            {
                messageText = _messageTextGenerator.GenerateText(CurrentStopSelling,
                    MessageTypeEnum.EndStopSellingMessage);
            }
            else
            {
                return;
            }

            Clipboard.SetText(messageText);
            _toastPresenter.ShowAsync("Сообщение скопировано в буфер обмена", ToastDuration.Short, ToastPosition.Center);
        }

        private async void OpenAboutWindowCommandExecute(object obj)
        {
            await GetViewModel<AboutControlViewModel>().ShowAsync();
        }

        private async void OpenSettingsWindowCommandExecute(object obj)
        {
            using (var settingsViewModel = GetViewModel<SettingsViewModel>())
            {
                settingsViewModel.WorkFolderPath =
                    Properties.Settings.Default.PathToWorkFolder;
                await settingsViewModel.ShowAsync();
                if (settingsViewModel.WorkFolderPath != "" && settingsViewModel.WorkFolderPath != Properties.Settings.Default.PathToWorkFolder)
                {
                    Properties.Settings.Default.PathToWorkFolder = settingsViewModel.WorkFolderPath;
                    Properties.Settings.Default.Save();
                    MessageBox.Show(
                            "Изменения сохранены. Приложение будет закрыто. Просьба перезапустить программу." +
                            "Не забудте убедится, что все необходимые файлы присутствуют в новой директории.",
                            "Изменения сохранены", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown(); // close application
                }
            }
        }

        #endregion

        #endregion

        private void FirstRunCheck()
        {
            if(Properties.Settings.Default.firstRun)
            {
                using (var settingsViewModel = GetViewModel<SettingsViewModel>())
                {
                    settingsViewModel.WorkFolderPath = "";
                    settingsViewModel.ShowAsync();
                    if (settingsViewModel.WorkFolderPath != "")
                    {
                        Properties.Settings.Default.PathToWorkFolder = settingsViewModel.WorkFolderPath;
                        Properties.Settings.Default.firstRun = false;
                        Properties.Settings.Default.Save();
                        MessageBox.Show(
                            "Изменения сохранены. Приложение будет закрыто. Просьба перезапустить программу." +
                            "Не забудте убедится, что все необходимые файлы присутствуют в новой директории.",
                            "Изменения сохранены", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.Shutdown(); // close application
                    }
                }
            }
        }


        private void WorkFolderExistCheck()
        {
            if(_workFolderOwnerChecker.WorkFolderExist()) return;

            var interactResult = MessageBox.Show("Рабочая папка не существует. Хотите выбрать другую папку?", 
                "Ошибка открытия рабочей папки", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (interactResult == MessageBoxResult.Yes)
            {
                using (var settingsViewModel = GetViewModel<SettingsViewModel>())
                {
                    settingsViewModel.WorkFolderPath = Properties.Settings.Default.PathToWorkFolder;
                    settingsViewModel.ShowAsync();
                    if (settingsViewModel.WorkFolderPath != Properties.Settings.Default.PathToWorkFolder)
                    {
                        Properties.Settings.Default.PathToWorkFolder = settingsViewModel.WorkFolderPath;
                        Properties.Settings.Default.Save();
                        MessageBox.Show(
                            "Изменения сохранены. Приложение будет закрыто. Просьба перезвпустить программу." +
                            "Не забудте убедится, что все необходимые файлы присутствуют в новой директории.",
                            "Изменения сохранены", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.Shutdown(); // close application
                    }
                }
            }
            else
            {
                MessageBox.Show("Убедитесь в наличии доступа к рабочей папке. " +
                                "Приложение будет закрыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown(); // close application
            }
        }

        private void ReadDataFromDiskOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            WorkFolderExistCheck();
            ViewBusy = true;
            Initialize();
        }



        private async void Initialize()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var tempStopSellings = new ObservableCollection<StopSelling>(_dataSource.LoadStopSellings()); //load serialized stopSellings
                    if (StopSellings == null) StopSellings = tempStopSellings;
                    else if (!StopSellings.SequenceEqual(tempStopSellings))
                    {
                        // ReSharper disable once InconsistentNaming
                        var selectedTTName = CurrentStopSelling.TTName;
                        CurrentStopSelling = null;
                        StopSellings = tempStopSellings;
                        CurrentStopSelling = StopSellings.FirstOrDefault(x => x.TTName == selectedTTName);
                    }

                    bool dictionarysChanged = false;
                    var tempRasons = new ObservableCollection<string>(_dataSource.LoadReasonsOfStopSellings()); //load reason list
                    if (Reasons == null) Reasons = tempRasons;
                    else if (!Reasons.SequenceEqual(tempRasons))
                    {
                        Reasons = tempRasons;
                        dictionarysChanged = true;
                    }

                    var tempResponsibilities = new ObservableCollection<string>(_dataSource.LoadResponsibleDepartments()); //load responsibility list
                    if (Responsibilities == null) Responsibilities = tempResponsibilities;
                    else if (!Responsibilities.SequenceEqual(tempResponsibilities))
                    {
                        Reasons = tempResponsibilities;
                        dictionarysChanged = true;
                    }

                    if (dictionarysChanged) //refresh selected item
                    {
                        var tempSelectedItem = CurrentStopSelling;
                        CurrentStopSelling = null;
                        CurrentStopSelling = tempSelectedItem; 
                    }

                    foreach (var stopSelling in StopSellings) // refresh duration of stop sellings
                    {
                        stopSelling.StopStopSellingTime = DateTime.Now;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Fatal($"Loading data error: {exception}");
                    MessageBox.Show("Ошибка загрузки справочника . Проверьте наличие и корректность файлов. " +
                                    "Приложение будет закрыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown(); // close application
                }
            });
            ViewBusy = false;
        }


        /// <summary>
        /// This method run on MainViewModel closing
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Task<bool> CloseAsync(object parameter = null)
        {
            return Empty.TrueTask;
        }
        public ICommand CloseCommand { get; set; }
        public event EventHandler<ICloseableViewModel, ViewModelClosingEventArgs> Closing;
        public event EventHandler<ICloseableViewModel, ViewModelClosedEventArgs> Closed;

    }
}