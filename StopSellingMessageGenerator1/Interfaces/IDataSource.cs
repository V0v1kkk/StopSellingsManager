using System.Collections.Generic;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface IDataSource
    {
        /// <summary>
        /// Path to work folder
        /// </summary>
        string WorkFolderPath { get; set; }


        /// <summary>
        /// Save list of stop sellings in storage in work foleder
        /// </summary>
        /// <param name="stopSellings">List of object which represent and discribe stop selling event</param>
        /// <returns>Return true if save was succes, and return false else</returns>
        bool SaveStopSellings(List<StopSelling> stopSellings);

        /// <summary>
        /// Load list of stop sellings from storage in work foleder
        /// </summary>
        /// <returns>List of object which represent and discribe stop selling event</returns>
        List<StopSelling> LoadStopSellings();


        /// <summary>
        /// Save list probably reasons of stop sellings to storage in work foleder
        /// </summary>
        /// <param name="reasons">List of probably reasons of stop sellings</param>
        /// <returns>Return true if save was succes, and return false else</returns>
        bool SaveReasonsOfStopSellings(List<string> reasons);

        /// <summary>
        /// Load list probably reasons of stop sellings from storage in work foleder
        /// </summary>
        /// <returns>List of strings which may be a reasons of stop selling</returns>
        List<string> LoadReasonsOfStopSellings();


        /// <summary>
        /// Save list of depatments which may be responsible for stop selling to storage in work folder
        /// </summary>
        /// <param name="departments">List of probably responsible departments for stop selling</param>
        /// <returns>Return true if save was succes, and return false else</returns>
        bool SaveResponsibleDepartments(List<string> departments);

        /// <summary>
        /// Load list of depatments which may be responsible for stop selling from storage in work folder
        /// </summary>
        /// <returns>List of strings which may be responsible department of stop selling</returns>
        List<string> LoadResponsibleDepartments();

    }
}