using CAMPUS_CONNECT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Campus_Connect.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class userAPIController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly CampusConnectContext _db;

		public userAPIController(IConfiguration configuration, CampusConnectContext db)
		{
			_configuration = configuration;
			_db = db;
		}
		[HttpPost("signin")]
		public IActionResult SignIn([FromBody] User loginDto)
		{
			if (loginDto == null)
				return BadRequest("Request is empty");

			// AUTHENTICATION (DATABASE)
			var foundUser = _db.Users.FirstOrDefault(x =>
				x.user_name == loginDto.user_name &&
				x.password == loginDto.password
			);

			if (foundUser == null)
				return Unauthorized("Username or Password is wrong");

			var dbrole = foundUser.role?.Trim();
			var inputrole = loginDto.role?.Trim();

			if (!string.Equals(dbrole, inputrole, StringComparison.OrdinalIgnoreCase))
				return Unauthorized("Role is incorrect");

			// CLAIMS (ROLE FROM DATABASE)
			var userClaims = new[]
			{
				new Claim(ClaimTypes.Name, foundUser.user_name),
				new Claim(ClaimTypes.Role, foundUser.role)
			};

			var jwtKey = _configuration["Jwt:Key"];

			if (string.IsNullOrEmpty(jwtKey))
				return BadRequest("JWT Key is missing in appsettings.json");

			var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

			var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

			var jwtToken = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: userClaims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: signingCredentials
			);

			// ✅ RESPONSE
			return Ok(new
			{
				message = "SignIn Success",
				token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
				user_name = foundUser.user_name,
				role = foundUser.role
			});
		}
	}
}


