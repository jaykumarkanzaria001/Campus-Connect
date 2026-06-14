namespace CAMPUS_CONNECT.Models
{

    public partial class LostFound
    {
        public int item_id { get; set; }

        public string item_name { get; set; } = null!;

        public string category { get; set; } = null!;

        public string description { get; set; } = null!;

        public DateTime datetime { get; set; }

        public string location { get; set; } = null!;

        public string status { get; set; } = null!;

        public string contact_information { get; set; } = null!;

        public string image_path { get; set; } = null!;
    }
}

