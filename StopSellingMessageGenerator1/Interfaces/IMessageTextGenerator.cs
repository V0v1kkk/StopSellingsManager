using StopSellingMessageGenerator.Enums;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.Interfaces
{
    public interface IMessageTextGenerator
    {
        string GenerateText(StopSelling stopSelling, MessageTypeEnum @enum);
    }
}