using System;
using System.Collections.Generic;

namespace CAMPUS_CONNECT.Models;

public partial class BuySell
{
    public int product_id { get; set; }

    public string name { get; set; } = null!;

    public string mobile_no { get; set; } = null!;

    public string email { get; set; } = null!;

    public string item_title { get; set; } = null!;

    public string description { get; set; } = null!;

    public decimal price { get; set; }

    public string image_path { get; set; } = null!;
}
