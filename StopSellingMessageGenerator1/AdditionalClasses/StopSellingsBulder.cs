using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StopSellingMessageGenerator.Interfaces;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    public class StopSellingsBulder : IStopSellingsBulder
    {
        private readonly ITtInformationSource _ttInformationSource;

        public StopSellingsBulder(ITtInformationSource ttInformationSource)
        {
            _ttInformationSource = ttInformationSource;
        }

        /// <summary>
        /// Build a new StopSelling object
        /// </summary>
        /// <returns>New StopSelling object, or null if InformationSource not set</returns>
        public StopSelling Build()
        {

            if (_ttInformationSource == null) return null;
            return new StopSelling(_ttInformationSource);
        }

    }
}
