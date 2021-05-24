using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Storage
{
    public class StorageContext : DbContext
    {
        public StorageContext()
        {
            if (!Directory.Exists(Path.GetDirectoryName(StorageManager.StorageFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(StorageManager.StorageFilePath));

            if (!File.Exists(StorageManager.StorageFilePath))
                File.Create(StorageManager.StorageFilePath).Close();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={StorageManager.StorageFilePath}");

            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), type =>
            {
                if (string.IsNullOrEmpty(type.Namespace))
                    return false;

                return type.Namespace.StartsWith(
                    GetType().Namespace ?? string.Empty);
            });
        }
    }
}
