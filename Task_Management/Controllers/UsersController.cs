using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] tblUser user)
        {
            // Check if the username is already taken
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Username is already taken");
            }

            // Hash the password before storing it
            user.Password = HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return StatusCode(201); // Created
        }

        // POST: api/users/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] tblUser loginUser)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == loginUser.Username);

            // Check if the user exists and the password is correct
            if (user == null || !VerifyPassword(loginUser.Password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string HashPassword(string password)
        {
            // Use a strong hashing algorithm (Argon2 not available in C# Core yet)
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return storedPassword == HashPassword(enteredPassword);
        }

        private string GenerateJwtToken(tblUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Adjust as needed
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
