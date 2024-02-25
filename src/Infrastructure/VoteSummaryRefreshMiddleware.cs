using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using CleanArchitecture.Blazor.Infrastructure.Services.Vote;
using Microsoft.AspNetCore.Http;
using CleanArchitecture.Blazor.Infrastructure.Services.VotingSystem;

namespace CleanArchitecture.Blazor.Infrastructure;
public class VoteSummaryRefreshMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context ,IVoteSummaryService voteSummaryService)
    {
        try
        {
            //  await _lock.WaitAsync(); // Acquire the lock

            // Load data asynchronously using services
            //await LoadDataAsync();
            await voteSummaryService.RefreshSummary();

            // Continue processing the request pipeline
            await _next(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            //_lock.Release();
        }
        finally
        {
            // _lock.Release(); // Release the lock
        }
    }

    //private async Task LoadDataAsync()
    //{
    //    // Resolve services using the service provider

    //    // Example: Call a method on your service
    //    await _voteSummaryService.RefreshSummary();
    //}
}

