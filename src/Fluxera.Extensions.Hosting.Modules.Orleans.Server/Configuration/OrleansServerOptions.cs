using System.Net;
using JetBrains.Annotations;

namespace Fluxera.Extensions.Hosting.Modules.Orleans;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class OrleansServerOptions
{
    /// <summary>
    ///     A unique identifier for this service, which should survive deployment and redeployment, where as <see cref="ClusterId" /> might not.
    /// </summary>
    public string ServiceId { get; set; } = "eMachine";

    /// <summary>
    ///     The cluster identity. This used to be called DeploymentId before Orleans 2.0 name.
    /// </summary>
    public string ClusterId { get; set; } = "eMachine-default";

    /// <summary>
    ///     The IP address used for clustering.
    /// </summary>
    public IPAddress AdvertisedIPAddress { get; set; } = IPAddress.Loopback;

    /// <summary>
    ///     The port this silo uses for silo-to-silo communication.
    /// </summary>
    public int SiloPort { get; set; } = 11111;

    /// <summary>
    ///     The port this silo uses for silo-to-client (gateway) communication. Specify 0 to disable gateway functionality.
    /// </summary>
    public int GatewayPort { get; set; } = 30000;

    /// <summary>
    ///     The endpoint used to listen for silo to silo communication.
    ///     If not set will default to <see cref="AdvertisedIPAddress" /> + <see cref="SiloPort" />
    /// </summary>
    public IPEndPoint? SiloListeningEndpoint { get; set; }

    /// <summary>
    ///     The endpoint used to listen for client to silo communication.
    ///     If not set will default to <see cref="AdvertisedIPAddress" /> + <see cref="GatewayPort" />
    /// </summary>
    public IPEndPoint? GatewayListeningEndpoint { get; set; }

    /// <summary>
    ///     The silo name.
    /// </summary>
    public string? SiloName { get; set; }
}
