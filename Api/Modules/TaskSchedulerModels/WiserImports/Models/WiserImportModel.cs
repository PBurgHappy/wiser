using System.Xml.Serialization;

namespace Api.Modules.TaskSchedulerModels.WiserImports.Models;

[XmlType("WiserImport")]
public class WiserImportModel : ActionModel
{
    /// <summary>
    /// Gets or sets the ID of the mail template to get information from.
    /// </summary>
    public ulong TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the connection string, if left null the connection string set in the configuration will be used.
    /// </summary>
    public string ConnectionString { get; set; }
}