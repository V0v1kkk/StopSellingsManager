using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface IStopSellingsBulder
    {
        /// <summary>
        /// Build a new StopSelling object
        /// </summary>
        /// <returns>New StopSelling object, or null if creation fail</returns>
         StopSelling Build();
    }
}
