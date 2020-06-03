using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using Microsoft.IdentityModel.Tokens;
using SqlDatabase;

namespace Controllers
{
    public class ApplicationUserController: Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private ApplicationContext _context;

        private IConfiguration Configuration { get; }

        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ApplicationContext context,
        IConfiguration configuration
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            Configuration = configuration;
        }

        [HttpPost("api/register",Name="register")]
        public async Task<IActionResult> Register([FromBody]UserForCreationDto userForRegister)
        {
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = userForRegister.UserName,
                    EmpId = userForRegister.EmpId
                };

                var userRegistered = await _userManager.CreateAsync(user, userForRegister.Password);
                return Created("register", userRegistered);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("api/login")]
        public async Task<IActionResult> Login([FromBody]UserForCreationDto user)
        {
            try
            {
                var existUser = await _userManager.FindByNameAsync(user.UserName);

                if (existUser != null && await _userManager.CheckPasswordAsync(existUser,user.Password))
                {
                    var tokenDescriptor = new SecurityTokenDescriptor()
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim("userId",existUser.Id), 
                        }),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("ApplicationSettings:JWT_Secret"))), SecurityAlgorithms.HmacSha256)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    return Ok(new {token});

                }
                else
                {
                    return BadRequest(new { message = "Email or password is incorrect!" });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
