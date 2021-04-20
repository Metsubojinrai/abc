using Blog.Areas.Account.Models;
using Blog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Blog.Areas.Account.Models.ResetPasswordViewModel;

namespace Blog.Areas.Account.Controllers
{
    [Area("Account")]
    [Route("[controller]/[action]")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager
            ,ILogger<AccountController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _signManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if(ModelState.IsValid)
            {
                User user = new()
                {
                    UserName = model.Input.UserName,
                    Email = model.Input.Email
                };
                var result = await _userManager.CreateAsync(user, model.Input.Password);

                if(result.Succeeded)
                {
                    _logger.LogInformation("Đăng ký thành công");

                    // phát sinh token theo thông tin user để xác nhận email
                    // mỗi user dựa vào thông tin sẽ có một mã riêng, mã này nhúng vào link
                    // trong email gửi đi để người dùng xác nhận
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    // callbackUrl = /Account/ConfirmEmail?userId=useridxx&code=codexxxx
                    // Link trong email người dùng bấm vào, nó sẽ gọi Page: /Acount/ConfirmEmail để xác nhận
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                        values: new {area = "Account",userId = user.Id, code, returnUrl },
                        protocol: Request.Scheme);
                    // Gửi email    
                    await _emailSender.SendEmailAsync(model.Input.Email, "Xác nhận địa chỉ email",
                        $"Hãy xác nhận địa chỉ email bằng cách <a href='{callbackUrl}'>Bấm vào đây</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        // Nếu cấu hình phải xác thực email mới được đăng nhập thì chuyển hướng đến trang
                        // RegisterConfirmation - chỉ để hiện thông báo cho biết người dùng cần mở email xác nhận
                        return RedirectToAction("RegisterConfirmation", new { email = model.Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // Không cần xác thực - đăng nhập luôn
                        await _signManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl)
        {
            if(email==null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Không có user với email: '{email}'.");
            }

            if (user.EmailConfirmed)
            {
                // Tài khoản đã xác thực email
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterConfirmationViewModel
            {
                Email = email
            };
            if (returnUrl != null)
            {
                model.UrlContinue = Url.Action("ConfirmEmail","Account", new { email = model.Email, returnUrl = returnUrl });
            }
            else
                model.UrlContinue = Url.Action("ConfirmEmail","Account" ,new { email = model.Email });

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string area, string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Không tồn tại User - '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            // Xác thực email
            var result = await _userManager.ConfirmEmailAsync(user, code);

            var model = new ConfirmEmailViewModel();
            if (result.Succeeded)
            { 
                // Đăng nhập luôn nếu xác thực email thành công
                await _signManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                model.StatusMessage = "Lỗi xác nhận email";
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                // Tìm user theo email gửi đến
                var user = await _userManager.FindByEmailAsync(model.Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // Phát sinh Token để reset password
                // Token sẽ được kèm vào link trong email,
                // link dẫn đến trang /Account/ResetPassword để kiểm tra và đặt lại mật khẩu
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action("ResetPassword", "Account",
                        values: new { area = "Account", code = code},
                        protocol: Request.Scheme);

                // Gửi email
                await _emailSender.SendEmailAsync(
                    model.Input.Email,
                    "Đặt lại mật khẩu",
                    $"Để đặt lại mật khẩu hãy <a href='{callbackUrl}'>bấm vào đây</a>.");
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation(string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string area, string code = null)
        {
            var model = new ResetPasswordViewModel();
            if (code == null)
            {
                return BadRequest("Mã token không có.");
            }
            else
            {
                model.Input = new InputModel
                {
                    // Giải mã lại code từ code trong url (do mã này khi gửi mail
                    // đã thực hiện Encode bằng WebEncoders.Base64UrlEncode)
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Tìm User theo email
            var user = await _userManager.FindByEmailAsync(model.Input.Email);
            if (user == null)
            {
                // Không thấy user
                return RedirectToAction("ResetPasswordConfirmation");
            }

            // Đặt lại passowrd user - có kiểm tra mã token khi đổi
            var result = await _userManager.ResetPasswordAsync(user, model.Input.Code, model.Input.Password);

            if (result.Succeeded)
            {
                // Chuyển đến trang thông báo đã reset thành công
                return RedirectToAction("ResetPasswordConfirmation");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(model);
        }

        public IActionResult ResetPasswordConfirmation(string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signManager.SignOutAsync();
            _logger.LogInformation("Đăng xuất thành công");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = "")
        {
            //Xóa cookie bên ngoài hiện có để đảm bảo quy trình đăng nhập
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                //Tìm kiếm user theo username hoặc email
                User user = await _userManager.FindByNameAsync(model.Input.UserNameOrEmail);
                if (user == null)
                    user = await _userManager.FindByEmailAsync(model.Input.UserNameOrEmail);

                if(user == null)
                {
                    ModelState.AddModelError("","Không tồn tại tài khoản");
                    return View(model);
                }
                var rs = await _signManager.PasswordSignInAsync(user, model.Input.Password, model.Input.RememberMe, true);
                if(rs.Succeeded)
                {
                    _logger.LogInformation("Đăng nhập thành công");
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else return RedirectToAction("Index", "Home");
                }

                if (rs.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginTwoStep), new { model.Input.RememberMe, model.ReturnUrl });
                }

                if (rs.IsLockedOut)
                {
                    _logger.LogInformation("Tài khoản bị tạm khóa");
                    return RedirectToAction("Lockout", "Account");
                }
                else
                {
                    _logger.LogInformation("Không đăng nhập được");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginTwoStep(bool rememberMe, string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = await _signManager.GetTwoFactorAuthenticationUserAsync();

            if(user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new TwoStepModel { RememberMe = rememberMe };

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginTwoStep(TwoStepModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                ModelState.AddModelError("", "Không tồn tại tài khoản");
                return View();
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return Redirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
            {
                return RedirectToAction("Index","Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            var model = new ConfirmEmailChangeViewModel();
            if (!result.Succeeded)
            {
                model.StatusMessage = "Error changing email.";
                return View(model);
            }

            await _signManager.RefreshSignInAsync(user);
            model.StatusMessage = "Thank you for confirming your email change.";
            return View(model);

        }

        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
