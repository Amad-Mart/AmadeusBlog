using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace AmadeusBlog.Data
{
    public class Connection
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var dataBaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            return string
                .IsNullOrEmpty(dataBaseUrl) ? 
                connectionString : 
                BuildConnectionString(dataBaseUrl);
        }

        static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(":");

            return new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            }.ToString();
        }
    }
}
