using System.Collections.Generic;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface ITtInformationSource
    {
        /// <summary>
        /// 
        /// </summary>
        List<PassportOfTT> TTPassports { get; }
    }
}