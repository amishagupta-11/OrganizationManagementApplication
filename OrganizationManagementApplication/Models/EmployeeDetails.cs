using System.Text.Json.Serialization;

namespace OrganizationManagementApplication.Models
{
    public class EmployeeDetails
    {
        public int Id { get; set; }

        [JsonInclude]
        public string? FullName { get; set; }

        [JsonInclude]
        public string? Email { get; set; }
    }
}
