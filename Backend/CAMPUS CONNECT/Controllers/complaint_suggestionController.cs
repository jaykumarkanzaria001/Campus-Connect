using CAMPUS_CONNECT.DTOs;
using CAMPUS_CONNECT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Campus_Connect.Controllers
{
    [Route("api/complaintsuggestion")]
    [ApiController]
    public class complaint_suggestionAPIController : ControllerBase
    {
        private readonly IConfiguration _config;

        public complaint_suggestionAPIController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(Roles = "Student")]
        [HttpPost("report")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB max
		
		public IActionResult ReportItem([FromForm] ReportRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid data");

            string image_path = null;

            // ✅ Save Image in wwwroot/uploads/complaintsuggestion/
            if (request.image_path != null && request.image_path.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "complaintsuggestion");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.image_path.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    request.image_path.CopyTo(stream);
                }

				image_path = $"/uploads/complaintsuggestion/{fileName}";
            }

			// ==================================================
			// 1) INSERT ALL ITEMS (SQL)
			// ==================================================
			string query = @"INSERT INTO complaint_suggestion
                            (response_type, location, description, status, contact_email, image_path) 
                            VALUES 
                            (@Rt, @location, @description, @status, @contact_email, @image_path)";

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("dbcs")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Rt", request.response_type ?? "");
                    cmd.Parameters.AddWithValue("@location", request.location ?? "");
                    cmd.Parameters.AddWithValue("@description", request.description ?? "");
                    cmd.Parameters.AddWithValue("@status", request.status ?? "");
                    cmd.Parameters.AddWithValue("@contact_email", request.contact_email ?? "");
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
        // GET: api/complaintsuggestion
        // ==================================================
        [Authorize]
        [HttpGet]
		public IActionResult GetItems()
        {
            List<complaint_suggestion> items = new List<complaint_suggestion>();

            string query = "SELECT * FROM complaint_suggestion ORDER BY report_id DESC";

            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("dbcs")))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new complaint_suggestion
                            {
								report_id = Convert.ToInt32(reader["report_id"]),
								response_type = reader["response_type"].ToString(),
                                location = reader["location"].ToString(),
                                description = reader["description"].ToString(),
                                status = reader["Status"].ToString().Trim(),
								contact_email = reader["contact_email"].ToString(),
                                image_path = $"{Request.Scheme}://{Request.Host}/{reader["image_path"]}"
                                //image_path = reader["image_path"] == DBNull.Value ? null : reader["image_path"].ToString()
                            });
                        }
                    }
                }
            }

            return Ok(items);
        }



		// ==================================================
		// 3) UPDATE STATUS(SQL
		// ==================================================
		[Authorize(Roles = "Admin")]
		[HttpPut("status")]
		public async Task<IActionResult> UpdateStatus(int id, string status)
		{
			string query = "UPDATE complaint_suggestion SET status=@status WHERE report_id=@id";

			using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("dbcs")))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@status", status);
					cmd.Parameters.AddWithValue("@id", id);

					int rows = cmd.ExecuteNonQuery();

					if (rows == 0)
						return NotFound();
				}
			}

			return Ok("Database Updated");
		}
	}
}