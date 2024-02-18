using CleanArchitecture.Blazor.Infrastructure.Constants;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Server.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Components.Shared;

public interface ILogOut
{
    Task Execute(string? message = null, bool force = false);
}

public class LogOut(IDialogService dialogService, IAccessTokenProvider tokenProvider, AuthenticationStateProvider authenticationStateProvider) : ILogOut
{
    public async Task Execute(string? message = null, bool force = false)
    {
        var confirmed = force;

        var parameters = new DialogParameters<LogoutConfirmation>
        {
            { x => x.ContentText,force?"After changes you must logout to see changes": $"{ConstantString.LogoutConfirmation},{message??""}" },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = dialogService.Show<LogoutConfirmation>(ConstantString.LogoutNecessaryConfirmationTitle, parameters, options);
        var result = await dialog.Result;
        confirmed = force||!result.Canceled;

        if (confirmed)
        {
            await tokenProvider.RemoveAuthDataFromStorage();
            ((BlazorAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
