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
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace StopSellingMessageGenerator
{
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

		
		public bool SaveReasonsOfStopSellings(List<string> reasons)
        {
			try
			{
				var serializer = new XmlSerializer(typeof(List<string>));
				using (FileStream fs = new FileStream(_workPath + "\\Reasons.xml", FileMode.Create, FileAccess.Write, FileShare.None))
				{
					serializer.Serialize(fs, reasons);
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
		
		
		
		public bool SaveResponsibleDepartments(List<string> departments)
        {
			try
			{
				var serializer = new XmlSerializer(typeof(List<string>));
				using (FileStream fs = new FileStream(_workPath + "\\Responsibilities.xml", FileMode.Create, FileAccess.Write, FileShare.None))
				{
					serializer.Serialize(fs, departments);
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