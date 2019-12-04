using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ALo.Addresses.FiasUpdater.Fias.Models
{
    [XmlRoot("AddressObjects")]
    public class Addresses
    {
        [XmlElement(ElementName = "Object")]
        public List<AddressObject> Objects { get; set; }
    }

    [XmlRoot(ElementName = "Object")]
    public class AddressObject
    {
        [XmlAttribute("AOID")]
        public Guid Id { get; set; }
        [XmlAttribute("AOGUID")]
        public Guid GlobalId { get; set; }
        [XmlAttribute("FORMALNAME")]
        public string FormalName { get; set; }
        [XmlAttribute("REGIONCODE")]
        public string RegionCode { get; set; }
        [XmlAttribute("AUTOCODE")]
        public string AutonomyCode { get; set; }
        [XmlAttribute("AREACODE")]
        public string AreaCode { get; set; }
        [XmlAttribute("CITYCODE")]
        public string CityCode { get; set; }
        [XmlAttribute("CTARCODE")]
        public string DistrictCode { get; set; }
        [XmlAttribute("PLACECODE")]
        public string PlaceCode { get; set; }
        [XmlAttribute("PLANCODE")]
        public string PlanningStructureCode { get; set; }
        [XmlAttribute("STREETCODE")]
        public string StreetCode { get; set; }
        [XmlAttribute("EXTRCODE")]
        public string ExtraCode { get; set; }
        [XmlAttribute("SEXTCODE")]
        public string SubExtraCode { get; set; }
        [XmlAttribute("OFFNAME")]
        public string OfficialName { get; set; }
        [XmlAttribute("POSTALCODE")]
        public string PostalCode { get; set; }
        [XmlAttribute("IFNSFL")]
        public string IfnsPhysicalPersonsCode { get; set; }
        [XmlAttribute("TERRIFNSFL")]
        public string TerritorialIfnsPhysicalPersonsCode { get; set; }
        [XmlAttribute("IFNSUL")]
        public string IfnsJuridicalPersonsCode { get; set; }
        [XmlAttribute("TERRIFNSUL")]
        public string TerritorialIfnsJuridicalPersonsCode { get; set; }
        [XmlAttribute("OKATO")]
        public string OKATO { get; set; }
        [XmlAttribute("OKTMO")]
        public string OKTMO { get; set; }
        [XmlAttribute("UPDATEDATE")]
        public DateTime UpdateDate { get; set; }
        [XmlAttribute("SHORTNAME")]
        public string ShortName { get; set; }
        [XmlAttribute("AOLEVEL")]
        public int Level { get; set; }
        [XmlAttribute("PARENTGUID")]
        public Guid ParentId { get; set; }
        [XmlAttribute("PREVID")]
        public Guid PreviousId { get; set; }
        [XmlAttribute("NEXTID")]
        public Guid NextId { get; set; }
        [XmlAttribute("CODE")]
        public string KladrCodeWithActuality { get; set; }
        [XmlAttribute("PLAINCODE")]
        public string KladrCode { get; set; }
        [XmlAttribute("ACTSTATUS")]
        public int FiasActuality { get; set; }
        [XmlAttribute("CENTSTATUS")]
        public int CenterStatus { get; set; }
        [XmlAttribute("OPERSTATUS")]
        public int OperationStatus { get; set; }
        [XmlAttribute("CURRSTATUS")]
        public int KladrActuality { get; set; }
        [XmlAttribute("STARTDATE")]
        public DateTime StartDate { get; set; }
        [XmlAttribute("ENDDATE")]
        public DateTime EndDate { get; set; }
        [XmlAttribute("NORMDOC")]
        public Guid NormativeDocumentId { get; set; }
        [XmlAttribute("LIVESTATUS")]
        public int LiveStatus { get; set; }
        [XmlAttribute("CADNUM")]
        public string CadastrNumber { get; set; }
        [XmlAttribute("DIVTYPE")]
        public int DiviationType { get; set; }
    }
}
