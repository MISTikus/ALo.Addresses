using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias.Models
{
    [XmlRoot(ElementName = "Object")]
    public class AddressObject
    {
        /// <summary>
        /// Глобальный уникальный идентификатор адресного объекта
        /// </summary>
        [XmlAttribute("AOGUID")]
        public Guid GlobalId { get; set; }
        /// <summary>
        /// Формализованное наименование
        /// </summary>
        [XmlAttribute("FORMALNAME"), StringLength(120)]
        public string FormalName { get; set; }
        /// <summary>
        /// Код региона
        /// </summary>
        [XmlAttribute("REGIONCODE"), StringLength(2)]
        public string RegionCode { get; set; }
        /// <summary>
        /// Код автономии
        /// </summary>
        [XmlAttribute("AUTOCODE"), StringLength(1)]
        public string AuthonomyCode { get; set; }
        /// <summary>
        /// Код района
        /// </summary>
        [XmlAttribute("AREACODE"), StringLength(3)]
        public string AreaCode { get; set; }
        /// <summary>
        /// Код города
        /// </summary>
        [XmlAttribute("CITYCODE"), StringLength(3)]
        public string CityCode { get; set; }
        /// <summary>
        /// Код внутригородского района
        /// </summary>
        [XmlAttribute("CTARCODE"), StringLength(3)]
        public string CityDistrictNumber { get; set; }
        /// <summary>
        /// Код населенного пункта
        /// </summary>
        [XmlAttribute("PLACECODE"), StringLength(3)]
        public string PlaceCode { get; set; }
        /// <summary>
        /// Код элемента планировочной структуры
        /// </summary>
        [XmlAttribute("PLANCODE"), StringLength(4)]
        public string PlanningStructureCode { get; set; }
        /// <summary>
        /// Код улицы
        /// </summary>
        [XmlAttribute("STREETCODE"), StringLength(4)]
        public string StreetCode { get; set; }
        /// <summary>
        /// Код дополнительного адресообразующего элемента
        /// </summary>
        [XmlAttribute("EXTRCODE"), StringLength(4)]
        public string ExtraCode { get; set; }
        /// <summary>
        /// Код подчиненного дополнительного адресообразующего элемента
        /// </summary>
        [XmlAttribute("SEXTCODE"), StringLength(3)]
        public string SubExtraCode { get; set; }
        /// <summary>
        /// Официальное наименование
        /// </summary>
        [XmlAttribute("OFFNAME"), StringLength(120)]
        public string OfficialName { get; set; }
        /// <summary>
        /// Почтовый индекс
        /// </summary>
        [XmlAttribute("POSTALCODE"), StringLength(6)]
        public string PostalCode { get; set; }
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
        /// Краткое наименование типа объекта
        /// </summary>
        [XmlAttribute("SHORTNAME"), StringLength(10)]
        public string ShortTypeName { get; set; }
        /// <summary>
        /// Уровень адресного объекта 
        /// </summary>
        [XmlAttribute("AOLEVEL")]
        public int Level { get; set; }
        /// <summary>
        /// Идентификатор объекта родительского объекта
        /// </summary>
        [XmlAttribute("PARENTGUID")]
        public Guid ParentId { get; set; }
        /// <summary>
        /// Уникальный идентификатор записи. Ключевое поле
        /// </summary>
        [XmlAttribute("AOID")]
        public Guid Id { get; set; }
        /// <summary>
        /// Идентификатор записи связывания с предыдушей исторической записью
        /// </summary>
        [XmlAttribute("PREVID")]
        public Guid PreviousHistoryId { get; set; }
        /// <summary>
        /// Идентификатор записи  связывания с последующей исторической записью
        /// </summary>
        [XmlAttribute("NEXTID")]
        public Guid NextHistoryId { get; set; }
        /// <summary>
        /// Код адресного элемента одной строкой с признаком актуальности из классификационного кода (КЛАДР?)
        /// </summary>
        [XmlAttribute("CODE"), StringLength(17)]
        public string Code { get; set; }
        /// <summary>
        /// Код адресного элемента одной строкой без признака актуальности (последних двух цифр) (КЛАДР?)
        /// </summary>
        [XmlAttribute("PLAINCODE"), StringLength(15)]
        public string PlainCode { get; set; }
        /// <summary>
        /// Статус последней исторической записи в жизненном цикле адресного объекта:
        /// 0 – Не последняя
        /// 1 - Последняя
        /// </summary>
        [XmlAttribute("ACTSTATUS")]
        public int ActualityStatus { get; set; }
        /// <summary>
        /// Статус актуальности адресного объекта ФИАС на текущую дату:
        /// 0 – Не актуальный
        /// 1 - Актуальный
        /// </summary>
        [XmlAttribute("LIVESTATUS")]
        public int CurrentStatus { get; set; }
        /// <summary>
        /// Статус центра
        /// </summary>
        [XmlAttribute("CENTSTATUS")]
        public int CenterStatus { get; set; }
        /// <summary>
        /// Статус действия над записью – причина появления записи (см. OperationStatuses )
        /// </summary>
        [XmlAttribute("OPERSTATUS")]
        public int OperationStatus { get; set; }
        /// <summary>
        /// Статус актуальности КЛАДР 4 (последние две цифры в коде)
        /// </summary>
        [XmlAttribute("CURRSTATUS")]
        public int KladrActualityStatus { get; set; }
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
        /// Внешний ключ на нормативный документ
        /// </summary>
        [XmlAttribute("NORMDOC")]
        public Guid DocumentId { get; set; }
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
