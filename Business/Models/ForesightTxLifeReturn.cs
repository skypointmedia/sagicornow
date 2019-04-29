using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SagicorNow.Business.Models
{
    [XmlRoot("TXLife")]
    public class ForesightTxLifeReturn
    {
        [XmlElement("TXLifeResponse")]
        public ForesightTxLifeResponse[] TxLifeResponse { get; set; }
    }

    public class ForesightTxLifeResponse
    {
        [XmlElement("IllustrationResult")]
        public ForesightIllustrationResult IllustrationResult { get; set; }
    }

    public class ForesightIllustrationResult
    {
        public ForesightResultBasis ResultBasis { get; set; }
    }

    public class ForesightResultBasis
    {
        public ForesightVector Vector { get; set; }
    }

    public class ForesightVector
    {
        [XmlElement(DataType = "decimal", Type = typeof(decimal))]
        public decimal[] V { get; set; }
    }
}