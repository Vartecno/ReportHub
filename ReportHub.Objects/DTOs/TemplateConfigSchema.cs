namespace ReportHub.Objects.DTOs
{
    public class TemplateConfigSchema
    {
        public string TemplateId { get; set; } = string.Empty;
        public Dictionary<string, FieldSchema> RequiredFields { get; set; } = new();
        public Dictionary<string, FieldSchema> OptionalFields { get; set; } = new();
        public Dictionary<string, object> DefaultValues { get; set; } = new();
        public List<string> SupportedCurrencies { get; set; } = new();
        public List<string> SupportedLanguages { get; set; } = new();
    }
}