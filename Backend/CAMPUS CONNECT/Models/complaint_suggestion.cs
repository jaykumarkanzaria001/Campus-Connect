using System.ComponentModel.DataAnnotations;

namespace CAMPUS_CONNECT.Models;

public partial class complaint_suggestion
{
    [Key]
    public int report_id { get; set; }

    public string response_type { get; set; } = null!;

    public string location { get; set; } = null!;

    public string description { get; set; } = null!;

    public string status { get; set; } = null!;

    public string contact_email { get; set; } = null!;

    public string image_path { get; set; } = null!;
}
