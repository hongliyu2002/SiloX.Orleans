using System.Threading.Tasks;
using Fluxera.Extensions.Hosting;

namespace Vending.Apps.Wpf;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await ApplicationHost.RunAsync<AppHost>(args);
    }
}
