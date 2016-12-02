using FileHelpers;
// ReSharper disable InconsistentNaming
#pragma warning disable 169

namespace StopSellingMessageGenerator.Models
{
    [DelimitedRecord("	")]
    [IgnoreFirst]
    [IgnoreEmptyLines]
    public class PassportOfTT
    {
        [FieldOptional]
        public string Code;
        [FieldOptional]
        public string Index;
        [FieldOptional]
        public string Name;
        [FieldOptional]
        public string Status;
        [FieldOptional]
        public string ChanelOfDistrib;
        [FieldOptional]
        public string Format;
        [FieldOptional]
        public string GreidPlan;
        [FieldOptional]
        public int? GreidFact;
        [FieldOptional]
        public string OpenDatePalan;
        [FieldOptional]
        public string OpenDateFact;
        [FieldOptional]
        public string CloseDatePlan;
        [FieldOptional]
        public string CloseDateFact;
        [FieldOptional]
        public string OpenSrok;
        [FieldOptional]
        public string Region;
        [FieldOptional]
        public string Oblast;
        [FieldOptional]
        public string City;



        [FieldOptional]
        private string Okrug;
        [FieldOptional]
        private string Adress;
        [FieldOptional]
        private string AdditionalAdress;
        [FieldOptional]
        private string Long;
        [FieldOptional]
        private string Lat;
        [FieldOptional]
        private string SubwayStantion;
        [FieldOptional]
        private string WorkShedule;
        [FieldOptional]
        private string Phone;
        [FieldOptional]
        private string IpPhone;
        [FieldOptional]
        private string Email;
        [FieldOptional]
        private string Recipient;
        [FieldOptional]
        private string OmPhone;
        [FieldOptional]
        private string UmPhone;
        [FieldOptional]
        private string OmName;
        [FieldOptional]
        private string UmName;
        [FieldOptional]
        private string OmDeputyPhone;
        [FieldOptional]
        private string UmDeputyPhone;
        [FieldOptional]
        private string OmDeputy;
        [FieldOptional]
        private string UmDeputy;
        [FieldOptional]
        private string DTOName;
        [FieldOptional]
        private string DirectorTTPhone;
        [FieldOptional]
        private string DirectorTTName;
        [FieldOptional]
        private string DocumentaionDepartmentManager;
        [FieldOptional]
        private string RentalDepartmentManager;
        [FieldOptional]
        private string LegalEntity;
        [FieldOptional]
        private string SigningOfLeaseDate;
        [FieldOptional]
        private string FormOfContract;
        [FieldOptional]
        private string Sublease;
        [FieldOptional]
        private string OtherField1; //Срок действия договора аренды
        [FieldOptional]
        private string OtherField2; //Начало оплаты Аренды
        [FieldOptional]
        private string OtherField3; //Передача помещения (план)
        [FieldOptional]
        private string OtherField4; //Передача помещения (факт)
        [FieldOptional]
        private string OtherField5; //Приемка МОТТ
        [FieldOptional]
        private string OtherField6; //Приемка ТТ ОМом
        [FieldOptional]
        private string OtherField7; //Монтаж РК
        [FieldOptional]
        private string OtherField8; //Регистрация рекламы
        [FieldOptional]
        private string OtherField9; //Начало ремонта (план)
        [FieldOptional]
        private string OtherField10; //НачалаРемонтаФакт
        [FieldOptional]
        private string OtherField11; //Окончание ремонта (план)
        [FieldOptional]
        private string OtherField12; //Окончание ремонта (факт)
        [FieldOptional]
        private string OtherField13; //Завоз оборудования (план)
        [FieldOptional]
        private string OtherField14; //Завоз оборудования (планир)
        [FieldOptional]
        private string OtherField15; //Завоз оборудования (факт)
        [FieldOptional]
        private string OtherField16; //Начало сборки оборудования
        [FieldOptional]
        private string OtherField17; //Окончание сборки оборудования
        [FieldOptional]
        private string OtherField18; //Монтаж ИТ-оборудования
        [FieldOptional]
        private string OtherField19; //Начало ребрендирования
        [FieldOptional]
        private string OtherField20; //Окончание ребрендирования
        [FieldOptional]
        private string OtherField21; //Переделана из ТТ
        [FieldOptional]
        private string OtherField22; //Переделана с
        [FieldOptional]
        private string OtherField23; //Переделана по
        [FieldOptional]
        private string OtherField24; //Выделенная мощность по договору
        [FieldOptional]
        private string OtherField25; //Общая площадь по договору
        [FieldOptional]
        private string OtherField26; //Торговая площадь
        [FieldOptional]
        private string OtherField27; //Наличие шкаф-купе
        [FieldOptional]
        private string OtherField28; //Наличие места для терминала
        [FieldOptional]
        private string OtherField29; //Площадь склада
        [FieldOptional]
        private string OtherField30; //Площадь подсобного помещения
        [FieldOptional]
        private string OtherField31; //Суммарный метраж витрин
        [FieldOptional]
        private string OtherField32; //Дата создания в базе 
    }
}