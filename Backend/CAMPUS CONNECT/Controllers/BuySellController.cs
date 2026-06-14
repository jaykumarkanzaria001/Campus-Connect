//using Campus_Connect.Models;
using CAMPUS_CONNECT.DTOs;
using CAMPUS_CONNECT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Campus_Connect.Controllers
{
	[Route("api/buysell")]
	[ApiController]
	public class buy_sellAPIController : ControllerBase
	{
		private readonly IConfiguration _config;

		public buy_sellAPIController(IConfiguration config)
		{
			_config = config;
		}

		[Authorize(Roles = "Student")]
		[HttpPost("sell")]
		[RequestSizeLimit(10 * 1024 * 1024)] // 10MB max
		public IActionResult ReportItem([FromForm] BuySellRequestDto request)
		{
			if (request == null)
				return BadRequest("Invalid data");

			string image_path = null;

			// Save image_path in wwwroot/uploads/buysell/
			if (request.image_path != null && request.image_path.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "buysell");

				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.image_path.FileName);
				var fullPath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					request.image_path.CopyTo(stream);
				}

				image_path = $"uploads/buysell/{fileName}";
			}

			// Insert into SQL Server
			string query = @"INSERT INTO buy_sell 
                            (name, mobile_no, email, item_title, description, price, image_path) 
                            VALUES 
                            (@name, @mobile_no, @email, @item_title, @description, @price, @image_path)";

			using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("dbcs")))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@name", request.name ?? "");
					cmd.Parameters.AddWithValue("@mobile_no", request.mobile_no);
					cmd.Parameters.AddWithValue("@email", request.email ?? "");
					cmd.Parameters.AddWithValue("@item_title", request.item_title ?? "");
					cmd.Parameters.AddWithValue("@description", request.description ?? "");
					cmd.Parameters.AddWithValue("@price", request.price);
					cmd.Parameters.AddWithValue("@image_path", (object?)image_path ?? DBNull.Value);

					cmd.ExecuteNonQuery();
				}
			}

			return Ok(new
			{
				message = "Item reported successfully ✅",
				image_path = image_path
			});
		}
		// ==================================================
		// 2) FETCH ALL ITEMS (SQL)
		// GET: api/buysell
		// ==================================================
		[Authorize]
		[HttpGet]
		public IActionResult GetItems()
		{
			List<BuySell> items = new List<BuySell>();

			string query = "SELECT * FROM buy_sell ORDER BY product_id DESC";

			using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("dbcs")))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							items.Add(new BuySell
							{
								product_id = Convert.ToInt32(reader["product_id"]),
								name = reader["name"].ToString(),
								mobile_no = reader["mobile_no"].ToString(),
								email = reader["email"].ToString(),
								item_title = reader["item_title"].ToString(),
								description = reader["description"].ToString(),
								price = decimal.Parse(reader["price"].ToString()),
								image_path = reader["image_path"] == DBNull.Value ? null : reader["image_path"].ToString()
							});
						}
					}
				}
			}

			return Ok(items);
		}
	}
}

