using CleanArchitecture.Blazor.Server.UI.Pages.Identity.Authentication;
using Microsoft.JSInterop;

namespace CleanArchitecture.Blazor.Server.UI.Services.JsInterop;

public sealed class ExternalAuth
{
    private readonly IJSRuntime _jsRuntime;

    public ExternalAuth(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> Login(string provider, DotNetObjectReference<Login> reference)
    {
        try
        {
            var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/externalauth.js");
            return jsmodule.InvokeVoidAsync(JSInteropConstants.ExternalLogin, provider, reference);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
    }
    public async Task<ValueTask> Logout()
    {
        try
        {
            var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/externalauth.js");
            return jsmodule.InvokeVoidAsync(JSInteropConstants.ExternalLogout);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
    }
}