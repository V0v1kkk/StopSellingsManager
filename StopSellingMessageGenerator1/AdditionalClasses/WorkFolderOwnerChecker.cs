using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StopSellingMessageGenerator.Interfaces;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    class WorkFolderOwnerChecker : IWorkFolderOwnerChecker
    {
        private readonly string _workFolder;

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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return "";
            }
        }
    }
}
