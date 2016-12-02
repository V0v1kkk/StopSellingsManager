using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using FileHelpers;
using NLog;
using StopSellingMessageGenerator.Interfaces;
using StopSellingMessageGenerator.Models;

// ReSharper disable once CheckNamespace
namespace StopSellingMessageGenerator
{
	
    // ReSharper disable once InconsistentNaming
    public class DAL : IDataSource, ITtInformationSource
    {
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		
        private static DAL _instance;
        private string _workPath;
        private readonly FileHelperEngine<PassportOfTT> _engine;
        private List<PassportOfTT> _ttPassports;



        private DAL(string workPath)
        {
            _engine = new FileHelperEngine<PassportOfTT>();
            _workPath = workPath;

            Logger.Debug($"Initialized DAL with path: {workPath}");
        }

        // ReSharper disable once InconsistentNaming
        public static DAL GetDAL(string workPath)
        {
            if (_instance == null)
            {
                _instance = new DAL(workPath);
                return _instance;
            }
            return _instance;
        }



        public List<PassportOfTT> TTPassports => _ttPassports ?? (_ttPassports = _engine.ReadFile(_workPath + "\\" + "Таблица.txt").ToList());



        public string WorkFolderPath{
			get{ return _workPath;}
			set{ _workPath = value; }}




        public bool SaveStopSellings(List<StopSelling> stopSellings)
        {
            BinaryFormatter formatter = new BinaryFormatter();
			try
			{
				using (FileStream fs = new FileStream(_workPath + "\\StopSellings.dat", FileMode.Create, FileAccess.Write, FileShare.None))
				{
					formatter.Serialize(fs, stopSellings);
				}
				return true;
			}
			catch(Exception exception)
			{
				Logger.Fatal($"Не удалось сохранить активные стоп-продажы. Ошибка {exception}");
				StringBuilder bulder = new StringBuilder();
				foreach(var stopSelling in stopSellings)
				{
					bulder.Append(stopSelling);
					bulder.Append(Environment.NewLine);
				}
				File.WriteAllText("dump.txt", bulder.ToString());
				return false;
			}
        }

        public List<StopSelling> LoadStopSellings()
        {
            BinaryFormatter formatter = new BinaryFormatter();
			try
			{
				using (FileStream fs = new FileStream(_workPath + "\\StopSellings.dat", FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					var stopSellings = (List<StopSelling>)formatter.Deserialize(fs);
					foreach(var sellings in stopSellings) sellings.DataSource = this;
					return stopSellings;
				}
			}
			catch(Exception exception)
			{
				Logger.Error($"Ошибка загрузки списка активных стоп-продаж: {exception}");
				return new List<StopSelling>(); 
			}
        }


        /*
        public bool ExportStopSelling(StopSelling stopSelling)
        {
            bool otchetFileExist = File.Exists(_workPath + "\\otchet.csv");
			try
			{
				using (FileStream aFile = new FileStream(_workPath + "\\otchet.csv", FileMode.Append, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(aFile, Encoding.GetEncoding("windows-1251")))
				{
					if(!otchetFileExist)
                    {
                        sw.WriteLine("Регион;" +
                                     "Город;" +
                                     "Номер ТТ;" +
                                     "Название ТТ;" +
                                     "Грейд;" +
                                     "Описание проблемы;" +
                                     "Наличие резервной связи (Да\\Нет);" +
                                     "Дата и время настройки резерва (МСК);" +
                                     "Время возникновения стоп-продажи (МСК);" +
                                     "Длительность стоп-продажи;" +
                                     "Комментарий;" +
                                     "Зона ответственности;" +
                                     "Проблема экскалировалась на РНИТ (Да\\Нет);" +
                                     "Время до эскалации до РНИТ"); //if we create file, we need to write header
                    }
					sw.WriteLine(stopSelling.ToString());
				}
				return true;
			}
			catch(Exception exception)
			{
				Logger.Fatal($"Не удалось сохранить стоп-продажу в отчёт. Ошибка {exception}" + Environment.NewLine + stopSelling);
				return false;
			}
        }
		
    */
		
		
		
		/// <summary>
        /// 
        /// </summary>
        /// <param name="reasons"></param>
        /// <returns></returns>
		public bool SaveReasonsOfStopSellings(List<string> reasons)
        {
			try
			{
				var serializer = new XmlSerializer(typeof(List<string>));
				using (var stream = File.OpenWrite(_workPath + "\\Reasons.xml")) //Перезаписываем
				{
					serializer.Serialize(stream, reasons);
				}
				return true;
			}
			catch(Exception exception)
			{
				Logger.Error($"Не удалось сохранить список причин стоп-продаж. Ошибка {exception}");
				return false;
			}
        }

        public List<string> LoadReasonsOfStopSellings()
        {
            var serializer = new XmlSerializer(typeof(List<string>));
			try
			{
				using (var stream = File.OpenRead(_workPath + "\\Reasons.xml"))
				{
					var other = (List<string>)(serializer.Deserialize(stream));
					return new List<string>(other);
				}
			}
			catch(Exception exception)
			{
				Logger.Error($"Ошибка загрузки списка причин стоп-продаж: {exception}");
				return new List<string>(); 
			}
        }
		
		
		
		public bool SaveResponsibleDepartments(List<string> reasons)
        {
			try
			{
				var serializer = new XmlSerializer(typeof(List<string>));
				using (var stream = File.OpenWrite(_workPath + "\\Responsibilities.xml")) //Перезаписываем
				{
					serializer.Serialize(stream, reasons);
				}
				return true;
			}            
			catch(Exception exception)
			{
				Logger.Error($"Не удалось сохранить список ответственных отделов. Ошибка {exception}");
				return false;
			}
        }

        public List<string> LoadResponsibleDepartments()
        {
            var serializer = new XmlSerializer(typeof(List<string>));
			try
			{
				using (var stream = File.OpenRead(_workPath + "\\Responsibilities.xml"))
				{
					var other = (List<string>)(serializer.Deserialize(stream));
					return new List<string>(other);
				}
			}
			catch(Exception exception)
			{
				Logger.Fatal($"Ошибка загрузки списка ответственных подразделений: {exception}");
				return new List<string>(); 
			}
        }

    }
}