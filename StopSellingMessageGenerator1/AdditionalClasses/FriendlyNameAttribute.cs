using System;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FriendlyNameAttribute : Attribute
    {
         public string FriendlyName { get; set; }

        public FriendlyNameAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }
    }
}