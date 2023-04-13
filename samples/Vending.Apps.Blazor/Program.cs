using Fluxera.Extensions.Hosting;

namespace Vending.Apps.Blazor;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        await ApplicationHost.RunAsync<AppHost>(args);
    }
}