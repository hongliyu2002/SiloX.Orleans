namespace SiloX.Orleans.Persistence.AdoNet;

/// <summary>
///     This enumeration represents the supported ADO.NET database providers.
/// </summary>
public enum AdoNetDbProvider
{
    /// <summary>
    ///     Indicates that the SQL Server database provider is used.
    /// </summary>
    SQLServer,
    /// <summary>
    ///     Indicates that the PostgreSQL database provider is used.
    /// </summary>
    PostgreSQL,

    /// <summary>
    ///     Indicates that the MySQL database provider is used.
    /// </summary>
    MySQL,
    /// <summary>
    ///     Indicates that the Oracle database provider is used.
    /// </summary>
    Oracle
}
