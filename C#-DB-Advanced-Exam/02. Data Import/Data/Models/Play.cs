using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Theatre.Data.Models.Enums;

namespace Theatre.Data.Models
{
    public class Play
    {
        public Play()
        {
            HashSet<Ticket> Tickets = new HashSet<Ticket>();  

            HashSet<Cast> Casts = new HashSet<Cast>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]

        public string Duration { get; set; }

        [Required]
        [Range(0.00,10.00)]
        public float Rating { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        [MaxLength(700)]
        public string Description { get; set; }

        [Required]
        [MaxLength(30)]
        public string Screenwriter  { get; set; }

        public HashSet<Cast> Casts { get; set; }

        public HashSet<Ticket> Tickets { get; set; }
    }
}
