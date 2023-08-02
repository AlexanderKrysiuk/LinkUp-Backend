namespace LinkUp.Domain
{
    public class ValidationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }
    }
}
