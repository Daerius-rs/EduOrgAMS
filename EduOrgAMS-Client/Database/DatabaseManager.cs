using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace EduOrgAMS.Client.Database
{
    public static class DatabaseManager
    {
        private static MySqlConnectionStringBuilder ConnectionStringBuilder { get; set; }
        private static bool IsInitialized { get; set; }

        public static ServerVersion ServerVersion
        {
            get
            {
                return ServerVersion.AutoDetect(
                    ConnectionString);
            }
        }
        public static string ConnectionString
        {
            get
            {
                if (ConnectionStringBuilder == null)
                {
                    ConnectionStringBuilder = new MySqlConnectionStringBuilder
                    {
                        Server = "127.0.0.1",
                        Port = 3306,
                        Database = "eoams",
                        UserID = "EOAMS-[Client]",
                        Password = "NDz3L25EkIT9mv7",
                        LoadBalance = MySqlLoadBalance.LeastConnections,
                        Pooling = true,
                        MinimumPoolSize = 5,
                        MaximumPoolSize = 200,
                        ConnectionLifeTime = (uint)TimeSpan.FromMinutes(5).TotalSeconds,
                        ConnectionIdleTimeout = (uint)TimeSpan.FromSeconds(60).TotalSeconds,
                        DefaultCommandTimeout = (uint)TimeSpan.FromSeconds(120).TotalSeconds,
                        ConnectionTimeout = (uint)TimeSpan.FromSeconds(60).TotalSeconds,
                        CharacterSet = CharSet.Utf8Mb4.Name
                    };
                }

                return ConnectionStringBuilder.ConnectionString;
            }
        }



        static DatabaseManager()
        {
            IsInitialized = false;
        }

        public static async Task Initialize(
            string database, string login, string password,
            IPAddress address, ushort port = 3306,
            TimeSpan? commandTimeout = null, TimeSpan? connectionTimeout = null,
            TimeSpan? connectionLifetime = null, TimeSpan? connectionIdle = null,
            CharSet charSet = null,
            MySqlLoadBalance loadBalance = MySqlLoadBalance.LeastConnections)
        {
            if (IsInitialized)
                return;

            if (address == null
                || address.Equals(IPAddress.None)
                || address.Equals(IPAddress.Any))
            {
                address = IPAddress.Parse("127.0.0.1");
            }

            commandTimeout ??= TimeSpan.FromSeconds(120);
            connectionTimeout ??= TimeSpan.FromSeconds(60);
            connectionLifetime ??= TimeSpan.FromMinutes(5);
            connectionIdle ??= TimeSpan.FromSeconds(60);
            charSet ??= CharSet.Utf8Mb4;

            ConnectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = address.ToString(),
                Port = port,
                Database = database,
                UserID = login,
                Password = password,
                LoadBalance = loadBalance,
                Pooling = true,
                MinimumPoolSize = 5,
                MaximumPoolSize = 200,
                ConnectionLifeTime = (uint)connectionLifetime.Value.TotalSeconds,
                ConnectionIdleTimeout = (uint)connectionIdle.Value.TotalSeconds,
                DefaultCommandTimeout = (uint)commandTimeout.Value.TotalSeconds,
                ConnectionTimeout = (uint)connectionTimeout.Value.TotalSeconds,
                CharacterSet = charSet.Name
            };

            await using var context = CreateContext();

            await context.Database.MigrateAsync()
                .ConfigureAwait(false);

            context.Database.SetCommandTimeout(
                (int)commandTimeout.Value.TotalSeconds);

            IsInitialized = true;
        }

        public static DatabaseContext CreateContext()
        {
            return new DatabaseContext();
        }



        public static TEntity Find<TEntity>(
            params object[] keyValues)
            where TEntity : class
        {
            using var context = CreateContext();

            return context.Find<TEntity>(
                keyValues);
        }
        public static async ValueTask<TEntity> FindAsync<TEntity>(
            params object[] keyValues)
            where TEntity: class
        {
            await using var context = CreateContext();

            return await context.FindAsync<TEntity>(
                    keyValues)
                .ConfigureAwait(false);
        }

        public static EntityEntry<TEntity> Add<TEntity>(
            TEntity entity)
            where TEntity : class
        {
            using var context = CreateContext();

            var result = context.Add<TEntity>(
                entity);

            SaveChanges(context);

            return result;
        }
        public static async ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(
            TEntity entity)
            where TEntity : class
        {
            await using var context = CreateContext();

            var result = await context.AddAsync<TEntity>(
                    entity)
                .ConfigureAwait(false);

            await SaveChangesAsync(context)
                .ConfigureAwait(false);

            return result;
        }

        public static EntityEntry<TEntity> Update<TEntity>(
            TEntity entity)
            where TEntity : class
        {
            using var context = CreateContext();

            var result = context.Update<TEntity>(
                entity);

            SaveChanges(context);

            return result;
        }

        public static EntityEntry<TEntity> Remove<TEntity>(
            TEntity entity)
            where TEntity : class
        {
            using var context = CreateContext();

            var result = context.Remove<TEntity>(
                entity);

            SaveChanges(context);

            return result;
        }

        public static int SaveChanges(DatabaseContext context,
            bool acceptAllChangesOnSuccess = true)
        {
            return context.SaveChanges(
                acceptAllChangesOnSuccess);
        }
        public static Task<int> SaveChangesAsync(DatabaseContext context,
            bool acceptAllChangesOnSuccess = true)
        {
            return context.SaveChangesAsync(
                acceptAllChangesOnSuccess);
        }
    }
}
