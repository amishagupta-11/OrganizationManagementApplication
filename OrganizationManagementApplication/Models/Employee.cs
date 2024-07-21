using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrganizationManagementApplication.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [JsonInclude]
        public string? FirstName { get; set; }

        [JsonInclude]
        public string? LastName { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime DateOfBirth { get; set; }

        [JsonInclude]
        [JsonProperty("email_address")]
        public string? Email { get; set; }
    }
}

