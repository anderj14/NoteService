using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteService.Models;
using NoteService.Tools;

namespace NoteService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly NoteDatabaseContext _context;

        public UserController(NoteDatabaseContext context)
        {
            _context = context;
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

                return Ok(dbUser);
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
    }
}
