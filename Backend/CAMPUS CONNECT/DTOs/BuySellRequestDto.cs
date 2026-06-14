namespace CAMPUS_CONNECT.DTOs
{
    public class BuySellRequestDto
    {
        public string name { get; set; } = null!;

        public string mobile_no { get; set; }

        public string email { get; set; } = null!;

        public string item_title { get; set; } = null!;

        public string? description { get; set; }

        public string price { get; set; }

        public IFormFile? image_path { get; set; } = null!;
    }
}
