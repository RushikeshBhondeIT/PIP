using EmployeeAPI.Models;
using EmployeeAPI.Models.Authentication.SignIn;
using EmployeeAPI.Models.Authentication.SignUp;
using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Serilog;
using ServiceStack.DataAnnotations;
using ServiceStack.Messaging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [RoutePrefix("api/v1/")]
    public class AccountsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        ObjectResult? status;


        public AccountsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _signInManager = signInManager;
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
                return StatusCode(StatusCodes.Status400BadRequest, LogInformation("Error", string.Format($"{registerDTO.EmployeeName} User Already  Exist")));
            }
            IdentityUser user = new()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.EmployeeName,
                // TwoFactorEnabled = true
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                return await this.RegisterUserMethod(user, registerDTO, role);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, LogInformation("Error", string.Format($"{role}" + " " + "This Role Does not exist ")));
            }
        }




        private async Task<ObjectResult> RegisterUserMethod(IdentityUser user, RegisterUser registerDTO, string role)
        {
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, LogInformation("Error", $"{registerDTO.EmployeeName}" + " " + "User Failed to create, Please try again... "));
            }
            //Add role to the user...
            await _userManager.AddToRoleAsync(user, role);
            //Add Token to verify the email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_configuration["Url:EmailConfirmationLink"]}{token}&email={user.Email}";
            var message = new MessageForEmail(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
            _emailService.SendEmailToVerify(message);
            return StatusCode(StatusCodes.Status201Created, LogInformation("Success", string.Format($" {registerDTO.EmployeeName}" + " " + "User Created & Email Sent to" + " "+ $"{user.Email}"+" "+ $"Successfully")));
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

                    return StatusCode(StatusCodes.Status200OK, LogInformation("Success", string.Format($"{user.UserName}" + " " + "Email Verified Successfully ")));
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest, LogInformation("Success", $"User does not exist"));
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> Login([FromBody] LogIn loginModel)
        {
            //checking the user ...

            var user = await _userManager.FindByEmailAsync(loginModel.Email!);  //exception throw.
            if (user == null)//check
            {
                return StatusCode(StatusCodes.Status401Unauthorized, LogInformation("Error", string.Format($"{loginModel.Email} This User account dosent exist , Please register yourself")));
            }
            await _signInManager.SignOutAsync();
            var pass = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
            if (pass.RequiresTwoFactor == true && user.TwoFactorEnabled)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                var message = new MessageForEmail(new string[] { user.Email! }, "OTP Confirmation", token);
                _emailService.SendEmailToVerify(message);

                return StatusCode(StatusCodes.Status200OK, LogInformation("Success", string.Format($"We have sent an OTP to your Email = {user.Email} ")));
            }

            //checking the password
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password!))
            {
                if( user.EmailConfirmed == true)
                {
                    return this.LogInHepler(user).Result;
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, LogInformation("Error", string.Format($"{loginModel.Email} Please confirm your email !")));
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, LogInformation("Error", string.Format($"{loginModel.Email} Please Enter Correct Password !")));
            }
        }



        [HttpGet("LogIn-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
                if (signIn.Succeeded)
                {
                    if (user != null)
                    {
                        return this.LogInHepler(user!).Result;
                    }

                }
                return StatusCode(StatusCodes.Status404NotFound, LogInformation("Error", $"Invalid code"));
            }
            return StatusCode(StatusCodes.Status400BadRequest, LogInformation("Error", $"Invalid username or otp please try it again"));
        }

        [AllowAnonymous]
        [HttpGet("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = $"{_configuration["Url:ForgotPasswordLink"]}{token}&email={user.Email}";
                var message = new MessageForEmail(new string[] { user.Email! }, "Forgot Password link", forgotPasswordLink!);
                _emailService.SendEmailToVerify(message);
                return StatusCode(StatusCodes.Status200OK, LogInformation(forgotPasswordLink, string.Format( $"Password change request is sent on Email {user.Email} Please Open your email  & click the link")));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, LogInformation("Error", $"Cannot send email , please try again with proper email"));
        }

        [AllowAnonymous]
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, EMail = email };
            return Ok(new
            {
                model
            });
        }

        [AllowAnonymous]
        [HttpPost("change-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.EMail);
            if (user != null)
            {
                var tokens = resetPassword.Token.Replace(" ", "+");
                var resetPaaResult = await _userManager.ResetPasswordAsync(user, tokens, resetPassword.Password);
                if (resetPaaResult.Succeeded)
                {
                    foreach (var error in resetPaaResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return StatusCode(StatusCodes.Status200OK, LogInformation("Success", $"Password has been changed !"));
                }
                return StatusCode(StatusCodes.Status500InternalServerError, LogInformation("Error", $"Password has not been changed , Please try once again"));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, LogInformation("Error", $"User is not available in the resource , Please try again"));
        }

        [AllowAnonymous]
        [HttpGet("UserDetail")]
        public async Task<IActionResult> GetUserDetail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return StatusCode(StatusCodes.Status200OK, LogInformation("Success", $"{user}"));

            }
            return StatusCode(StatusCodes.Status500InternalServerError, LogInformation("Error", $"User is not available in the resource , Please try again"));
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
        private Response LogInformation(string status, string message)
        {
            Response res = new Response
            {
                Status = status,
                Message = message
            };
            if (status == "Error")
            {
                Log.Error(res.Status + " " + " " + res.Message);
            }
            else
            {
                Log.Information(res.Status + " " + " " + res.Message);
            }
          
            return res;
        }


        private async Task<OkObjectResult> LogInHepler(IdentityUser user)
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
            Log.Information(string.Format($"User {user.UserName} Logged in Time ={DateTime.Now}" + " " + $"With Expiration Time:{jwtToken.ValidTo}"));
            // returning the token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expration = jwtToken.ValidTo
            });
        }
    }
}
