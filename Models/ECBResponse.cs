using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CurrencyExchangeAPI.Models
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
    public class Envelope
    {
        [XmlElement("subject", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
        public string Subject { get; set; }

        [XmlElement("Sender", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
        public Sender Sender { get; set; }

        [XmlElement("Cube", Namespace = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")]
        public Cube RootCube { get; set; }
    }

    public class Sender
    {
        [XmlElement("name", Namespace = "http://www.gesmes.org/xml/2002-08-01")]
        public string Name { get; set; }
    }

    public class Cube
    {
        [XmlElement("Cube")]
        public List<TimeCube> TimeCubes { get; set; }
    }

    public class TimeCube
    {
        [XmlAttribute("time")]
        public string Time { get; set; }

        [XmlElement("Cube")]
        public List<CurrencyRate> CurrencyRates { get; set; }
    }

    public class CurrencyRate
    {
        [XmlAttribute("currency")]
        public string Currency { get; set; }

        [XmlAttribute("rate")]
        public decimal Rate { get; set; }
    }
}