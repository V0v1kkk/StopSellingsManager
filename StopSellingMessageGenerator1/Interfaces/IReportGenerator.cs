using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface IReportGenerator
    {
        /// <summary>
        /// Export to report StopSelling object.
        /// </summary>
        /// <param name="stopSelling">Object which represent and discribe stop selling event.</param>
        /// <returns>Return true if export was success, and return false if export was fail</returns>
        bool ExportToReport(StopSelling stopSelling);
    }
}