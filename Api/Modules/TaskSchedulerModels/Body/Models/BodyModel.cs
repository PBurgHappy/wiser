﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Api.Modules.TaskSchedulerModels.Body.Models
{
    /// <summary>
    /// A model to add a body.
    /// </summary>
    [XmlType("Body")]
    public class BodyModel
    {
        /// <summary>
        /// Gets or sets the type of the content of the body.
        /// </summary>
        [Required]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the body parts.
        /// </summary>
        [Required]
        [XmlArray("BodyParts")]
        [XmlArrayItem(typeof(BodyPartModel))]
        public BodyPartModel[] BodyParts { get; set; }
    }
}
