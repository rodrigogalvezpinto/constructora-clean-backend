using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace ConstructoraClean.Infrastructure.Data;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
        
        _connectionString = configuration["ConnectionStrings:DefaultConnection"] 
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión DefaultConnection");
    }

    public virtual IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
} 