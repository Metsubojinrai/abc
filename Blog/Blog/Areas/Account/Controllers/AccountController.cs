using Blog.Areas.Account.Models;
using Blog.Controllers;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
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
        private readonly ISMSSender _sMSSender;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager
            ,ILogger<AccountController> logger, IEmailSender emailSender, ISMSSender sMSSender)
        {
            _logger = logger;
            _signManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _sMSSender = sMSSender;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        #region Register
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
        #endregion

        #region Forgot Password
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
        #endregion

        #region Log out
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signManager.SignOutAsync();
            _logger.LogInformation("Đăng xuất thành công");
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Log in
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = "")
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/");

            //Xóa cookie bên ngoài hiện có để đảm bảo quy trình đăng nhập
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var model = new LoginViewModel()
            {
                ExternalLogins = (await _signManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // Đã đăng nhập nên chuyển hướng về Index
            if (_signManager.IsSignedIn(User)) return Redirect("Index");

            if (ModelState.IsValid)
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
                        return LocalRedirect(model.ReturnUrl);
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
        #endregion

        // Post yêu cầu login bằng dịch vụ ngoài
        // Provider = Google, Facebook ... 
        #region External Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
        {
            // Kiểm tra yêu cầu dịch vụ provider tồn tại
            var listprovider = (await _signManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var provider_process = listprovider.Find((m) => m.Name == provider);
            if (provider_process == null)
            {
                return NotFound("Dịch vụ không chính xác: " + provider);
            }

            // redirectUrl - là Url sẽ chuyển hướng đến - sau khi CallbackPath (/dang-nhap-tu-google) thi hành xong
            // nó bằng /account/externallogin?handler=Callback
            // tức là gọi CallbackAsync
            string redirectUrl = Url.Action(nameof(ExternalLoginConfirmation), new { ReturnUrl = returnUrl });

            // Cấu hình
            var properties = _signManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            // Chuyển hướng đến dịch vụ ngoài (Googe, Facebook)
            return Challenge(properties,provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Lỗi provider: {remoteError}";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            // Lấy thông tin do dịch vụ ngoài chuyển đến
            var info = await _signManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi thông tin từ dịch vụ đăng nhập.";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            // Đăng nhập bằng thông tin LoginProvider, ProviderKey từ info cung cấp bởi dịch vụ ngoài
            // User nào có 2 thông tin này sẽ được đăng nhập - thông tin này lưu tại bảng UserLogins của Database
            // Trường LoginProvider và ProviderKey ---> tương ứng UserId 
            var result = await _signManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // User đăng nhập thành công vào hệ thống theo thông tin info
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);              
            }
            else if (result.IsLockedOut)
            {
                // Bị tạm khóa
                return RedirectToAction("Lockout");
            }
            else
            {
                var userExisted = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (userExisted != null)
                {
                    // Đã có Acount, đã liên kết với tài khoản ngoài - nhưng không đăng nhập được
                    // có thể do chưa kích hoạt email
                    return RedirectToAction("RegisterConfirmation", new { userExisted.Email });

                }

                // Chưa có Account liên kết với tài khoản ngoài
                // Hiện thị form để thực hiện bước tiếp theo ở ExternalLoginConfirmation
                var model = new ExternalLoginViewModel
                {
                    ReturnUrl = returnUrl,
                    ProviderDisplayName = info.ProviderDisplayName
                };
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    // Có thông tin về email từ info, lấy email này hiện thị ở Form
                    model.Input = new ExternalLoginViewModel.InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }

                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // Lấy lại Info
            var info = await _signManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Không có thông tin tài khoản ngoài.";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                string externalMail = null;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    externalMail = info.Principal.FindFirstValue(ClaimTypes.Email);
                }
                var userWithexternalMail = (externalMail != null) ? (await _userManager.FindByEmailAsync(externalMail)) : null;

                // Xử lý khi có thông tin về email từ info, đồng thời có user với email đó
                // trường hợp này sẽ thực hiện liên kết tài khoản ngoài + xác thực email luôn     
                if ((userWithexternalMail != null) && (model.Input.Email == externalMail))
                {
                    // xác nhận email luôn nếu chưa xác nhận
                    if (!userWithexternalMail.EmailConfirmed)
                    {
                        var codeactive = await _userManager.GenerateEmailConfirmationTokenAsync(userWithexternalMail);
                        await _userManager.ConfirmEmailAsync(userWithexternalMail, codeactive);
                    }
                    // Thực hiện liên kết info và user
                    var resultAdd = await _userManager.AddLoginAsync(userWithexternalMail, info);
                    if (resultAdd.Succeeded)
                    {
                        // Thực hiện login    
                        await _signManager.SignInAsync(userWithexternalMail, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Tài khoản chưa có, tạo tài khoản mới
                var user = new User { UserName = model.Input.Email, Email = model.Input.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {

                    // Liên kết tài khoản ngoài với tài khoản vừa tạo
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Đã tạo user mới từ thông tin {Name}.", info.LoginProvider);
                        // Email tạo tài khoản và email từ info giống nhau -> xác thực email luôn
                        if (user.Email == externalMail)
                        {
                            var codeactive = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            await _userManager.ConfirmEmailAsync(user, codeactive);
                            await _signManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                            return Redirect(returnUrl);
                        }

                        // Trường hợp này Email tạo User khác với Email từ info (hoặc info không có email)
                        // sẽ gửi email xác để người dùng xác thực rồi mới có thể đăng nhập
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Account",
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(model.Input.Email, "Xác nhận địa chỉ email",
                            $"Hãy xác nhận địa chỉ email bằng cách <a href='{callbackUrl}'>bấm vào đây</a>.");

                        // Chuyển đến trang thông báo cần kích hoạt tài khoản
                        if (_userManager.Options.SignIn.RequireConfirmedEmail)
                        {
                            return RedirectToAction("RegisterConfirmation", new { Email = model.Input.Email });
                        }

                        // Đăng nhập ngay do không yêu cầu xác nhận email
                        await _signManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.ProviderDisplayName = info.ProviderDisplayName;
            model.ReturnUrl = returnUrl;
            return View(model);
        }
        #endregion

        #region Login 2FA
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginTwoStep(bool rememberMe, string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = await _signManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return NotFound();

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

            var model = new TwoStepModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe };

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginTwoStep(TwoStepModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _sMSSender.SendSms(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Yêu cầu người dùng đã đăng nhập bằng tên người dùng / mật khẩu 
            var user = await _signManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "Tài khoản người dùng bị khóa.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Mã không hợp lệ.");
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }
        #endregion

        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
