using System;
using System.Globalization;
using System.Linq;
using System.Text;
using MugenMvvmToolkit.Models;
using NLog;
using StopSellingMessageGenerator.AdditionalClasses;
using StopSellingMessageGenerator.Interfaces;

// ReSharper disable InconsistentNaming

namespace StopSellingMessageGenerator.Models
{
	[Serializable]
    public class StopSelling : NotifyPropertyChangedBase
    {
        public StopSelling(ITtInformationSource source = null)
        {
            DataSource = source;
        }

        [NonSerialized]
        public ITtInformationSource DataSource; //Поставщик данных для модели

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _region;
        private string _city;
        private string _ttNumber;
        private string _ttName;
        private string _greid;
        private string _reason;
        private bool _reserve;
        private DateTime _reserveConfigureDateTime = DateTime.Now;
        private DateTime _startStopSellingTime;
        private string _coments;
        private string _responsibility;
        private bool _RNITNotified;

        private string _obrashenieAndTimeString;
        private string _obrashenieNumber;
        private DateTime _rnitNotifiedTime = DateTime.Now;
        private DateTime _stopStopSellingTime = DateTime.Now;
	    private string _expectedSolutionTime;

        #region Properties which setup in UI

        [FriendlyName("Номер ТТ")]
        public string TTNumber
        {
            get
            {
                return _ttNumber;
            }
            set
            {
                value = value.Trim();
                if (value == _ttNumber) return;
                _ttNumber = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Название ТТ")]
        public string TTName
        {
            get
            {
                return _ttName;
            }
            private set
            {
                if (value == _ttName) return;
                _ttName = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Регион")]
        public string Region
        {
            get
            {
                return _region;
            }
            private set
            {
                if (value == _region) return;
                _region = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Населённый пункт")]
        public string City
        {
            get
            {
                return _city;
            }
            private set
            {
                if (value == _city) return;
                _city = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Грейд")]
        public string Greid
        {
            get
            {
                return _greid;
            }
            private set
            {
                if (value == _greid) return;
                _greid = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Номер обращения с датой и временем")]
        public string ObrashenieAndTimeStringAdapter //Обращение 00000547008 от 21.11.2016 12:07:27
        {
            get
            {
                return _obrashenieAndTimeString;
            }
            set
            {
                _obrashenieAndTimeString = value;
                OnPropertyChanged();

                string[] partsOfString = value.Split(' ');
                if (partsOfString.Length != 5) return;
                ObrashenieNumber = partsOfString[1];
                try
                {
                    string format = "dd.MM.yyyy H:mm:ss"; // https://msdn.microsoft.com/ru-ru/library/8kb3ddd4(v=vs.110).aspx
                    var result = DateTime.ParseExact(partsOfString[3] + " " + partsOfString[4], format, CultureInfo.InvariantCulture);
                    StartStopSellingTime = result;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Ticket number and date parse error. Text: {value}, Exeption: {ex.Message}");
                }
            }
        }

        [FriendlyName("Причина")]
        public string Reason
        {
            get { return _reason; }
            set
            {
                if (value == _reason) return;
                _reason = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Ответственное подразделение")]
        public string Responsibility
        {
            get
            {
                return _responsibility;
            }
            set
            {
                if (value == _responsibility) return;
                _responsibility = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Время возникновения стоп-продажи (МСК)")]
        public DateTime StartStopSellingTime
        {
            get
            {
                return _startStopSellingTime;
            }
            set
            {
                if (value == _startStopSellingTime) return;
                _startStopSellingTime = value;
                OnPropertyChanged();
                OnPropertyChanged("DurationStopSelling");
            }
        }

        [FriendlyName("Время окончания стоп-продажи (МСК)")]
        public DateTime StopStopSellingTime
        {
            get
            {
                return _stopStopSellingTime;
            }
            set
            {
                if (value == _stopStopSellingTime) return;
                _stopStopSellingTime = value;
                OnPropertyChanged();
                OnPropertyChanged("DurationStopSelling");
            }
        }

        [FriendlyName("Описание стоп-продажи")]
        public string Coments
        {
            get { return _coments; }
            set
            {
                if (value == _coments) return;
                _coments = value;
                OnPropertyChanged();
            }
        }


        [FriendlyName("Сроки решения")]
        public string ExpectedSolutionTime
        {
            get { return _expectedSolutionTime; }
            set
            {
                if (value == _expectedSolutionTime) return;
                _expectedSolutionTime = value;
                OnPropertyChanged();
            }
        }


        [FriendlyName("Резерв настраивался? (Да/Нет)")]
        public bool Reserve
        {
            get { return _reserve; }
            set
            {
                if (value == _reserve) return;
                _reserve = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Время настройки резерва (МСК)")]
        public DateTime ReserveConfigureDateTime
        {
            get { return _reserveConfigureDateTime; }
            set
            {
                if (value.Equals(_reserveConfigureDateTime)) return;
                _reserveConfigureDateTime = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("РНИТ информировался? (Да/Нет)")]
        public bool RnitNotified
        {
            get { return _RNITNotified; }
            set
            {
                if (value == _RNITNotified) return;
                _RNITNotified = value;
                OnPropertyChanged();
            }
        }

        [FriendlyName("Время информирования РНИТ (МСК)")]
        public DateTime RnitNotifiedTime
        {
            get { return _rnitNotifiedTime; }
            set
            {
                if (value == _rnitNotifiedTime) return;
                _rnitNotifiedTime = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region  Other properties

        [FriendlyName("Продолжительность стоп-продажи")]
        public string DurationStopSelling
        {
            get
            {
                TimeSpan span = StopStopSellingTime - StartStopSellingTime;
                StringBuilder stringBuilder = new StringBuilder();

                if (span.Days > 0)
                {
                    stringBuilder.Append($"{span.Days} ");
                    stringBuilder.Append(Helper.GetDeclension(span.Days, "день", "дня", "дней"));
                }
                if (span.Hours > 0)
                {
                    if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                    stringBuilder.Append($"{span.Hours} ");
                    stringBuilder.Append(Helper.GetDeclension(span.Hours, "час", "часа", "часов"));
                }
                if (span.Minutes > 0)
                {
                    if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                    stringBuilder.Append($"{span.Minutes} ");
                    stringBuilder.Append(Helper.GetDeclension(span.Minutes, "минуа", "минуты", "минут"));
                }
                return stringBuilder.ToString();
            }
        }

        [FriendlyName("Время до информирования РНИТ")]
        public string TimeToRnitNotified
        {
            get
            {
                if (!RnitNotified) return "";

                TimeSpan span = RnitNotifiedTime - StartStopSellingTime;
                StringBuilder stringBuilder = new StringBuilder();
                if (span.Days > 0)
                {
                    stringBuilder.Append($"{span.Days} ");
                    stringBuilder.Append(Helper.GetDeclension(span.Days, "день", "дня", "дней"));
                }
                if (span.Hours > 0)
                {
                    if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                    stringBuilder.Append($"{span.Hours} ");
                    stringBuilder.Append(Helper.GetDeclension(span.Hours, "час", "часа", "часов"));
                }
                if (span.Minutes > 0)
                {
                    if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                    stringBuilder.Append($"{span.Minutes} ");
                    stringBuilder.Append(Helper.GetDeclension(span.Minutes, "минута", "минуты", "минут"));
                }
                return stringBuilder.ToString();
            }
        }


        [FriendlyName("Номер обращения")]
        public string ObrashenieNumber
        {
            get { return _obrashenieNumber; }
            set
            {
                if (value == _obrashenieNumber) return;
                _obrashenieNumber = value;
                OnPropertyChanged();
            }
        }

        
        [FriendlyName("Продолжительность стоп-продажи")]
        public string DurationStopSellingInHoursAndMinutes
        {
            get
            {
                TimeSpan span = StopStopSellingTime - StartStopSellingTime;
                StringBuilder stringBuilder = new StringBuilder();

                int hours = (int)Math.Floor(span.Minutes / 60.0);
                if (hours > 0)
                {
                    stringBuilder.Append(hours);
                    stringBuilder.Append(":");
                }
                else
                {
                    stringBuilder.Append("00:");
                }

                int minutes = span.Minutes - hours * 60;
                if (minutes > 0)
                {
                    stringBuilder.Append(minutes);
                }
                else
                {
                    stringBuilder.Append("00");
                }

                return stringBuilder.ToString();
            }
        }




        #endregion


	    #region Methods

        public void CheckTTnumber()
        {
            var passportTt = DataSource.TTPassports.FirstOrDefault(x => x.Code.Contains(TTNumber.Trim()));
            if (passportTt == null)
            {
                Logger.Error($"TT not found in dictionary. TTNumber: {TTNumber}");

                TTName = "";
                Region = "";
                City = "";
                Greid = "";

                return;
            }

            TTName = passportTt.Name;
            Region = passportTt.Region;
            City = passportTt.City;
            Greid = passportTt.GreidFact.HasValue ? passportTt.GreidFact.ToString() : "";
        }

        public override string ToString()
        {
            return ToString(';');
        }

        public string ToString(char splitter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Region);
            stringBuilder.Append(splitter);

            stringBuilder.Append(City);
            stringBuilder.Append(splitter);

            stringBuilder.Append(TTNumber);
            stringBuilder.Append(splitter);

            stringBuilder.Append(TTName);
            stringBuilder.Append(splitter);

            stringBuilder.Append(Greid);
            stringBuilder.Append(splitter);

            stringBuilder.Append(Reason);
            stringBuilder.Append(splitter);

            stringBuilder.Append(Reserve ? "Да" : "Нет");
            stringBuilder.Append(splitter);

            if(Reserve) stringBuilder.Append(ReserveConfigureDateTime);
            else stringBuilder.Append("-");
            stringBuilder.Append(splitter);

            stringBuilder.Append(StartStopSellingTime);
            stringBuilder.Append(splitter);

            stringBuilder.Append(DurationStopSellingInHoursAndMinutes);
            stringBuilder.Append(splitter);

            stringBuilder.Append(Coments);
            stringBuilder.Append(splitter);

            stringBuilder.Append(Responsibility);
            stringBuilder.Append(splitter);

            stringBuilder.Append(RnitNotified ? "Да" : "Нет");
            stringBuilder.Append(splitter);

            if (RnitNotified) stringBuilder.Append(RnitNotifiedTime);
            else stringBuilder.Append("-");
            stringBuilder.Append(splitter);

            if (RnitNotified) stringBuilder.Append(TimeToRnitNotified);

            return stringBuilder.ToString();
        }

        #endregion
    }
}