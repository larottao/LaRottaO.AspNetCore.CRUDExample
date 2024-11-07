using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace LaRottaO.AspNetCore.CRUDExample.Models
{
    public class CollaboratorData
    {
        [SwaggerIgnore]
        public int Id { get; set; }

        [Required]
        public String PassportNumber { get; set; } = "";

        [Required]
        public DateTime? OverTimeStart { get; set; }

        [Required]
        public DateTime? OverTimeEnd { get; set; }

        [Required]
        public String ActivitySummary { get; set; } = "";

        public bool HadBreakfast { get; set; }
        public bool HadLunch { get; set; }
        public bool HadDinner { get; set; }

        [SwaggerIgnore]
        public DateTime? EntryCreationDate { get; set; }

        public static CollaboratorData CreateEmpty()
        {
            return new CollaboratorData
            {
                PassportNumber = "N/A",
                OverTimeStart = null,
                OverTimeEnd = null,
                ActivitySummary = "N/A",
                HadBreakfast = false,
                HadLunch = false,
                HadDinner = false,
                EntryCreationDate = null
            };
        }
    }
}