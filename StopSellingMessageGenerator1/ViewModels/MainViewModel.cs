using System;
using System.Collections.Generic;
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
        private readonly IStopSellingsBulder _stopSellingsBulder;
        private readonly IToastPresenter _toastPresenter;
        private readonly IMessageTextGenerator _messageTextGenerator;
        private readonly IReportGenerator _reportGenerator;
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

        public MainViewModel(IDataSource dataSource, IStopSellingsBulder stopSellingsBulder, IToastPresenter toastPresenter,
                                                IMessageTextGenerator messageTextGenerator, IReportGenerator reportGenerator, 
                                                IWorkFolderOwnerChecker workFolderOwnerChecker)
        {
            if (stopSellingsBulder == null || dataSource == null)
            {
                Application.Current.Shutdown(); // close application
            }

            _stopSellingsBulder = stopSellingsBulder;
            _dataSource = dataSource;
            _toastPresenter = toastPresenter;
            _messageTextGenerator = messageTextGenerator;
            _reportGenerator = reportGenerator;
            _workFolderOwnerChecker = workFolderOwnerChecker;

            FirstRunCheck();

            WorkFolderExistCheck();

            WorkFolderOwnerCheck();

            ViewBusy = true;
            Initialize();

            CreateNewStopSellingCommand = new RelayCommand(CreateNewStopSellingExecute);
            CloseAndExportStopSellingCommand = new RelayCommand(CloseAndExportStopSellingExecute);
            CreateMessageTextCommand = new RelayCommand(CreateMessageTextExecute, CreateMessageTextCanExecute, this);

            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindowCommandExecute);
            OpenAboutWindowCommand = new RelayCommand(OpenAboutWindowCommandExecute);
            CheckTtInformationCommand = new RelayCommand(CheckTtInformationCommandExecute, CheckTtInformationCommandCanExecute, this);

            var writeToDiskTimer = new Timer(180000);
            writeToDiskTimer.Elapsed += WriteDataToDiskOnTimer;
            writeToDiskTimer.Enabled = true;
        }


        #region Commands

        public ICommand CreateNewStopSellingCommand;
        public ICommand CloseAndExportStopSellingCommand;
        public ICommand CreateMessageTextCommand;

        public ICommand OpenSettingsWindowCommand;
        public ICommand OpenAboutWindowCommand;

        public ICommand CheckTtInformationCommand;

        #region CanExecute
        private bool CreateMessageTextCanExecute(object arg)
        {
            return (NewStopSellingMessageOption || EndStopSellingMessageOption) && (CurrentStopSelling != null);
        }

        private bool CheckTtInformationCommandCanExecute(object o)
        {
            return true;
        }

        #endregion

        #region Execute
        private void CreateNewStopSellingExecute()
        {
            var newStopSelling = _stopSellingsBulder.Build();

            if (newStopSelling == null)
            {
                MessageBox.Show("Ошибка при создании стоп-продажи. Проверьте корректность файлов.", "Ошибка при создании стоп-продажи", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            StopSellings.Add(newStopSelling);
            CurrentStopSelling = newStopSelling;
        }

        private void CloseAndExportStopSellingExecute()
        {
            if (CurrentStopSelling == null)
            {
                MessageBox.Show("Выберите стоп-продажу для экспорта", "Не выбрана стоп-продажа", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (_reportGenerator.ExportToReport(CurrentStopSelling))
            {
                StopSellings.Remove(CurrentStopSelling);
                CurrentStopSelling = null;
                _toastPresenter.ShowAsync("Стоп-продажа перенесена в отчёт", ToastDuration.Short, ToastPosition.Center);
            }
            else
            {
                _toastPresenter.ShowAsync("Ошибка при экспорте стоп-продажи в отчёт", ToastDuration.Long, ToastPosition.Center);
            }
        }

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
                            "Изменения сохранены. Приложение будет закрыто. Просьба перезвпустить программу." +
                            "Не забудте убедится, что все необходимые файлы присутствуют в новой директории.",
                            "Изменения сохранены", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown(); // close application
                }
            }
        }

        private void CheckTtInformationCommandExecute(object obj)
        {
            CurrentStopSelling?.CheckTTnumber();
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
                            "Не забудте убедиться, что все необходимые файлы присутствуют в новой директории.",
                            "Изменения сохранены", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.Shutdown(); // close application
                    }
                }
            }
        }

        /// <summary>
        /// Check owner of current work folder.
        /// If folder occupy, offer choice rewrite owner or close application.
        /// </summary>
        private void WorkFolderOwnerCheck()
        {
            if(_workFolderOwnerChecker.MeIsOwner()) return;

            var interactResult = MessageBox.Show("Папка занята другим пользователем:" + Environment.NewLine + _workFolderOwnerChecker.GetOwnerData() + 
                                    Environment.NewLine + "Сбросить сессию пользователя?" ,"Ошибка открытия рабочей папки", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(interactResult == MessageBoxResult.Yes)
            {
                _workFolderOwnerChecker.MakeMeOwner();
                if(!_workFolderOwnerChecker.MeIsOwner()) 
                {
                    MessageBox.Show("Не удалось занять рабочую папку. Приложение будет закрыто.", "Ошибка захвата рабочей папки",MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown(); // close application
                }
            }
            else
            {
                MessageBox.Show("Приложение будет закрыто.","Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown(); // close application
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
                            "Изменения сохранены. Приложение будет закрыто. Просьба перезапустить программу." +
                            "Не забудте убедиться, что все необходимые файлы присутствуют в новой директории.",
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

        private void WriteDataToDiskOnTimer(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            WorkFolderOwnerCheck();
            _workFolderOwnerChecker.MakeMeOwner(); //rewrite owner for update last write time

            List<string> readonsDistinct = (from reason in Reasons select reason).Distinct().ToList();
            List<string> responsibilitiesDistinct = (from responsibility in Responsibilities select responsibility).Distinct().ToList();
            _dataSource.SaveReasonsOfStopSellings(readonsDistinct);
            _dataSource.SaveResponsibleDepartments(responsibilitiesDistinct);
            _dataSource.SaveStopSellings(StopSellings.ToList());
        }



        private async void Initialize()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    //todo: add exeptions to _datasource implementation
                    StopSellings = new ObservableCollection<StopSelling>(_dataSource.LoadStopSellings()); //load serialized stopSellings
                    Reasons = new ObservableCollection<string>(_dataSource.LoadReasonsOfStopSellings()); //load reason list
                    Responsibilities = new ObservableCollection<string>(_dataSource.LoadResponsibleDepartments()); //load responsibility list
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
            return Task<bool>.Factory.StartNew(() =>
            {
                if(_workFolderOwnerChecker.MeIsOwner())
                {
                    List<string> readonsDistinct = (from reason in Reasons select reason).Distinct().ToList();
                    List<string> responsibilitiesDistinct = (from responsibility in Responsibilities select responsibility).Distinct().ToList();
                    _dataSource.SaveReasonsOfStopSellings(readonsDistinct);
                    _dataSource.SaveResponsibleDepartments(responsibilitiesDistinct);
                    _dataSource.SaveStopSellings(StopSellings.ToList());
                    _workFolderOwnerChecker.ClearOwnership();
                }
                else
                {
                    MessageBox.Show("Рабочая папка занята другим пользователем. Данные не сохранены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return true;
            });
        }
        public ICommand CloseCommand { get; set; }
        public event EventHandler<ICloseableViewModel, ViewModelClosingEventArgs> Closing;
        public event EventHandler<ICloseableViewModel, ViewModelClosedEventArgs> Closed;

    }
}