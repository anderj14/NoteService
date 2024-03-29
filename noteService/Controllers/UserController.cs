using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NoteService.Models;
using NoteService.Tools;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly NoteDatabaseContext _context;
        private readonly IConfiguration _configuration;

        public UserController(NoteDatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> UserLogin([FromBody] User user)
        {
            try
            {
                String password = Password.hashPassword(user.Password);

                var dbUser = _context.Users.Where(u => u.Username == user.Username && u.Password == password).Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Active
                }).FirstOrDefault();

                if (dbUser == null)
                {
                    return Ok("Username or password is incorrect");
                }

                List<Claim> authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dbUser.Username),
                    new Claim("userID", dbUser.UserId.ToString()),
                    new Claim("username", dbUser.Username)
                };

                var token = this.getToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> UserRegister([FromBody] User user)
        {

            try
            {
                var dbUser = await _context.Users.Where(u => u.Username == user.Username).FirstOrDefaultAsync();

                if (dbUser != null)
                {
                    return BadRequest("Username already exists");
                }

                user.Password = Password.hashPassword(user.Password);
                user.Active = 1;
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok("User is successfully registered");
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [Authorize]
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                List<User> listUsers = _context.Users.ToList();

                if (listUsers != null)
                {
                    return Ok(listUsers);
                }
                return Ok("They are no users in the database");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private JwtSecurityToken getToken(List<Claim> authClaim)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(24),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
