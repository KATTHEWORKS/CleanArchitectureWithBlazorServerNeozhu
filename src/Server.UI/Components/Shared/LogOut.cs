using CleanArchitecture.Blazor.Infrastructure.Constants;
using CleanArchitecture.Blazor.Infrastructure.Services.JWT;
using CleanArchitecture.Blazor.Server.UI.Components.Dialogs;
using CleanArchitecture.Blazor.Server.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace CleanArchitecture.Blazor.Server.UI.Components.Shared;

public interface ILogOut
{
    Task Execute();
}

public class LogOut(IDialogService dialogService, IAccessTokenProvider tokenProvider, AuthenticationStateProvider authenticationStateProvider) : ILogOut
{
    public async Task Execute()
    {
        var parameters = new DialogParameters<LogoutConfirmation>
        {
            { x => x.ContentText, $"{ConstantString.LogoutConfirmation}" },
            { x => x.Color, Color.Error }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = dialogService.Show<LogoutConfirmation>(ConstantString.LogoutNecessaryConfirmationTitle, parameters, options);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            await tokenProvider.RemoveAuthDataFromStorage();
            ((BlazorAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
