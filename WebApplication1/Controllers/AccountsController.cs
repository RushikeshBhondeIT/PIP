using EmployeeAPI.Models;
using EmployeeAPI.Models.Authentication.SignUp;
using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Umbraco.Core;

namespace EmployeeAPI.Controllers
{
    [ApiController]
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

        [HttpGet("api/v1/Register")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("api/v1/RegisterAdmin")]
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
                UserName = registerDTO.Email,
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

        [HttpGet("api/v1/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokens=token.Replace(" ", "+");
            if (user != null)
            {
               
                var result = await _userManager.ConfirmEmailAsync(user, tokens.ToString());
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Verified Successfully " });

                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This Doesnot  exist ! " });

            //var message = new MessageForEmail(new string[]
            //{"chetnasatghare97@gmail.com" }, "TEST", "<h1>Subscribe to my channel!</h1>");
            //_emailService.SendEmailToVerify(message);
            //return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Sent Successfully " });
        }
    }
}
