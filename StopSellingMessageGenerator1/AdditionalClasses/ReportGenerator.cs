using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using StopSellingMessageGenerator.Interfaces;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    class ReportGenerator : IReportGenerator
    {
        private readonly string _workFolderPath;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ReportGenerator(string workFolderPath)
        {
            _workFolderPath = workFolderPath;
        }

        public bool ExportToReport(StopSelling stopSelling)
        {
            if (stopSelling == null) return false;

            TypeInfo stopSellingInfo = stopSelling.GetType().GetTypeInfo();
            IEnumerable<PropertyInfo> propertyInfos = stopSellingInfo.DeclaredProperties;
            var infos = propertyInfos as List<PropertyInfo> ?? propertyInfos.ToList();

            bool reportFileExist = File.Exists(_workFolderPath + "\\Report.csv");
            try
            {
                using (FileStream fileStream = new FileStream(_workFolderPath + "\\Report.csv", FileMode.Append, FileAccess.Write))
                using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding("windows-1251")))
                {
                    if (!reportFileExist)
                    {
                        WriteLine(streamWriter, stopSelling ,infos, true);                  
                    }
                    WriteLine(streamWriter, stopSelling, infos, false);
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.Fatal($"Не удалось сохранить стоп-продажу в отчёт. Ошибка {exception}" + Environment.NewLine + stopSelling);
                return false;
            }
        }

        private void WriteLine(StreamWriter streamWriter, StopSelling stopSelling, List<PropertyInfo> propertyInfos, bool isHeader)
        {
            var headerTemplate = File.ReadAllText(_workFolderPath + "\\output.txt");

            Regex regex = new Regex(@"%\w*%", RegexOptions.Compiled);
            var matches = regex.Matches(headerTemplate);
            var uniqueMatches = matches.OfType<Match>().Select(m => m.Value).Distinct();

            foreach (string match in uniqueMatches)
            {
                string macrosName = match.Replace("%", "");
                var propery = propertyInfos.FirstOrDefault(x => x.Name == macrosName);
                if (propery != null)
                {
                    if (isHeader)
                    {
                        foreach (var attribute in propery.GetCustomAttributes(true))
                        {
                            FriendlyNameAttribute friendlyNameAttribute = attribute as FriendlyNameAttribute;
                            if (friendlyNameAttribute != null)
                            {
                                headerTemplate = headerTemplate.Replace(match, friendlyNameAttribute.FriendlyName);
                                break; //if we find needed attribure, break attribure iterating
                            }
                        }
                    }
                    else
                    {
                        var getMethod = propery.GetGetMethod();
                        var result = getMethod.Invoke(stopSelling, null);
                        string converted = "";
                        if (result is string)
                        {
                            converted = result.ToString();
                        }
                        else if (result is DateTime)
                        {
                            DateTime temp = (DateTime)result;
                            converted = temp.ToString("dd.MM.yyyy HH:mm:ss");
                        }
                        else if (result is bool)
                        {
                            bool temp = (bool)result;
                            converted = temp ? "Да" : "Нет";
                        }

                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (!string.IsNullOrEmpty(converted))
                        {
                            headerTemplate = headerTemplate.Replace(match, converted);
                        }
                        else
                        {
                            headerTemplate = headerTemplate.Replace(match, "");
                        }
                    }
                }
            }
            streamWriter.WriteLine(headerTemplate);
        }

    }
}
