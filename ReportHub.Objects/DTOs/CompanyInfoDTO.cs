using System.ComponentModel.DataAnnotations;

namespace ReportHub.Objects.DTOs
{
    public class CompanyInfoDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalInfo { get; set; } = new();
    }
}