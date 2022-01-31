namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Plays");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPlayDTO[]), xmlRoot);

            using StringReader sr = new StringReader(xmlString);

            ImportPlayDTO[] playDtos = (ImportPlayDTO[])
                xmlSerializer.Deserialize(sr);

            HashSet<Play> plays = new HashSet<Play>();

            foreach (ImportPlayDTO playDto in playDtos)
            {
                if (!IsValid(playDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                bool isDurationValid = TimeSpan.TryParseExact(playDto.Duration, "hours:minutes:seconds",
                    CultureInfo.InvariantCulture, TimeSpanStyles.None, out TimeSpan duration);

                bool isDurationEnough = TimeSpan.Parse(playDto.Duration) > TimeSpan.FromHours(1);

                if (!isDurationValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!isDurationEnough)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                Play p = new Play()
                {
                    Title = playDto.Title,
                    Duration = playDto.Duration,
                    Rating = playDto.Rating,
                    Genre = playDto.Genre,
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter
                };

                plays.Add(p);

                sb.AppendLine(String.Format(SuccessfulImportPlay, p.Title, p.Genre, p.Rating));
            }

            context.Plays.AddRange(plays);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Casts");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCastDTO[]), xmlRoot);

            using StringReader sr = new StringReader(xmlString);

            ImportCastDTO[] castDtos = (ImportCastDTO[])
                xmlSerializer.Deserialize(sr);

            HashSet<Cast> casts = new HashSet<Cast>();

            foreach (ImportCastDTO castDTO in castDtos)
            {
                if (!IsValid(castDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                string Character = "";

                if (castDTO.IsMainCharacter)
                {
                    Character = "main";
                }
                else
                {
                    Character = "lesser";
                }

                Cast c = new Cast()
                {
                    FullName = castDTO.FullName,
                    IsMainCharacter = castDTO.IsMainCharacter,
                    PhoneNumber = castDTO.PhoneNumber,
                    PlayId = castDTO.PlayId
                };

                casts.Add(c);

                sb.AppendLine(String.Format(SuccessfulImportActor, c.FullName, Character));
            }

            context.Casts.AddRange(casts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportTheatreDTO[] theaterDTOs = JsonConvert.DeserializeObject<ImportTheatreDTO[]>(jsonString);

            HashSet<Theatre> validTheaters = new HashSet<Theatre>();
            foreach (ImportTheatreDTO theatre in theaterDTOs)
            {
                if (!IsValid(theatre))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre t = new Theatre()
                {
                    Name = theatre.Name,
                    NumberOfHalls = theatre.NumberOfHalls,
                    Director = theatre.Director
                };

                HashSet<Ticket> theatreTickets = new HashSet<Ticket>();
                foreach (ImportTicketDTO ticket in theatre.Tickets)
                {
                    if (!IsValid(ticket))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ti = new Ticket()
                    {
                        Price = ticket.Price,
                        RowNumber = ticket.RowNumber,
                        PlayId = ticket.PlayId
                    };

                    theatreTickets.Add(ti);
                }

                t.Tickets = theatreTickets;

                validTheaters.Add(t);

                sb.AppendLine(String.Format(SuccessfulImportTheatre, t.Name, theatreTickets.Count));
            }

            context.Theatres.AddRange(validTheaters);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
