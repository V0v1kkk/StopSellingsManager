using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
                                                IMessageTextGenerator messageTextGenerator, IReportGenerator reportGenerator)
        {
            if (stopSellingsBulder == null || dataSource == null)
            {
                //todo: Error and exit
            }

            _stopSellingsBulder = stopSellingsBulder;
            _dataSource = dataSource;
            _toastPresenter = toastPresenter;
            _messageTextGenerator = messageTextGenerator;
            _reportGenerator = reportGenerator;

            FirstRunCheck();


            ViewBusy = true;
            Initialize();

            CreateNewStopSellingCommand = new RelayCommand(CreateNewStopSellingExecute);
			CloseAndExportStopSellingCommand = new RelayCommand(CloseAndExportStopSellingExecute);
            CreateMessageTextCommand = new RelayCommand(CreateMessageTextExecute, CreateMessageTextCanExecute, this);

            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindowCommandExecute);
            OpenAboutWindowCommand = new RelayCommand(OpenAboutWindowCommandExecute);
            CheckTtInformationCommand = new RelayCommand(CheckTtInformationCommandExecute, CheckTtInformationCommandCanExecute, this);
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
            //return CurrentStopSelling != null && !string.IsNullOrWhiteSpace(CurrentStopSelling.TTNumber);
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
            CurrentStopSelling.StopStopSellingTime = DateTime.Now; //set current time
            string messageText;
            if (NewStopSellingMessageOption)
            {
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
                        "Изменения сохранены. Для применения изменений перезапустите программу. Не забудте убедится, что все необходимые файлы присутствуют в новой директории.",
                        "Изменения сохранены", MessageBoxButton.OK, MessageBoxImage.Information);
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
            //throw new NotImplementedException();
        }


        private async void Initialize()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    StopSellings = new ObservableCollection<StopSelling>(_dataSource.LoadStopSellings()); //load serialized stopSellings
                    Reasons = new ObservableCollection<string>(_dataSource.LoadReasonsOfStopSellings()); //load reason list
                    Responsibilities = new ObservableCollection<string>(_dataSource.LoadResponsibleDepartments()); //load responsibility list
                }
                catch (Exception exception)
                {
                    Logger.Fatal($"Loading data error: {exception}");
                    MessageBox.Show("Ошибка загрузки справочника . Проверьте наличие и корректность файлов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            });
            ViewBusy = false;
        }




        #region Commands



        

        #endregion


        /// <summary>
        /// This method run on MainViewModel closing
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Task<bool> CloseAsync(object parameter = null)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                List<string> readonsDistinct = (from reason in Reasons select reason).Distinct().ToList();
                List<string> responsibilitiesDistinct = (from responsibility in Responsibilities select responsibility).Distinct().ToList();
                _dataSource.SaveReasonsOfStopSellings(readonsDistinct);
                _dataSource.SaveResponsibleDepartments(responsibilitiesDistinct);
                _dataSource.SaveStopSellings(StopSellings.ToList());	
                return true;
            });
        }

        public ICommand CloseCommand { get; set; }
        public event EventHandler<ICloseableViewModel, ViewModelClosingEventArgs> Closing;
        public event EventHandler<ICloseableViewModel, ViewModelClosedEventArgs> Closed;


    }
}
