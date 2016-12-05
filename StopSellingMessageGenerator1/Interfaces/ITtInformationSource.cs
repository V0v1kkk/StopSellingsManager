using System.Collections.Generic;
using StopSellingMessageGenerator.Models;
// ReSharper disable InconsistentNaming

namespace StopSellingMessageGenerator.Interfaces
{
    public interface ITtInformationSource
    {
        /// <summary>
        /// List of objects represent passports of trade points.
        /// </summary>
        List<PassportOfTT> TTPassports { get; }
    }
}