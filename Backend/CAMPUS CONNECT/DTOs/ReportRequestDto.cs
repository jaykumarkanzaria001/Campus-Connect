namespace CAMPUS_CONNECT.DTOs
{
    public class ReportRequestDto
    {
        public string response_type { get; set; } = null!;

        public string location { get; set; } = null!;

        public string? description { get; set; }

        public string status { get; set; } = null!;

        public string contact_email { get; set; } = null!;

        public IFormFile? image_path { get; set; } = null!;
    }
}
