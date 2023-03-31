using System.Threading.Tasks;
using Fluxera.Extensions.Hosting;

namespace Vending.App;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await ApplicationHost.RunAsync<AppHost>(args);
    }
}
