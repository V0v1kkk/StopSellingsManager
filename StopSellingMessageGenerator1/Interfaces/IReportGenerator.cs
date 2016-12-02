using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface IReportGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopSelling"></param>
        /// <returns></returns>
        bool ExportToReport(StopSelling stopSelling);
    }
}