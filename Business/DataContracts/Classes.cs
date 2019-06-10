using System.Xml.Serialization;

namespace SagicorNow.Business.DataContracts
{

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ACORD.org/Standards/Life/2")]
    [XmlRoot("TXLife", Namespace = "http://ACORD.org/Standards/Life/2")]
    public class TXLife
    {
        public TXLife()
        {
            Xmlns = new XmlSerializerNamespaces();
            Xmlns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            Xmlns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        }

        [XmlElement]
        public TXLifeRequest[] TXLifeRequest { get; set; }

        [XmlNamespaceDeclarations] public XmlSerializerNamespaces Xmlns;
    }

    public class OLifEExtension
    {
        [XmlElement]
        public System.Xml.XmlElement[] Any { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string VendorCode { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ExtensionCode { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NMTOKEN")]
        public string DataRep { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SystemCode { get; set; }
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ACORD.org/Standards/Life/2")]
    public class TXLifeRequest
    {
        public string TransRefGUID { get; set; }
        public TransType TransType { get; set; }
        public IllustrationRequest IllustrationRequest { get; set; }
        public OLifE OLifE { get; set; }
    }

    public class TransType : ObjectBase
    {
    }

    public class IllustrationRequest
    {
        [XmlElement]
        public RequestBasis[] RequestBasis { get; set; }
    }

    public class RequestBasis
    {
        public InterestAssumption InterestAssumption { get; set; }
        public MortalityAssumption MortalityAssumption { get; set; }
        [XmlElement]
        public VectorRequest[] VectorRequest { get; set; }
    }

    public class VectorRequest
    {
        public VectorSetType VectorSetType { get; set; }
        [XmlElement]
        public VectorItem[] VectorItem { get; set; }
    }

    public class OLifE
    {
        public SourceInfo SourceInfo { get; set; }
        [XmlElement]
        public Holding[] Holding { get; set; }
        [XmlElement]
        public Party[] Party { get; set; }
        [XmlElement]
        public Relation[] Relation { get; set; }
    }

    public class SourceInfo
    {
        public string SourceInfoName { get; set; }
        public string SourceInfoDescription { get; set; }
        public string FileControlID { get; set; }
    }

    public class Holding
    {
        [System.Xml.Serialization.XmlAttributeAttribute]
        public string id { get; set; }
        public CurrencyTypeCode CurrencyTypeCode { get; set; }
        public HoldingTypeCode HoldingTypeCode { get; set; }
        public Policy Policy { get; set; }
    }

    public class Policy
    {
        public LineOfBusiness LineOfBusiness { get; set; }
        public string ProductCode { get; set; }
        public IssueNation IssueNation { get; set; }
        public string CarrierCode { get; set; }
        public Jurisdiction Jurisdiction { get; set; }
        public Life Life { get; set; }
        public ApplicationInfo ApplicationInfo { get; set; }
    }

    public class Life
    {
        [XmlElement]
        public Coverage[] Coverage { get; set; }
    }

    public class Party
    {
        [System.Xml.Serialization.XmlAttributeAttribute]
        public string id { get; set; }
        public Person Person { get; set; }
        public PartyTypeCode PartyTypeCode { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string BirthDate { get; set; }
        public int Age { get; set; }
    }

    public class Relation
    {
        [System.Xml.Serialization.XmlAttributeAttribute]
        public string id { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute]
        public string OriginatingObjectID { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute]
        public string RelatedObjectID { get; set; }

        public RelationRoleCode RelationRoleCode { get; set; }
    }

    public class Coverage
    {
        [System.Xml.Serialization.XmlAttributeAttribute]
        public string id { get; set; }
        public IndicatorCode IndicatorCode { get; set; }
        public LifeCovTypeCode LifeCovTypeCode { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public decimal CurrentAmt { get; set; }
        public int NumChildren { get;  set; }
        public string LevelPremiumPeriod { get; set; }
        public decimal ModalPremAmt { get; set; }
        [XmlElement]
        public LifeParticipant[] LifeParticipant { get; set; }

    }

    public class LifeCovTypeCode:ObjectBase
    {
    }

    public class VectorItem
    {
        public VectorType VectorType { get; set; }
    }

    public class InterestAssumption : ObjectBase { }

    public class VectorType : ObjectBase
    {

    }

    public class MortalityAssumption : ObjectBase { }

    public class PartyTypeCode : ObjectBase
    {

    }

    public class LifeParticipantRoleCode : ObjectBase
    {

    }

    public class HoldingTypeCode : ObjectBase
    {

    }

    public class IndicatorCode : ObjectBase
    {

    }


    public class PaymentMode : ObjectBase
    {

    }


    public class SmokerStat : ObjectBase
    {

    }


    public class PermTableRating : ObjectBase
    {

    }

    public class UnderwritingClass : ObjectBase
    {

    }

    public class VectorSetType : ObjectBase
    {

    }

    public class LineOfBusiness : ObjectBase
    {

    }

    public class CurrencyTypeCode : ObjectBase
    {

    }

    public class IssueNation : ObjectBase
    {

    }

    public class Jurisdiction : ObjectBase
    {

    }

    public class Gender : ObjectBase
    {

    }

    public class RelationRoleCode : ObjectBase
    {

    }

    public class QuotedPremiumMode : ObjectBase
    {

    }

    public class Operation
    {
    }

    public class LifeParticipant
    {
        [System.Xml.Serialization.XmlAttributeAttribute]
        public string PartyID { get; set; }
        public LifeParticipantRoleCode LifeParticipantRoleCode { get; set; }
        public SmokerStat SmokerStat { get; set; }
        public decimal TempFlatExtraAmt { get; set; }
        public PermTableRating PermTableRating { get; set; }
        public UnderwritingClass UnderwritingClass { get; set; }
        public string TempFlatExtraDuration { get; set; }
    }

    public class ApplicationInfo
    {
        public string RequestedIssueDate { get; set; }
        public double QuotedPremiumAmt { get; set; }
        public QuotedPremiumMode QuotedPremiumMode { get; set; }
    }

    public class ObjectBase
    {
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "nonNegativeInteger")]
        public string tc { get; set; }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value { get; set; }
    }
}