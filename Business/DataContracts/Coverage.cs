using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SagicorNow.Foresight.DTO
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ACORD.org/Standards/Life/2")]
    public class Coverage : object, System.ComponentModel.INotifyPropertyChanged
    {

        private Jurisdiction jurisdictionField;

        private LifeCovTypeCode lifeCovTypeCodeField;

        private IndicatorCode indicatorCodeField;

        private string durationDesignField;

        private System.DateTime expiryDateField;

        private bool expiryDateFieldSpecified;

        private DeathBenefitOptType deathBenefitOptTypeField;

        private PaymentMode paymentModeField;

        private decimal cashValueField;

        private bool cashValueFieldSpecified;

        private double currentAmtField;

        private bool currentAmtFieldSpecified;

        private decimal deathBenefitAmtField;

        private bool deathBenefitAmtFieldSpecified;

        private decimal initCovAmtField;

        private bool initCovAmtFieldSpecified;

        private double currentNumberOfUnitsField;

        private bool currentNumberOfUnitsFieldSpecified;

        private decimal modalPremAmtField;

        private bool modalPremAmtFieldSpecified;

        private decimal totAnnualPremAmtField;

        private bool totAnnualPremAmtFieldSpecified;

        private System.DateTime effDateField;

        private bool effDateFieldSpecified;

        private string levelPremiumPeriodField;

        private CovOption[] covOptionField;

        private LifeParticipant[] lifeParticipantField;

        private string durationField;

        private KeyedValue[] keyedValueField;

        private string numChildrenField;

        private OLifEExtension[] oLifEExtensionField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        public Jurisdiction Jurisdiction
        {
            get {
                return this.jurisdictionField;
            }
            set {
                this.jurisdictionField = value;
                this.RaisePropertyChanged("Jurisdiction");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public LifeCovTypeCode LifeCovTypeCode
        {
            get {
                return this.lifeCovTypeCodeField;
            }
            set {
                this.lifeCovTypeCodeField = value;
                this.RaisePropertyChanged("LifeCovTypeCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public IndicatorCode IndicatorCode
        {
            get {
                return this.indicatorCodeField;
            }
            set {
                this.indicatorCodeField = value;
                this.RaisePropertyChanged("IndicatorCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 3)]
        public string DurationDesign
        {
            get {
                return this.durationDesignField;
            }
            set {
                this.durationDesignField = value;
                this.RaisePropertyChanged("DurationDesign");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", Order = 4)]
        public System.DateTime ExpiryDate
        {
            get {
                return this.expiryDateField;
            }
            set {
                this.expiryDateField = value;
                this.RaisePropertyChanged("ExpiryDate");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ExpiryDateSpecified
        {
            get {
                return this.expiryDateFieldSpecified;
            }
            set {
                this.expiryDateFieldSpecified = value;
                this.RaisePropertyChanged("ExpiryDateSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        public DeathBenefitOptType DeathBenefitOptType
        {
            get {
                return this.deathBenefitOptTypeField;
            }
            set {
                this.deathBenefitOptTypeField = value;
                this.RaisePropertyChanged("DeathBenefitOptType");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        public PaymentMode PaymentMode
        {
            get {
                return this.paymentModeField;
            }
            set {
                this.paymentModeField = value;
                this.RaisePropertyChanged("PaymentMode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 7)]
        public decimal CashValue
        {
            get {
                return this.cashValueField;
            }
            set {
                this.cashValueField = value;
                this.RaisePropertyChanged("CashValue");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CashValueSpecified
        {
            get {
                return this.cashValueFieldSpecified;
            }
            set {
                this.cashValueFieldSpecified = value;
                this.RaisePropertyChanged("CashValueSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 8)]
        public double CurrentAmt
        {
            get {
                return this.currentAmtField;
            }
            set {
                this.currentAmtField = value;
                this.RaisePropertyChanged("CurrentAmt");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CurrentAmtSpecified
        {
            get {
                return this.currentAmtFieldSpecified;
            }
            set {
                this.currentAmtFieldSpecified = value;
                this.RaisePropertyChanged("CurrentAmtSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 9)]
        public decimal DeathBenefitAmt
        {
            get {
                return this.deathBenefitAmtField;
            }
            set {
                this.deathBenefitAmtField = value;
                this.RaisePropertyChanged("DeathBenefitAmt");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DeathBenefitAmtSpecified
        {
            get {
                return this.deathBenefitAmtFieldSpecified;
            }
            set {
                this.deathBenefitAmtFieldSpecified = value;
                this.RaisePropertyChanged("DeathBenefitAmtSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 10)]
        public decimal InitCovAmt
        {
            get {
                return this.initCovAmtField;
            }
            set {
                this.initCovAmtField = value;
                this.RaisePropertyChanged("InitCovAmt");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool InitCovAmtSpecified
        {
            get {
                return this.initCovAmtFieldSpecified;
            }
            set {
                this.initCovAmtFieldSpecified = value;
                this.RaisePropertyChanged("InitCovAmtSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 11)]
        public double CurrentNumberOfUnits
        {
            get {
                return this.currentNumberOfUnitsField;
            }
            set {
                this.currentNumberOfUnitsField = value;
                this.RaisePropertyChanged("CurrentNumberOfUnits");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CurrentNumberOfUnitsSpecified
        {
            get {
                return this.currentNumberOfUnitsFieldSpecified;
            }
            set {
                this.currentNumberOfUnitsFieldSpecified = value;
                this.RaisePropertyChanged("CurrentNumberOfUnitsSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 12)]
        public decimal ModalPremAmt
        {
            get {
                return this.modalPremAmtField;
            }
            set {
                this.modalPremAmtField = value;
                this.RaisePropertyChanged("ModalPremAmt");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ModalPremAmtSpecified
        {
            get {
                return this.modalPremAmtFieldSpecified;
            }
            set {
                this.modalPremAmtFieldSpecified = value;
                this.RaisePropertyChanged("ModalPremAmtSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 13)]
        public decimal TotAnnualPremAmt
        {
            get {
                return this.totAnnualPremAmtField;
            }
            set {
                this.totAnnualPremAmtField = value;
                this.RaisePropertyChanged("TotAnnualPremAmt");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TotAnnualPremAmtSpecified
        {
            get {
                return this.totAnnualPremAmtFieldSpecified;
            }
            set {
                this.totAnnualPremAmtFieldSpecified = value;
                this.RaisePropertyChanged("TotAnnualPremAmtSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", Order = 14)]
        public System.DateTime EffDate
        {
            get {
                return this.effDateField;
            }
            set {
                this.effDateField = value;
                this.RaisePropertyChanged("EffDate");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool EffDateSpecified
        {
            get {
                return this.effDateFieldSpecified;
            }
            set {
                this.effDateFieldSpecified = value;
                this.RaisePropertyChanged("EffDateSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 15)]
        public string LevelPremiumPeriod
        {
            get {
                return this.levelPremiumPeriodField;
            }
            set {
                this.levelPremiumPeriodField = value;
                this.RaisePropertyChanged("LevelPremiumPeriod");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CovOption", Order = 16)]
        public CovOption[] CovOption
        {
            get {
                return this.covOptionField;
            }
            set {
                this.covOptionField = value;
                this.RaisePropertyChanged("CovOption");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("LifeParticipant", Order = 17)]
        public LifeParticipant[] LifeParticipant
        {
            get {
                return this.lifeParticipantField;
            }
            set {
                this.lifeParticipantField = value;
                this.RaisePropertyChanged("LifeParticipant");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 18)]
        public string Duration
        {
            get {
                return this.durationField;
            }
            set {
                this.durationField = value;
                this.RaisePropertyChanged("Duration");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("KeyedValue", Order = 19)]
        public KeyedValue[] KeyedValue
        {
            get {
                return this.keyedValueField;
            }
            set {
                this.keyedValueField = value;
                this.RaisePropertyChanged("KeyedValue");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 20)]
        public string NumChildren
        {
            get {
                return this.numChildrenField;
            }
            set {
                this.numChildrenField = value;
                this.RaisePropertyChanged("NumChildren");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OLifEExtension", Order = 21)]
        public OLifEExtension[] OLifEExtension
        {
            get {
                return this.oLifEExtensionField;
            }
            set {
                this.oLifEExtensionField = value;
                this.RaisePropertyChanged("OLifEExtension");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string id
        {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
                this.RaisePropertyChanged("id");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}