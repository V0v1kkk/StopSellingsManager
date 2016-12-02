using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using StopSellingMessageGenerator.Enums;
using StopSellingMessageGenerator.Interfaces;
using StopSellingMessageGenerator.Models;
using System.Reflection;
using System.Text.RegularExpressions;
using NLog;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    public class MessageTextGeneratorReflection : IMessageTextGenerator
    {
        private readonly string _workFolderPath;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public MessageTextGeneratorReflection(string workFolderPath)
        {
            _workFolderPath = workFolderPath;
        }

        public string GenerateText(StopSelling stopSelling, MessageTypeEnum @enum)
        {
            if (stopSelling == null) return "";

            TypeInfo stopSellingInfo = stopSelling.GetType().GetTypeInfo();
            IEnumerable<PropertyInfo> propertyInfos = stopSellingInfo.DeclaredProperties;
            var infos = propertyInfos as IList<PropertyInfo> ?? propertyInfos.ToList();

            string textTemplate = @enum == MessageTypeEnum.StartStopSellingMessage ? GetOpenTemplateText() : GetCloseTemplateText();
            if (string.IsNullOrEmpty(textTemplate)) return "";

            Regex regex = new Regex(@"%\w*%", RegexOptions.Compiled);
            var matches = regex.Matches(textTemplate);
            var uniqueMatches = matches.OfType<Match>().Select(m => m.Value).Distinct();

            foreach (string match in uniqueMatches)
            {
                string macrosName = match.Replace("%","");
                var propery = infos.FirstOrDefault(x => x.Name == macrosName);
                if (propery != null)
                {
                    var getMethod = propery.GetGetMethod();
                    var result = getMethod.Invoke(stopSelling, null);
                    string converted = "";
                    if (result is string)
                    {
                        converted = result.ToString();
                    }
                    else if(result is DateTime)
                    {
                        DateTime temp = (DateTime)result;
                        converted = temp.ToString("dd.MM.yyyy HH:mm:ss");
                    }
                    else if(result is bool)
                    {
                        bool temp = (bool) result;
                        converted = temp ? "Да" : "Нет";
                    }
                    
                    if (!string.IsNullOrEmpty(converted))
                    {
                        textTemplate = textTemplate.Replace(match, converted);
                    }
                }
               
            }
            return DecodeEncodedNonAsciiCharacters(textTemplate);
        }

        string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
        }

        private string GetOpenTemplateText()
        {
            return CheckFileExistAndGetText("open.txt");
        }

        private string GetCloseTemplateText()
        {
            return CheckFileExistAndGetText("close.txt");
        }

        private string CheckFileExistAndGetText(string filename)
        {
            try
            {
                if (!File.Exists(_workFolderPath + $"\\{filename}"))
                {
                    Logger.Error("Файл с шаблоном сообщения " + _workFolderPath + $"\\{filename}" + ". Не существует.");
                    return "";
                }
                return File.ReadAllText(_workFolderPath + $"\\{filename}");
            }
            catch (Exception exception)
            {
                Logger.Error("Ошибка чтения шаблона сообщения: " + _workFolderPath + $"\\{filename}" + ". " + exception);
                return "";
            }
            
        }
    }
}
