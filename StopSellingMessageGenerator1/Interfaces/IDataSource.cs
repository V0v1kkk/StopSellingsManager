using System.Collections.Generic;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface IDataSource
    {
        string WorkFolderPath { get; set; }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopSelling"></param>
        bool ExportStopSelling(StopSelling stopSelling);*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopSellings"></param>
        bool SaveStopSellings(List<StopSelling> stopSellings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<StopSelling> LoadStopSellings();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reasons"></param>
        bool SaveReasonsOfStopSellings(List<string> reasons);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<string> LoadReasonsOfStopSellings();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopSellings"></param>
        bool SaveResponsibleDepartments(List<string> stopSellings);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<string> LoadResponsibleDepartments();

    }
}