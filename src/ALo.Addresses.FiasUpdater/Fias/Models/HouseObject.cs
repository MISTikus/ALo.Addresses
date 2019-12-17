using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias.Models
{
    [XmlRoot(ElementName = "House")]
    public class HouseObject
    {
        /// <summary>
        /// Почтовый индекс
        /// </summary>
        [XmlAttribute("POSTALCODE"), StringLength(6)]
        public string PostalCode { get; set; }
        /// <summary>
        /// Код региона
        /// </summary>
        [XmlAttribute("REGIONCODE"), StringLength(2)]
        public string RegionCode { get; set; }
        /// <summary>
        /// Код ИФНС ФЛ
        /// </summary>
        [XmlAttribute("IFNSFL"), StringLength(4)]
        public string IndividualTaxOfficeCode { get; set; }
        /// <summary>
        /// Код территориального участка ИФНС ФЛ
        /// </summary>
        [XmlAttribute("TERRIFNSFL"), StringLength(4)]
        public string TerritorialIndividualTaxOfficeCode { get; set; }
        /// <summary>
        /// Код ИФНС ЮЛ
        /// </summary>
        [XmlAttribute("IFNSUL"), StringLength(4)]
        public string LegalEntityTaxOfficeCode { get; set; }
        /// <summary>
        /// Код территориального участка ИФНС ЮЛ
        /// </summary>
        [XmlAttribute("TERRIFNSUL"), StringLength(4)]
        public string TerritorualLegalEntityTaxOfficeCode { get; set; }
        /// <summary>
        /// ОКАТО
        /// </summary>
        [XmlAttribute("OKATO"), StringLength(11)]
        public string Okato { get; set; }
        /// <summary>
        /// ОКTMO
        /// </summary>
        [XmlAttribute("OKTMO"), StringLength(11)]
        public string Oktmo { get; set; }
        /// <summary>
        /// Дата время внесения (обновления) записи
        /// </summary>
        [XmlAttribute("UPDATEDATE")]
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// Номер дома
        /// </summary>
        [XmlAttribute("HOUSENUM"), StringLength(20)]
        public string HouseNumber { get; set; }
        /// <summary>
        /// Признак владения
        /// </summary>
        [XmlAttribute("ESTSTATUS")]
        public int OwnershipStatus { get; set; }
        /// <summary>
        /// Номер корпуса
        /// </summary>
        [XmlAttribute("BUILDNUM"), StringLength(10)]
        public string BuildingNumber { get; set; }
        /// <summary>
        /// Номер строения
        /// </summary>
        [XmlAttribute("STRUCNUM"), StringLength(10)]
        public string StructureNumber { get; set; }
        /// <summary>
        /// Признак строения
        /// </summary>
        [XmlAttribute("STRSTATUS")]
        public int StructureStatus { get; set; }
        /// <summary>
        /// Уникальный идентификатор записи дома
        /// </summary>
        [XmlAttribute("HOUSEID")]
        public Guid Id { get; set; }
        /// <summary>
        /// Глобальный уникальный идентификатор дома
        /// </summary>
        [XmlAttribute("HOUSEGUID")]
        public Guid GlobalId { get; set; }
        /// <summary>
        /// Guid записи родительского объекта (улицы, города, населенного пункта и т.п.)
        /// </summary>
        [XmlAttribute("AOGUID")]
        public Guid AddressId { get; set; }
        /// <summary>
        /// Начало действия записи
        /// </summary>
        [XmlAttribute("STARTDATE")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Окончание действия записи
        /// </summary>
        [XmlAttribute("ENDDATE")]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Состояние дома
        /// </summary>
        [XmlAttribute("STATSTATUS")]
        public int BuildingState { get; set; }
        /// <summary>
        /// Внешний ключ на нормативный документ
        /// </summary>
        [XmlAttribute("NORMDOC")]
        public Guid DocumentId { get; set; }
        /// <summary>
        /// Счетчик записей зданий, сооружений для формирования классификационного кода
        /// </summary>
        [XmlAttribute("COUNTER")]
        public int Counter { get; set; }
        /// <summary>
        /// Кадастровый номер
        /// </summary>
        [XmlAttribute("CADNUM"), StringLength(100)]
        public string CadastreNumber { get; set; }
        /// <summary>
        /// Тип деления: 
        /// 0 – не определено
        /// 1 – муниципальное
        /// 2 – административное
        /// </summary>
        [XmlAttribute("DIVTYPE")]
        public int DivisionType { get; set; }
    }
}
