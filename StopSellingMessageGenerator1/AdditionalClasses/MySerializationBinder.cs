using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    internal sealed class DictionarySerializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.Contains("List"))
                return Type.GetType("System.Collections.Generic.List`1[StopSellingMessageGenerator.Models.StopSelling]");
            return Type.GetType(
                $"{"StopSellingMessageGenerator.Models.StopSelling"}, {Assembly.GetExecutingAssembly().FullName}");
        }

    }
}