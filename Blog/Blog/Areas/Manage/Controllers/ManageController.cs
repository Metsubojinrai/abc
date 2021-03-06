using Blog.Areas.Manage.Models;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Blog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("[controller]/[action]")]
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ManageController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UrlEncoder _urlEncoder;
        private readonly ISMSSender _sMSSender;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(UserManager<User> userManager,
            SignInManager<User> signInManager, ILogger<ManageController> logger,
            IEmailSender emailSender, UrlEncoder urlEncoder, ISMSSender sMSSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
            _sMSSender = sMSSender;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #region Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound($"Không tải được tài khoản ID: '{_userManager.GetUserId(User)}'.");
            }

            var model = new UserProfileViewModel()
            {
                UserName = user.UserName,
                Input = new UserProfileViewModel.InputModel
                {
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    Address=user.Address,
                    Birthday=user.Birthday,
                    ProfilePicture=user.ProfilePicture
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không có tài khoản ID :'{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(model.Input.FullName!=user.FullName)
            {
                user.FullName = model.Input.FullName;
                await _userManager.UpdateAsync(user);
            }

            if (model.Input.Address != user.Address)
            {
                user.Address = model.Input.Address;
                await _userManager.UpdateAsync(user);
            }

            if (model.Input.Birthday != user.Birthday)
            {
                user.Birthday = model.Input.Birthday;
                await _userManager.UpdateAsync(user);
            }

            if(Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                var basePath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\File\\Image\\User\\");
                bool basePathExist = System.IO.Directory.Exists(basePath);
                if (!basePathExist)
                    Directory.CreateDirectory(basePath);
                var fileName = Path.Combine(file.FileName);
                var filePath = Path.Combine(basePath, file.FileName);
                if(!System.IO.File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath,FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }                
                }
                user.ProfilePicture = fileName;
                await _userManager.UpdateAsync(user);
            }
            await _signInManager.RefreshSignInAsync(user);
            model.StatusMessage = "Hồ sơ đã cập nhật";
            return RedirectToAction("Index", "Manage");
        }
        #endregion

        #region Change Password
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return NotFound($"Không có tài khoản ID :'{_userManager.GetUserId(User)}'.");
            }

            var hasPwd = await _userManager.HasPasswordAsync(user);
            if(!hasPwd)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if(user==null)
            {
                return NotFound($"Không có tài khoản ID :'{_userManager.GetUserId(User)}'.");
            }

            var changePwdResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if(!changePwdResult.Succeeded)
            {
                AddErrors(changePwdResult);
                return View(model);
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("Thay đổi mật khẩu thành công.");
            model.StatusMessage = "Mật khẩu đã thay đổi.";
            return RedirectToAction(nameof(ChangePassword));
        }
        #endregion

        #region Set Password
        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không có tài khoản ID: '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không có tài khoản ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            model.StatusMessage = "Mật khẩu đã được thiết lập.";

            return RedirectToAction(nameof(SetPassword));
        }
        #endregion

        #region Change Email
        [HttpGet]
        public async Task<IActionResult> Email()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không tải được tài khoản ID '{_userManager.GetUserId(User)}'.");
            }
            var email = await _userManager.GetEmailAsync(user);
            var model = new EmailViewModel()
            {
                Email = email,

                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user)
            };
            return View(model);
        }

        [HttpPost,ActionName("Email")]
        public async Task<IActionResult> ChangeEmail(EmailViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không tải được tài khoản ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = await _userManager.GetEmailAsync(user);
            if (model.Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action(
                    "ConfirmEmailChange", "Account",
                    values: new { userId, email = model.Input.NewEmail, code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(model.Input.NewEmail,
                    "Xác nhận Email của bạn",
                    $"Xác nhận tài khoản bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a>.");

                model.StatusMessage = "Liên kết xác nhận thay đổi Email đã được gửi. Hãy kiểm tra Email của bạn.";
                return RedirectToAction("Email");
            }

            model.StatusMessage = "Email của bạn đã thay đổi.";
            return RedirectToAction("Email");
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationEmail(EmailViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không tải được tài khoản ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ConfirmEmail", "Account",
                values: new { userId, code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
            email,
            "Xác nhận Email của bạn.",
            $"Xác nhận tài khoản bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a>.");

            model.StatusMessage = "Email xác minh đã được gửi. Vui lòng kiểm tra Email của bạn.";
            return RedirectToAction("Email");
        }
        #endregion

        #region 2FA
        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không tải được tài khoản ID '{_userManager.GetUserId(User)}'.");
            }
            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không tải được tài khoản ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Đã xảy ra lỗi không mong muốn khi tắt 2FA cho người dùng có ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không tải được tài khoản ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Đã xảy ra lỗi không mong muốn khi tắt 2FA cho người dùng có ID '{user.Id}'.");
            }

            _logger.LogInformation("Người dùng ID {UserId} đã tắt 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không tải được tài khoản ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không tải được tài khoản '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            _logger.LogInformation("Người dùng ID {UserId} đã bật 2FA với ứng dụng xác thực.", user.Id);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không thể tải người dùng có ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Không thể tạo mã khôi phục cho người dùng có ID '{user.Id}' vì họ chưa bật 2FA.");
            }

            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Không thể tải người dùng có ID '{_userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Không thể tạo mã khôi phục cho người dùng có ID '{user.Id}' vì họ chưa bật 2FA.");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            _logger.LogInformation("Người dùng có ID {UserId} đã tạo mã khôi phục 2FA mới.", user.Id);

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            return View(nameof(ShowRecoveryCodes), model);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey[currentPosition..]);
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("TwoFactAuth"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(User user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }
        #endregion

        #region Phone
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPhoneNumber(PhoneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Tạo mã thông báo và gửi nó
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("Error");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _sMSSender.SendSms(model.PhoneNumber, "Mã bảo mật của bạn là: " + code);
            return RedirectToAction(nameof(VerifyPhone), new { model.PhoneNumber });
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPhone(string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("Error");
            }

            return phoneNumber == null ? View("Error") : View(new PhoneViewModel { PhoneNumber = phoneNumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhone(PhoneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Input.VerificationCode);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(ConfirmPhoneSuccess));
                }
            }

            ModelState.AddModelError(string.Empty, "Không xác minh được số điện thoại");
            return View(model);
        }

        public IActionResult ConfirmPhoneSuccess()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("Error");
            }

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await _sMSSender.SendSms(user.PhoneNumber, "Mã bảo mật của bạn là: " + code);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber(PhoneViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, model.Input.VerificationCode);
                if (result.Succeeded)
                {
                    var update = await _userManager.SetPhoneNumberAsync(user,null);
                    if (update.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region External Login
        [HttpGet]
        public async Task<IActionResult> ExternalLogin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng có ID '{_userManager.GetUserId(User)}'.");
            }
            var model = new ExternalLoginViewModel
            {
                CurrentLogins = await _userManager.GetLoginsAsync(user)
            };
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            if(user.PasswordHash != null || model.CurrentLogins.Count > 1)
            model.ShowRemoveButton = true;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveLogin(ExternalLoginViewModel model, string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Không thể tải người dùng có ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                throw new InvalidOperationException($"Đã xảy ra lỗi không mong muốn khi xóa thông tin đăng nhập bên ngoài cho người dùng có ID '{userId}'.");
            }

            await _signInManager.RefreshSignInAsync(user);
            model.StatusMessage = "Thông tin đăng nhập bên ngoài đã bị xóa.";

            return RedirectToAction("ExternalLogin","Manage");
        }

        [HttpPost]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Xóa cookie bên ngoài hiện có để đảm bảo quy trình đăng nhập sạch sẽ
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            string redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));

            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback(ExternalLoginViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            model.StatusMessage = "The external login was added.";
            return RedirectToAction("ExternalLogin", "Manage");
        }
        #endregion
    }
}
