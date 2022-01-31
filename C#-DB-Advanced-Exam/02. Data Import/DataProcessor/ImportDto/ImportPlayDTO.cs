using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPlayDTO
    {

        [XmlElement("Title")]
        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string Title { get; set; }

        [XmlElement("Duration")]
        [Required]
        public string Duration { get; set; }

        [XmlElement("Rating")]
        [Required]
        public float Rating { get; set; }

        [XmlElement("Genre")]
        [Required]
        public Genre Genre { get; set; }

        [XmlElement("Description")]
        [Required]
        public string Description { get; set; }

        [XmlElement("Screenwriter")]
        [Required]
        public string Screenwriter { get; set; }
    }
}
