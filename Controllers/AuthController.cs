using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthController));
        //private readonly UserDbContext _context;
       // private IConfiguration _config;
        static List<User> users;
        private IConfiguration _config;
        static AuthController()
        {
            users = new List<User>() {
            new User(){User_Id=1,Username="meghana",Password="meghana"},
            new User(){User_Id=2,Username="laxmi",Password="laxmi"},
            new User(){User_Id=3,Username="tara",Password="tara"}
            };
        }
        public AuthController(IConfiguration config)
        {
            //_context = context;
            _config = config;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] User login)
        {

            _log4net.Info(" login method is running");
            IActionResult response = Unauthorized();
            var user = users.FirstOrDefault(c => c.Username == login.Username && c.Password == login.Password);
            if (user == null)
            {
                _log4net.Info(" user null");
                return NotFound();
            }

            else
            {

                _log4net.Info(" user not null");
                var tokenString = GenerateJSONWebToken(login);
                response = Ok(new { token = tokenString });
                _log4net.Info(" token is obtained");

                return response;
            }
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            _log4net.Info("Token Generation initiated");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}