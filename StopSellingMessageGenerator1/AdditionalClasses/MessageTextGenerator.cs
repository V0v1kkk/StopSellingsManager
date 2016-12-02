using System;
using StopSellingMessageGenerator.Enums;
using StopSellingMessageGenerator.Interfaces;
using StopSellingMessageGenerator.Models;

namespace StopSellingMessageGenerator.AdditionalClasses
{
    [Obsolete("Позволял генерировать только чётко заданные сообщения. Заменён вариантом с рефлексией.")]
    public class MessageTextGenerator : IMessageTextGenerator
    {

        [Obsolete("Позволял генерировать только чётко заданные сообщения. Заменён вариантом с рефлексией.")]
        public string GenerateText(StopSelling stopSelling, MessageTypeEnum messageTypeEnum)
        {
            return messageTypeEnum == MessageTypeEnum.StartStopSellingMessage ? $"\u26D4 Регион {stopSelling.Region}. {stopSelling.City}. " +
                                                                                       $"Открыт инцидент {stopSelling.ObrashenieAndTimeStringAdapter} — {stopSelling.Reason}. " +
                                                                                       $"ЦМС {stopSelling.TTNumber} {stopSelling.TTName} ({stopSelling.Greid} грейд). " +
                                                                                       $"По обращению проводится диагностика. Зона ответственности - {stopSelling.Responsibility}. " +
                                                                                       $"Длительность стоп-продажи  {stopSelling.DurationStopSelling}. " +
                                                                                       "Дежурный: +7 929 212 77 85." 
																					   : 
                                                                                       $"\u2705 Регион {stopSelling.Region} {stopSelling.City}. " +
                                                                                       $"Закрыт инцидент {stopSelling.ObrashenieAndTimeStringAdapter} - {stopSelling.Reason}. " +
                                                                                       $"ЦМС {stopSelling.TTNumber} {stopSelling.TTName} ({stopSelling.Greid} грейд). " +
                                                                                       $"Зона ответственности - {stopSelling.Responsibility}. " +
                                                                                       $"Длительность стоп-продажи {stopSelling.DurationStopSelling}. " +
                                                                                       "Дежурный: +7 929 212 77 85.";
        }
    }
}