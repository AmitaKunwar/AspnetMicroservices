using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postreSQL database");

                    using var connection = new NpgsqlConnection
                            (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    { Connection = connection };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon (Id SERIAL PRIMARY KEY,
                                                                                                                            ProductName VARCHAR(24) NOT NULL,
                                                                                                                            Description TEXT,
                                                                                                                            Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES  ('IPHONE 10','Iphone 10 description', 100.00 )";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES  ('Samsung Galaxy','Samsung description', 980.00 )";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postreSQL database");

                }
                catch (NpgsqlException e)
                {
                    logger.LogError(e, "An error occurred while migrating the postresql database ");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);

                    }
                }
               
            }
            return host;
        }
    }
}
