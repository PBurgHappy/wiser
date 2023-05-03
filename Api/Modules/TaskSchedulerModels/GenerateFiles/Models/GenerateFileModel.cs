﻿using System.Xml.Serialization;
using Api.Modules.TaskSchedulerModels.Body.Models;

namespace Api.Modules.TaskSchedulerModels.GenerateFiles.Models
{
    [XmlType("GenerateFile")]
    public class GenerateFileModel : ActionModel
    {
        /// <summary>
        /// Gets or sets the location where the file need to be saved to.
        /// </summary>
        public string FileLocation { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets if a single file needs to be generated.
        /// </summary>
        public bool SingleFile { get; set; } = true;

        /// <summary>
        /// Gets or sets the body to save in the file.
        /// </summary>
        public BodyModel Body { get; set; }
    }
}