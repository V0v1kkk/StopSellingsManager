using StopSellingMessageGenerator.Enums;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface IMessageTextGenerator
    {
        /// <summary>
        /// Method for generating text of message that discribe stop selling
        /// </summary>
        /// <param name="stopSelling">Object which represent and discribe stop selling event</param>
        /// <param name="enum">Generate message for open or for close stop selling</param>
        /// <returns>Retun result message as string</returns>
        string GenerateText(StopSelling stopSelling, MessageTypeEnum @enum);
    }
}