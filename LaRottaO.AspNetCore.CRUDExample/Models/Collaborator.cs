using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace LaRottaO.AspNetCore.CRUDExample.Models
{
    public class Collaborator
    {
        [SwaggerIgnore]
        public int Id { get; set; }

        [Required]
        public string PassportNumber { get; set; } = "";

        [Required]
        public string FullName { get; set; } = "";

        [SwaggerIgnore]
        public DateTime EntryCreationDate { get; set; }

        public List<CollaboratorData> CollaboratorDataEntries { get; set; } = new List<CollaboratorData>();

        public static Collaborator CreateEmpty()
        {
            return new Collaborator
            {
                PassportNumber = "N/A",
                FullName = "Unknown"
            };
        }
    }
}