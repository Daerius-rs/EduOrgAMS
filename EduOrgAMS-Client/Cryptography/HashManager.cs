using System;
using System.IO;
using EduOrgAMS.Client.Settings;
using RIS.Cryptography.Hash;
using RIS.Cryptography.Hash.Methods;
using Environment = RIS.Environment;

namespace EduOrgAMS.Client.Cryptography
{
    public static class HashManager
    {
        private static Argon2idWP Argon2idProvider { get; }
        private static SHA512iCSP SHA512Provider { get; }

        public static HashService Service { get; }

        static HashManager()
        {
            Argon2idProvider = new Argon2idWP
            {
                KnownSecret = "EduOrgAMS.Client"
            };
            SHA512Provider = new SHA512iCSP();
            Service = new HashService(SHA512Provider);
        }



        public static string GetLibrariesHash()
        {
            return Service.GetDirectoryHash(Environment.ExecAppDirectoryName,false, new[]
            {
                "localizations",
                "downloads",
                "storage",
                "temp",
                "logs",
                "hash",
                AppSettings.SettingsFileName,
                PersistentSettings.SettingsFileName,
                Path.ChangeExtension(Path.GetFileName(Environment.ExecProcessFilePath), "exe"),
                Path.ChangeExtension(Path.GetFileName(Environment.ExecProcessFilePath), "pdb"),
                //dev
                "publish",
                "win-x64",
                "win-x86"
            });
        }

        public static string GetExeHash()
        {
            return Service.GetFileHash(Path.ChangeExtension(
                Environment.ExecProcessFilePath, "exe"));
        }

        public static string GetExePdbHash()
        {
            return Service.GetFileHash(Path.ChangeExtension(
                Environment.ExecProcessFilePath, "pdb"));
        }

        public static string GetPasswordHash(string login,
            string password)
        {
            return Argon2idProvider.GetHash(password,
                1 * 1024 * 64, 2, 1, login);
        }



        public static bool VerifyPasswordHash(string login,
            string password, string hash)
        {
            return Argon2idProvider.VerifyHash(
                password, hash, login);
        }
    }
}
