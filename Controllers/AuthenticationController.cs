using ApiPhoneBook.Data;
using ApiPhoneBook.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhoneBook.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Azure.Core;

namespace ApiPhoneBook.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UsersContext _context;
        public static User user = new User();
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration, UsersContext context) 
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        //[ValidateAntiForgeryToken]
        public  async Task< ActionResult<User>> Register(UserDto request)
        {
            if (ModelState.IsValid)
            {
                User user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

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

        [HttpPost("login")]
        public ActionResult<User> Login(UserDto request)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: identity.Claims,
                    expires: DateTime.Now.AddDays(1),
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


        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                //new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
