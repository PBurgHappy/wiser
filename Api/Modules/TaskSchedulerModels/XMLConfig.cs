using System;
using System.Xml.Serialization;

namespace Api.Modules.TaskSchedulerModels;

[XmlRoot(ElementName = "Configuration")]
public class Configuration
{
    [XmlAttribute(AttributeName = "ServiceName")]
    public string ServiceName { get; set; }

    [XmlAttribute(AttributeName = "ConnectionString")]
    public string ConnectionString { get; set; }

    [XmlAttribute(AttributeName = "LogSettings")]
    public bool LogSettings { get; set; }

    [XmlElement(ElementName = "RunSchemes")]
    public RunScheme[] RunSchemes { get; set; }

    [XmlElement(ElementName = "Queries")]
    public Query[] Queries { get; set; }

    [XmlElement(ElementName = "HttpApis")]
    public HttpApi[] HttpApis { get; set; }
}

[XmlRoot(ElementName = "Runscheme")]
public class RunScheme
{
    [XmlAttribute(AttributeName = "Type")]
    public string Type { get; set; }

    [XmlAttribute(AttributeName = "TimeId")]
    public int TimeId { get; set; }

    [XmlAttribute(AttributeName = "DayOfWeek")]
    public DayOfWeek DayOfWeek { get; set; }

    [XmlAttribute(AttributeName = "Delay")]
    // public TimeOnly Delay { get; set; }
    public string Delay { get; set; }
    
    [XmlAttribute(AttributeName = "Hour")]
    // public TimeOnly Hour { get; set; }
    public string Hour { get; set; }
}


[XmlRoot(ElementName = "HttpApi")]
public class HttpApi
{
    [XmlAttribute(AttributeName = "TimeId")]
    public int TimeId { get; set; }

    [XmlAttribute(AttributeName = "Order")]
    public int Order { get; set; }

    [XmlAttribute(AttributeName = "ResultSetName")]
    public string ResultSetName { get; set; }

    [XmlElement(ElementName = "UseResultSet")]
    public ResultSet UseResultSet { get; set; }

    [XmlAttribute(AttributeName = "Url")]
    public string Url { get; set; }
    
    // Make another class so they're not strings? Might make for easier processing
    [XmlAttribute(AttributeName = "Method")]
    public string Method { get; set; }

    [XmlElement(ElementName = "Headers")]
    public Header[] Headers { get; set; }

    [XmlElement(ElementName = "Body")]
    public XMLBody Body { get; set; }

    [XmlAttribute(AttributeName = "SingleRequest")]
    public bool SingleRequest { get; set; }

    [XmlAttribute(AttributeName = "NextUrlProperty")]
    public string NextUrlProperty { get; set; }
}

[XmlRoot(ElementName = "GenerateFile")]
public class GenerateFile
{
    [XmlAttribute(AttributeName = "TimeId")]
    public int TimeId { get; set; }

    [XmlAttribute(AttributeName = "Order")]
    public int Order { get; set; }

    [XmlElement(ElementName = "UseResultSet")]
    public ResultSet UseResultSet { get; set; }

    [XmlAttribute(AttributeName = "FileLocation")]
    public string FileLocation { get; set; }

    [XmlAttribute(AttributeName = "FileName")]
    public string FileName { get; set; }

    [XmlAttribute(AttributeName = "SingleFile")]
    public bool SingleFile { get; set; }

    [XmlElement(ElementName = "Body")]
    public XMLBody Body { get; set; }
}

// ____________________________________________________-

[XmlRoot(ElementName = "Query")]
public class Query
{
    [XmlAttribute(AttributeName = "TimeId")]
    public int TimeId { get; set; }

    [XmlAttribute(AttributeName = "Order")]
    public int Order { get; set; }

    [XmlAttribute(AttributeName = "ResultSetName")]
    public string ResultSetName { get; set; }
    
    // Not sure how import works
    //      <UseResultSet>Products.Results</UseResultSet>
    //      <OnlyWithSuccessState>Products,True</OnlyWithSuccessState>
    [XmlElement(ElementName = "UseResultSet")]
    public ResultSet UseResultSet { get; set; }

    [XmlAttribute(AttributeName = "OnlyWithSuccessRate")]
    public string OnlyWithSuccessRate { get; set; }
    
    [XmlAttribute(AttributeName = "QueryString")]
    public string QueryString { get; set; }
}

[XmlRoot(ElementName = "Header")]
public class Header
{
    [XmlAttribute(AttributeName = "Name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "Value")]
    public string Value { get; set; }
}

[XmlRoot(ElementName = "Body")]
public class XMLBody
{
    [XmlAttribute(AttributeName = "ContentType")]
    public string ContentType { get; set; }

    [XmlElement(ElementName = "BodyParts")]
    public BodyPart[] Bodyparts { get; set; }
}

[XmlRoot(ElementName = "BodyPart")]
public class BodyPart
{
    [XmlAttribute(AttributeName = "Text")]
    public string Text { get; set; }

    [XmlAttribute(AttributeName = "SingleItem")]
    public bool SingleItem { get; set; }

    [XmlElement(ElementName = "UseResultSet")]
    public ResultSet UseResultSet { get; set; }

    [XmlAttribute(AttributeName = "ForceIndex")]
    public bool ForceIndex { get; set; }
}

// [XmlRoot(ElementName = "ResultSet")]
public class ResultSet
{
    [XmlAttribute(AttributeName = "Name")]
    public string Name { get; set; }
}