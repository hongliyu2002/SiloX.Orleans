namespace SiloX.Orleans.Clustering.AdoNet;

/// <summary>
/// </summary>
public static class AdoNetInvariants
{
    /// <summary>
    ///     Microsoft SQL Server invariant name string.
    /// </summary>
    public const string InvariantNameSqlServer = "System.Data.SqlClient";
    /// <summary>
    ///     Oracle Database server invariant name string.
    /// </summary>
    public const string InvariantNameOracleDatabase = "Oracle.DataAccess.Client";
    /// <summary>
    ///     SQLite invariant name string.
    /// </summary>
    public const string InvariantNameSqlLite = "System.Data.SQLite";
    /// <summary>
    ///     MySql invariant name string.
    /// </summary>
    public const string InvariantNameMySql = "MySql.Data.MySqlClient";
    /// <summary>
    ///     PostgreSQL invariant name string.
    /// </summary>
    public const string InvariantNamePostgreSql = "Npgsql";
    /// <summary>
    ///     Dotnet core Microsoft SQL Server invariant name string.
    /// </summary>
    public const string InvariantNameSqlServerDotnetCore = "Microsoft.Data.SqlClient";
    /// <summary>
    ///     An open source implementation of the MySQL connector library.
    /// </summary>
    public const string InvariantNameMySqlConnector = "MySql.Data.MySqlConnector";
}
