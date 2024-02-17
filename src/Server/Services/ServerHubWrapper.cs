using CleanArchitecture.Blazor.Server.Common.Interfaces;
using CleanArchitecture.Blazor.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Blazor.Server.Services;

public class ServerHubWrapper : IApplicationHubWrapper
{
    //public sealed class HubClient : IAsyncDisposable -client sends/each click request to server
    //public class ServerHub : Hub<ISignalRHub> -server which recives each request 
    //ServerHubWrapper is another linked entity where signlaR configuration exists.For detailed errors enable here only
    //UI browser to server call happens throgh these only & in server side recieved & process happens always
    private readonly IHubContext<ServerHub, ISignalRHub> _hubContext;

    public ServerHubWrapper(IHubContext<ServerHub, ISignalRHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task JobStarted(string message)
    {
        await _hubContext.Clients.All.Start(message);
    }

    public async Task JobCompleted(string message)
    {
        await _hubContext.Clients.All.Completed(message);
    }
}