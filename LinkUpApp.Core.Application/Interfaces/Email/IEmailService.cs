using LinkUpApp.Core.Application.Dtos.Email;

namespace LinkUpApp.Core.Application.Interfaces.Email
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}
