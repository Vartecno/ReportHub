namespace ReportHub.Objects.DTOs
{
    public class FieldSchema
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public object DefaultValue { get; set; }
        public List<string> AllowedValues { get; set; }
        public string ValidationPattern { get; set; }
    }
}