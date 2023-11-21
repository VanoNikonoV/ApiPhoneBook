using ApiPhoneBook.Data;
using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiPhoneBook.Controllers
{
    [Route("authentication")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UsersContext _context;
        
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration, UsersContext context) 
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    User _user = new User();
                    string passwordHash
                    = BCrypt.Net.BCrypt.HashPassword(request.Password); // qwerty = "$2a$11$wDDh6OB3wZ./MgR.rHrT3esk6dwUEDwgaebGxBXQoPfcVh7eJQiMG"
                    
                    _user.FirstName = request.FirstName;
                    _user.LastName = request.LastName;
                    _user.PasswordHash = passwordHash;
                    _user.Email = request.Email;
                    _user.RoleId = 2;
     
                    await _context.Users.AddAsync(_user);

                    await _context.SaveChangesAsync();

                    return Ok();
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(request);

        }

        /// <summary>
        /// Метод проверяет наличие пользователя с данным логином, если пароли совпадаю отправляет jwt-токен
        /// </summary>
        /// <param name="request"> Модель запроса с логином пользователя и паролем</param>
        /// <returns>jwt-токен в string</returns>
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(RequestLogin request)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Неверный пароль");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var identity = GetIdentity(user);

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            //    _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha512Signature);

            DateTime now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private ClaimsIdentity GetIdentity(User user)
        {
            User user_ = _context.Users.FirstOrDefault(u => u.Email == user.Email); 

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "User", user.Email),
                    
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

    }
}
