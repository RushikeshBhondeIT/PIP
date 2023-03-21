using EmployeeAPI.Models;
using EmployeeAPI.Models.Authentication.SignIn;
using EmployeeAPI.Models.Authentication.SignUp;
using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceStack.DataAnnotations;
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
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "User Already  Exist" });
            }
            IdentityUser user = new()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.EmployeeName,
                TwoFactorEnabled = true
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                return await this.RegisterUserMethod(user, registerDTO, role);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This Role Does not exist " });
            }
        }

        private async Task<ObjectResult> RegisterUserMethod(IdentityUser user, RegisterUser registerDTO, string role)
        {
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Failed to create,Please try again... " });
            }
            //Add role to the user...
            await _userManager.AddToRoleAsync(user, role);
            //Add Token to verify the email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_configuration["Url:EmailConfirmationLink"]}{token}&email={user.Email}";
            var message = new MessageForEmail(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
            _emailService.SendEmailToVerify(message);
            return StatusCode(StatusCodes.Status201Created, new Response { Status = "Success", Message = $"User Created & Email Sent to {user.Email}  Successfully " });
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
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This use does not exist ! " });
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> Login([FromBody] LogIn loginModel)
        {
            //checking the user ...
            var user = await _userManager.FindByNameAsync(loginModel.Username!);  //exception throw.
            if (user == null )//check
            {
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Error", Message = $"Please enter correct username or password !" });
            }
            await _signInManager.SignOutAsync();
            var pass = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
            if (pass.RequiresTwoFactor == true&&user.TwoFactorEnabled)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                var message = new MessageForEmail(new string[] { user.Email! }, "OTP Confirmation", token);
                _emailService.SendEmailToVerify(message);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"We have sent an OTP to your Email = {user.Email} " });
            }

            //checking the password
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password!))
            {
                return this.LogInHepler(user).Result;
            }
            return Unauthorized( StatusCode(StatusCodes.Status200OK, new Response { Status = "Error", Message = $"This User is not Authorized" }));
        }

        [HttpPost("LogIn-2FA")]
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
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = $"Invalid code" });

            }
            return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = $"Invalid username or otp please try it again" });
        }

        [AllowAnonymous]
        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = $"{_configuration["Url:ForgotPasswordLink"]}{token}&email={user.Email}";
                var message = new MessageForEmail(new string[] { user.Email! }, "Forgot Password link", forgotPasswordLink!);
                _emailService.SendEmailToVerify(message);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"Password change request is sent on Email {user.Email} Please Open your email  & click the link" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"Cannot send email , please try again with proper email" });
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
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Success", Message = $"Password has  been changed !" });

                }
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"Password has not been changed , Please try once again" });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"Couldnt Send link to email , Please try again" });
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
            // returning the token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expration = jwtToken.ValidTo
            });
        }
    }
}
