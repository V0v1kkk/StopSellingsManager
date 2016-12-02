using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.Models.EventArg;
using MugenMvvmToolkit.ViewModels;

namespace StopSellingMessageGenerator.ViewModels
{
    public class SettingsViewModel : ViewModelBase, ICloseableViewModel
    {
        private string _workFolderPath;

        public string WorkFolderPath
        {
            get
            {
                return _workFolderPath;
            }

            set
            {
                if (value == _workFolderPath) return;
                _workFolderPath = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangeWorkDirectoryFolderCommand;
        public ICommand OkCommand;
        public ICommand CancelCommand;

        public SettingsViewModel()
        {
            ChangeWorkDirectoryFolderCommand = new RelayCommand(ChangeWorkDirectoryFolderCommandExecute);

            OkCommand = new RelayCommand(delegate(object o)
            {
                Closed?.Invoke(this,new ViewModelClosedEventArgs(this,null));
            });

            CancelCommand = new RelayCommand(o => 
            {
                WorkFolderPath ="";
                Closed?.Invoke(this, new ViewModelClosedEventArgs(this, null));
            });
        }

        private void ChangeWorkDirectoryFolderCommandExecute(object obj)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true,
                Description = @"Выберите рабочую директорию",
                SelectedPath = Directory.Exists(WorkFolderPath) ? WorkFolderPath : ""
            };
            try
            {
                if (dialog.ShowDialog().HasValue) WorkFolderPath = dialog.SelectedPath;
            }
            catch
            {
                // ignored
            }
        }


        #region ICloseableViewModel implementation

        public Task<bool> CloseAsync(object parameter = null)
        {
            return Empty.TrueTask;
        }
        public ICommand CloseCommand { get; set; }
        public event EventHandler<ICloseableViewModel, ViewModelClosingEventArgs> Closing;
        public event EventHandler<ICloseableViewModel, ViewModelClosedEventArgs> Closed;

        #endregion
    }
}