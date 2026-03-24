using LinkUpApp.Core.Application.Dtos.User;
using LinkUpApp.Core.Application.Dtos.User.Password;
using LinkUpApp.Core.Application.Interfaces.User;
using LinkUpApp.Core.Application.ViewModels.Login;
using LinkUpApp.Core.Application.ViewModels.Login.Password;
using LinkUpApp.Core.Domain.Common.Enum;
using LinkUpApp.Infrastructure.Identity.Entities;
using LinkUpWebApp.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountServicesForWebApp _accountServiceForWebApp;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginController(
            IAccountServicesForWebApp accountService,
            UserManager<ApplicationUser> userManager)
        {
            _accountServiceForWebApp = accountService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? returnUrl = null)
        {
            ApplicationUser? userSession = await _userManager.GetUserAsync(User);

            if (userSession != null)
            {
                var user = await _accountServiceForWebApp.GetUserByUsername(userSession.UserName ?? "");
                if (user != null && user.Role == Roles.User.ToString())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            if (!string.IsNullOrEmpty(returnUrl) &&
                returnUrl != "/" &&
                !returnUrl.Contains("/Login", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.UnauthorizedMessage = "Debe iniciar sesion para acceder a esta seccion.";
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(new LoginViewModel
            {
                UserName = string.Empty,
                Password = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(vm);
            }

            var loginDto = new LoginDto
            {
                UserName = vm.UserName,
                Password = vm.Password
            };

            var result = await _accountServiceForWebApp.AuthenticateAsync(loginDto);

            if (result.HasError)
            {
                ViewBag.ReturnUrl = returnUrl;

                if (result.Errors.Any())
                {
                    ViewBag.ErrorMessage = result.Errors.First();
                }

                return View(vm);
            }

            if (!string.IsNullOrEmpty(returnUrl) &&
                returnUrl != "/" &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountServiceForWebApp.SingOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel
            {
                FirstName = string.Empty,
                LastName = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                UserName = string.Empty,
                Password = string.Empty,
                ConfirmPassword = string.Empty
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string? profilePicturePath = null;
            if (vm.ProfilePicture != null)
            {
                profilePicturePath = FileManager.Upload(vm.ProfilePicture, vm.UserName, "Users");
            }

            SaveUserDto dto = new SaveUserDto
            {
                Id = null, // para nuevos registros
                UserName = vm.UserName,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Phone = vm.Phone,
                Password = vm.Password,
                ProfilePicturePath = profilePicturePath,
                Role = Roles.User.ToString(), // Siempre "User"
                IsActive = false // Inactivo hasta confirmar email
            };

            string origin = Request?.Headers?.Origin.ToString() ?? string.Empty;

            RegisterResponseDto? response = await _accountServiceForWebApp.RegisterUser(dto, origin);

            if (response.HasError)
            {
                ViewBag.hasError = true;
                ViewBag.Errors = response.Errors;
                return View(vm);
            }

            if (response != null && !string.IsNullOrWhiteSpace(response.Id))
            {
                dto.Id = response.Id;
                dto.ProfilePicturePath = FileManager.Upload(vm.ProfilePicture, dto.Id, "Users");
                await _accountServiceForWebApp.EditUser(dto, origin, true); 
            }

            TempData["SuccessMessage"] = "Usuario registrado exitosamente. Por favor revisa tu correo para activar tu cuenta.";
            return RedirectToAction("Index");
        }

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel
            {
                UserName = string.Empty
            });
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string origin = Request?.Headers?.Origin.ToString() ?? string.Empty;

            ForgotPasswordRequestDto dto = new()
            {
                UserName = vm.UserName,
                Origin = origin
            };

            UserResponseDto? returnUser = await _accountServiceForWebApp.ForgotPasswordAsync(dto);

            if (returnUser.HasError)
            {
                ViewBag.hasError = true;
                ViewBag.Errors = returnUser.Errors;
                return View(vm);
            }

            TempData["SuccessMessage"] = "Se ha enviado un correo con instrucciones para restablecer tu contraseña.";
            return RedirectToAction("Index");
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Enlace de restablecimiento invalido.";
                return RedirectToAction("Index");
            }

            var vm = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token,
                Password = string.Empty,
                ConfirmPassword = string.Empty
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            ResetPasswordRequestDto dto = (new ResetPasswordRequestDto
            {
                Id = vm.UserId,
                Token = vm.Token,
                Password = vm.Password
            });

            UserResponseDto? returnUser = await _accountServiceForWebApp.ResetPasswordAsync(dto);

            if (returnUser.HasError)
            {
                ViewBag.hasError = true;
                ViewBag.Errors = returnUser.Errors;
                return View(vm);
            }

            TempData["SuccessMessage"] = "Contraseña restablecida exitosamente. Ya puedes iniciar sesion.";
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Enlace de activacion invalido.";
                return RedirectToAction("Index");
            }

            var response = await _accountServiceForWebApp.ConfirmAccountAsync(userId, token);

            if (response.Contains("Ha ocurrido un error"))
            {
                TempData["ErrorMessage"] = response;
            }
            else
            {
                TempData["SuccessMessage"] = response;
            }

            return RedirectToAction("Index");
        }

    }
}
