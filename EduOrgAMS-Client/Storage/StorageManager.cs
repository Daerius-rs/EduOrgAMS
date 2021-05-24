using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Environment = RIS.Environment;

namespace EduOrgAMS.Client.Storage
{
    public static class StorageManager
    {
        private static StorageContext Context { get; set; }
        private static bool IsInitialized { get; set; }

        public static string StoragePath { get; }
        public static string StorageFilePath { get; }

        static StorageManager()
        {
            StoragePath = Path.Combine(Environment.ExecProcessDirectoryName,
                "storage");
            StorageFilePath = Path.Combine(StoragePath, "storage.db");

            IsInitialized = false;
        }

        public static async Task Initialize()
        {
            if (IsInitialized)
                return;

            Context = new StorageContext();

            await Context.Database.MigrateAsync()
                .ConfigureAwait(true);

            Context.Database.SetCommandTimeout(TimeSpan.FromSeconds(60));

            IsInitialized = true;
        }
    }
}
