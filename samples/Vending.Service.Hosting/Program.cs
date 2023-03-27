using Fluxera.Extensions.Hosting;
using Vending.Hosting;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await ApplicationHost.RunAsync<ServiceHost>(args);
    }
}
