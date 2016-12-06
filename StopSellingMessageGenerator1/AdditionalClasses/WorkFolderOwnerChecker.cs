using System;
using System.IO;
using NLog;
using StopSellingMessageGenerator.Interfaces;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    class WorkFolderOwnerChecker : IWorkFolderOwnerChecker
    {
        private readonly string _workFolder;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public WorkFolderOwnerChecker(string workFolder)
        {
            _workFolder = workFolder;
        }


        public bool WorkFolderExist()
        {
            return Directory.Exists(_workFolder);
        }

        public bool MeIsOwner()
        {
            try
            {
                if (!File.Exists(_workFolder + "\\owner.txt"))
                {
                    MakeMeOwner();
                    return true;
                }

                var ownerFileText = File.ReadAllText(_workFolder + "\\owner.txt");
                var ownerFileTextParts = ownerFileText.Split(';');
                if (ownerFileTextParts.Length < 2) return false;

                if (ownerFileTextParts[0] != Environment.UserName) return false;

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error("Erron on take ownership. Error message: " + exception);
                return false;
            }
        }

        public void MakeMeOwner()
        {
            File.WriteAllText(_workFolder + "\\owner.txt", Environment.UserName + ';' + DateTime.Now);
        }

        public string GetOwnerData()
        {
            try
            {
                var ownerFileText = File.ReadAllText(_workFolder + "\\owner.txt");
                var ownerFileTextParts = ownerFileText.Split(';');
                if (ownerFileTextParts.Length < 2) return "";
                return $"{ownerFileTextParts[0]}, дата и время последнего сброса данных на диск: {ownerFileTextParts[1]}";
            }
            catch (Exception exception)
            {
                Logger.Error("Erron on getting owner information. Error message: " + exception);
                return "";
            }
        }

        public void ClearOwnership()
        {
            try
            {
                if(File.Exists(_workFolder + "\\owner.txt")) File.Delete(_workFolder + "\\owner.txt");
            }
            catch(Exception exception)
            {
                Logger.Error("Erron on clear ownership. Error message: " + exception);
            }
        }
    }
}
