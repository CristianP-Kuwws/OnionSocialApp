using LinkUpApp.Core.Application.Dtos.Email;
using LinkUpApp.Core.Application.Dtos.User;
using LinkUpApp.Core.Application.Dtos.User.Password;
using LinkUpApp.Core.Application.Interfaces.Email;
using LinkUpApp.Core.Application.Interfaces.User;
using LinkUpApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LinkUpApp.Infrastructure.Identity.Services
{
    public class AccountServicesForWebApp : IAccountServicesForWebApp
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountServicesForWebApp(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
        {

            LoginResponseDto response = new()
            {
                Id = "",
                UserName = "",
                FirstName = "",
                LastName = "",
                Email = "",
                HasError = false,
                Errors = []
            };

            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No hay una cuenta registrada con este nombre {loginDto.UserName}");
                return response;
            }

            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.Errors.Add($"El correo de esta cuenta: {loginDto.UserName} no esta confirmado o la cuenta no se encuentra activa." +
                    $" Revisa tu correo electronico.");
                return response;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                response.HasError = true;

                if (result.IsLockedOut)
                {
                    response.Errors.Add($"Tu cuenta {loginDto.UserName} ha sido bloqueada temporalmente por multiples intentos fallidos. Intenta iniciar sesion de nuevo en 10 minutos.");
                }
                else
                {
                    response.Errors.Add($"Las credenciales de inicio de sesion para el usuario {user.UserName} son invalidas.");

                }
                return response;
            }

            var rolesList = await _userManager.GetRolesAsync(user);

            response.Id = user.Id;
            response.UserName = user.UserName;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.Email = user.Email ?? "";
            response.IsVerified = user.EmailConfirmed;
            response.Roles = rolesList.ToList();

            return response;
        }

        public async Task SingOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<RegisterResponseDto> RegisterUser(SaveUserDto saveDto, string origin) 
        {
            RegisterResponseDto response = new()
            {
                Id = "",
                UserName = "",
                FirstName = "",
                LastName = "",
                Email = "",
                HasError = false,
                Errors = []
            };

            var userWithSameUserName = await _userManager.FindByNameAsync(saveDto.UserName);
            if (userWithSameUserName != null)
            {
                response.HasError = true;
                response.Errors.Add($"Este nombre de usuario ya esta ocupado. {saveDto.UserName}.");
                return response;
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(saveDto.Email);
            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.Errors.Add($"Este correo ya esta ocupado. {saveDto.Email}.");
                return response;
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = saveDto.UserName,
                FirstName = saveDto.FirstName,
                LastName = saveDto.LastName,
                Email = saveDto.Email,
                EmailConfirmed = false,
                Phone = saveDto.Phone,
                ProfilePicturePath = saveDto.ProfilePicturePath,
                IsActive = saveDto.IsActive,

            };

            var result = await _userManager.CreateAsync(newUser, saveDto.Password);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(s => s.Description).ToList());
                return response;
            }

            if (!await _userManager.IsInRoleAsync(newUser, saveDto.Role)) // por el momento solo tenemos el rol de usuario, pero se deja abierto a extension 
                await _userManager.AddToRoleAsync(newUser, saveDto.Role);                                        

            string verificationUri = await GetVerificationEmailUri(newUser, origin);

            await _emailService.SendAsync(new EmailRequestDto()
            {
                ToEmail = saveDto.Email,
                Subject = "Confirma tu cuenta de LinkUpApp",
                HtmlBody = $"Hola {newUser.FirstName}, por favor confirma tu cuenta haciendo clic en el siguiente enlace: {verificationUri}"
            });

            var rolesList = await _userManager.GetRolesAsync(newUser);

            response.Id = newUser.Id;
            response.UserName = newUser.UserName;
            response.FirstName = newUser.FirstName;
            response.LastName = newUser.LastName;
            response.Email = newUser.Email ?? "";
            response.IsVerified = newUser.EmailConfirmed;
            response.Roles = rolesList.ToList();

            return response;

        }
        public async Task<EditResponseDto> EditUser(SaveUserDto saveDto, string origin, bool? isCreated = false) 
        {
            bool isNotCreated = !isCreated ?? false;

            EditResponseDto response = new()
            {
                Id = "",
                UserName = "",
                FirstName = "",
                LastName = "",
                Email = "",
                HasError = false,
                Errors = []
            };

            var userWithSameUserName = await _userManager.FindByNameAsync(saveDto.UserName);
            if (userWithSameUserName != null && userWithSameUserName.Id != saveDto.Id)
            {
                response.HasError = true;
                response.Errors.Add($"Este nombre de usuario ya esta ocupado. {saveDto.UserName}.");
                return response;
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(saveDto.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != saveDto.Id)
            {
                response.HasError = true;
                response.Errors.Add($"Este correo ya esta ocupado. {saveDto.Email}.");
                return response;
            }


            var user = await _userManager.FindByIdAsync(saveDto.Id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No hay una cuenta registrada para este usuario.");
                return response;
            }

            user.UserName = saveDto.UserName;
            user.FirstName = saveDto.FirstName;
            user.LastName = saveDto.LastName;
            user.EmailConfirmed = user.EmailConfirmed && user.Email == saveDto.Email; // si cambia el email, se debe reconfirmar
            user.Email = saveDto.Email;
            user.Phone = saveDto.Phone;
            user.ProfilePicturePath = string.IsNullOrWhiteSpace(saveDto.ProfilePicturePath) ? user.ProfilePicturePath : saveDto.ProfilePicturePath;
            user.IsActive = saveDto.IsActive;

            // Actualizar contrasenia si se proporciona una nueva

            if (!string.IsNullOrWhiteSpace(saveDto.Password) && isNotCreated)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultChange = await _userManager.ResetPasswordAsync(user, token, saveDto.Password);

                if (resultChange != null && !resultChange.Succeeded)
                {
                    response.HasError = true;
                    response.Errors.AddRange(resultChange.Errors.Select(s => s.Description).ToList());
                    return response;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(s => s.Description).ToList());
                return response;
            }

            var rolesList = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesList.ToList());

            await _userManager.AddToRoleAsync(user, saveDto.Role);

            if (!user.EmailConfirmed && isNotCreated) 
            {
                string verificationUri = await GetVerificationEmailUri(user, origin);
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    ToEmail = saveDto.Email,
                    Subject = "Confirma tu cuenta de LinkUpApp",
                    HtmlBody = $"Hola {user.FirstName}, por favor confirma tu cuenta haciendo clic en el siguiente enlace: <a href='{verificationUri}'>Confirmar Cuenta</a>"
                });
            }

            var updatedRolesList = await _userManager.GetRolesAsync(user);

            response.Id = user.Id;
            response.UserName = user.UserName;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.Email = user.Email ?? "";
            response.IsVerified = user.EmailConfirmed;
            response.Roles = updatedRolesList.ToList();

            return response;

        }
        public async Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            UserResponseDto response = new()
            {
                HasError = false,
                Errors = []
            };

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No hay una cuenta registrada para este usuario. {request.UserName}");
                return response;
            }

            // Desactivar usuario temporalmente
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            var resetUri = await GetResetPasswordUri(user, request.Origin);

            await _emailService.SendAsync(new EmailRequestDto()
            {
                ToEmail = user.Email,
                Subject = "Restablecer contraseña",
                HtmlBody = $"Hola {user.FirstName}, por favor confirma tu cuenta haciendo clic en el siguiente enlace: {resetUri}"
            });


            return response;
        }

        public async Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            UserResponseDto response = new()
            {
                HasError = false,
                Errors = []
            };

            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No hay una cuenta registrada para este usuario.");
                return response;
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(result.Errors.Select(s => s.Description).ToList());
                return response;
            }

            // Al completar el cambio de contrasenia, activar usuario
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return response;
        }

        public async Task<UserResponseDto> DeleteAsync(string id)
        {
            UserResponseDto response = new()
            {
                HasError = false,
                Errors = []
            };

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"No hay una cuenta registrada para este usuario.");
                return response;
            }

            await _userManager.DeleteAsync(user);
            return response;
        }

        public async Task<UserDto?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                Phone = user.Phone,
                ProfilePicturePath = user.ProfilePicturePath,
                Role = rolesList.FirstOrDefault() ?? "",
                IsVerified = user.EmailConfirmed,
                IsActive = user.IsActive,
            };

            return userDto;

        }
        public async Task<UserDto?> GetUserByUsername(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                Phone = user.Phone,
                ProfilePicturePath = user.ProfilePicturePath,
                Role = rolesList.FirstOrDefault() ?? "",
                IsVerified = user.EmailConfirmed,
                IsActive = user.IsActive,
            };

            return userDto;

        }

        public async Task<UserDto?> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return null;

            var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                Phone = user.Phone,
                ProfilePicturePath = user.ProfilePicturePath,
                Role = rolesList.FirstOrDefault() ?? "",
                IsVerified = user.EmailConfirmed,
                IsActive = user.IsActive,
            };

            return userDto;

        }

        public async Task<List<UserDto>> GetAllUser(bool? isActive = true)
        {
            var listUsersDto = new List<UserDto>();
            var users = _userManager.Users;

            if (isActive != null && isActive == true)
            {
                users = users.Where(w => w.EmailConfirmed);
            }

            var listUser = await users.ToListAsync();

            foreach (var item in listUser)
            {
                var roleList = await _userManager.GetRolesAsync(item);

                listUsersDto.Add(new UserDto()
                {
                    Id = item.Id,
                    UserName = item.UserName ?? "",
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email ?? "",
                    Phone = item.Phone,
                    ProfilePicturePath = item.ProfilePicturePath,
                    Role = roleList.FirstOrDefault() ?? "",
                    IsVerified = item.EmailConfirmed,
                    IsActive = item.IsActive,
                });

            }

            return listUsersDto;

        }

        public async Task<string> ConfirmAccountAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return "Este usuario no esta registrado.";
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token); // invalid token

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return $"Ha ocurrido un error a la hora de confirmar este email {user.Email}.";
            }

            return $"Cuenta confirmada para {user.Email}. Ya puedes usar la app!";
        }

        #region Private Methods
        private async Task<string> GetVerificationEmailUri(ApplicationUser user, string origin)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var route = "Login/ConfirmEmail";
            var completeUrl = new Uri(string.Concat(origin, "/", route)); // origin = https://localhost:5628 

            var verificationUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri.ToString(), "token", token);

            return verificationUri;
        }

        private async Task<string> GetResetPasswordUri(ApplicationUser user, string origin)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var route = "Login/ResetPassword";
            var completeUrl = new Uri(string.Concat(origin, "/", route)); // origin = https://localhost:5628 
            var resetUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);

            resetUri = QueryHelpers.AddQueryString(resetUri, "token", token);
            return resetUri;
        }

        #endregion
    }
}   
