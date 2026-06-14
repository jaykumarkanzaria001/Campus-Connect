using CAMPUS_CONNECT.DTOs;
using CAMPUS_CONNECT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Campus_Connect.Controllers
{
	[Route("api/lostfound")]
	[ApiController]
	public class lost_foundAPIController : ControllerBase
	{
		private readonly IConfiguration _config;

		public lost_foundAPIController(IConfiguration config)
		{
			_config = config;
		}

		[Authorize(Roles = "Student")]
		[HttpPost("report")]
		[RequestSizeLimit(10 * 1024 * 1024)] // 10MB 
		public IActionResult ReportItem([FromForm] LostFoundRequestDto request)
		{
			if (request == null)
				return BadRequest("Invalid data");

			string image_path = null;

			// Save Image in wwwroot/uploads/lostfound/
			if (request.image_path != null && request.image_path.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "lostfound");

				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.image_path.FileName);
				var fullPath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					request.image_path.CopyTo(stream);
				}

				image_path = $"/uploads/lostfound/{fileName}";
			}

			// Insert into SQL Server
			string query = @"INSERT INTO lost_found 
                            (item_name, category, description, datetime, location, status, contact_information, image_path) 
                            VALUES 
                            (@item_name, @category, @description, @datetime, @location, @status, @contact_information, @image_path)";

			using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("dbcs")))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@item_name", request.item_name ?? "");
					cmd.Parameters.AddWithValue("@category", request.category ?? "");
					cmd.Parameters.AddWithValue("@description", request.description ?? "");
					cmd.Parameters.AddWithValue("@datetime", DateTime.Now);
					cmd.Parameters.AddWithValue("@location", request.location ?? "");
					cmd.Parameters.AddWithValue("@status", request.status ?? "");
					cmd.Parameters.AddWithValue("@contact_information", request.contact_information ?? "");
					cmd.Parameters.AddWithValue("@image_path", (object?)image_path ?? DBNull.Value);

					cmd.ExecuteNonQuery();
				}
			}

			return Ok(new
			{
				message = "Item reported successfully ✅",
				imagePath = image_path
			});
		}
		// ==================================================
		// 2) FETCH ALL ITEMS (SQL)
		// GET: api/lostfound
		// ==================================================
		[Authorize]
		[HttpGet]
		public IActionResult GetItems()
		{
			List<LostFound> items = new List<LostFound>();

			string query = "SELECT * FROM lost_found ORDER BY item_id DESC";

			using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("dbcs")))
			{
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							items.Add(new LostFound
							{
								item_name = reader["item_name"].ToString(),
								category = reader["Category"].ToString(),
								description = reader["Description"].ToString(),
								datetime = Convert.ToDateTime(reader["datetime"]),
								location = reader["Location"].ToString(),
								status = reader["Status"].ToString().Trim(),
								contact_information = reader["Contact_information"].ToString(),
								image_path = reader["Image_Path"] == DBNull.Value ? null : reader["image_path"].ToString()
							});
						}
					}
				}
			}

			return Ok(items);
		}
	}
}
