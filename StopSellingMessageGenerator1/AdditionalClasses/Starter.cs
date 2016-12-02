using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MugenMvvmToolkit;
using StopSellingMessageGenerator.ViewModels;

namespace StopSellingMessageGenerator
{
    class Starter : MvvmApplication
    {
        public override Type GetStartViewModelType()
        {
            return typeof (MainViewModel);
        }
    }
}
