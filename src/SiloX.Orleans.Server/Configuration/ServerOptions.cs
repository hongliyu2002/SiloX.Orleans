﻿using JetBrains.Annotations;

namespace SiloX.Orleans;

/// <summary>
/// </summary>
[PublicAPI]
public sealed class ServerOptions
{
    /// <summary>
    ///     The IP address used for clustering.
    /// </summary>
    public string AdvertisedIPAddress { get; set; } = "127.0.0.1";

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
    public string? SiloListeningEndpoint { get; set; }

    /// <summary>
    ///     The endpoint used to listen for client to silo communication.
    ///     If not set will default to <see cref="AdvertisedIPAddress" /> + <see cref="GatewayPort" />
    /// </summary>
    public string? GatewayListeningEndpoint { get; set; }

    /// <summary>
    ///     The silo name.
    /// </summary>
    public string? SiloName { get; set; }
}
