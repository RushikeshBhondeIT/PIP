using EmployeeAPI.Models;
using EmployeeAPI.Models.Authentication.SignIn;
using EmployeeAPI.Models.Authentication.SignUp;
using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Umbraco.Core;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [System.Web.Http.RoutePrefix("api/v1/")]
    public class AccountsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpGet("Register")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> Index([FromBody] RegisterUser registerDTO, string role)
        {
            if (ModelState.IsValid == false)
            {
                var error = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return (IActionResult)registerDTO;
            }
            var userExist = await _userManager.FindByEmailAsync(registerDTO.Email);
            //Check user is exist
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "USER Already  Exist" });
            }

            IdentityUser user = new()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.EmployeeName,
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "USER Failed to create " });
                }
                //Add role to the user...
                await _userManager.AddToRoleAsync(user, role);

                //Add Token to verify the email

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"https://localhost:7115/api/v1/ConfirmEmail?token={token}&email={user.Email}";
                //var url = Url.Action("ConfirmEmail", "AccountsController", new { token, email = user.Email }, HttpContext.Request.Scheme);
                var message = new MessageForEmail(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                _emailService.SendEmailToVerify(message);

                return StatusCode(StatusCodes.Status201Created, new Response { Status = "Info", Message = $"User Created & Email Sent to {user.Email}  Successfully " });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This Role Does not exist " });
            }
            //Assign role
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokens = token.Replace(" ", "+");
            if (user != null)
            {

                var result = await _userManager.ConfirmEmailAsync(user, tokens.ToString());
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Verified Successfully " });

                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This Doesnot  exist ! " });
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> Login([FromBody] LogIn loginModel)
        {
            //checking the user ...
            var user = await _userManager.FindByEmailAsync(loginModel.Username!);
            //checking the password
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password!))
            {
                //claimlist creating
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                //we add roles to the claim
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                //generate the token with the claims 
                var jwtToken = GetToken(authClaims);
                // returning the token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expration = jwtToken.ValidTo
                });
            }
            return Unauthorized();
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
