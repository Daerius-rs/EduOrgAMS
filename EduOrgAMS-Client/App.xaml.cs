using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EduOrgAMS.Client.Cryptography;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Dialogs;
using EduOrgAMS.Client.Logging;
using EduOrgAMS.Client.Native;
using EduOrgAMS.Client.Navigation;
using EduOrgAMS.Client.Pages;
using EduOrgAMS.Client.Protocols;
using EduOrgAMS.Client.Settings;
using EduOrgAMS.Client.Storage;
using EduOrgAMS.Client.Utils;
using RIS;
using RIS.Cryptography;
using RIS.Wrappers;
using Environment = RIS.Environment;

namespace EduOrgAMS.Client
{
    public partial class App : Application
    {
        private const string UniqueName = "App+EduOrgAMS+Client+{26774ADB-E36E-4DE0-813B-961E66A86759}";

        private static readonly object InstanceSyncRoot = new object();
        private static volatile SingleInstanceApp _instance;
        internal static SingleInstanceApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceSyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new SingleInstanceApp(UniqueName);
                        }
                    }
                }

                return _instance;
            }
        }

        private static string AppStartupUri { get; set; }
        private static bool CreateHashFiles { get; set; }

        [STAThread]
        private static void Main(string[] args)
        {
#pragma warning disable SS002 // DateTime.Now was referenced
            NLog.GlobalDiagnosticsContext.Set("AppStartupTime", DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss", CultureInfo.InvariantCulture));
#pragma warning restore SS002 // DateTime.Now was referenced

            ParseArgs(args);

            Instance.Run(() =>
            {
                SingleInstanceMain();
            });
        }

        private static void SingleInstanceMain()
        {
            LogManager.Log.Info("App Run");

            Events.Information += OnInformation;
            Events.Warning += OnWarning;
            Events.Error += OnError;

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.TypeResolve += OnResolve;
            AppDomain.CurrentDomain.ResourceResolve += OnResolve;

            App app = new App();

            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

            LogManager.Log.Info($"Unique Name - {UniqueName}");
            LogManager.Log.Info($"Libraries Directory - {Environment.ExecAppDirectoryName}");
            LogManager.Log.Info($"Execution File Directory - {Environment.ExecProcessDirectoryName}");
            LogManager.Log.Info($"Is Standalone App - {Environment.IsStandalone}");
            LogManager.Log.Info($"Is Single File App - {Environment.IsSingleFile}");
            LogManager.Log.Info($"Runtime Name - {Environment.RuntimeName}");
            LogManager.Log.Info($"Runtime Version - {Environment.RuntimeVersion}");
            LogManager.Log.Info($"Runtime Identifier - {Environment.RuntimeIdentifier}");

            string librariesHash = HashManager.GetLibrariesHash();
            string exeHash = HashManager.GetExeHash();
            string exePdbHash = HashManager.GetExePdbHash();

            LogManager.Log.Info($"Libraries SHA512 - {librariesHash}");
            LogManager.Log.Info($"Exe SHA512 - {exeHash}");
            LogManager.Log.Info($"Exe PDB SHA512 - {exePdbHash}");

            if (CreateHashFiles)
            {
                const string hashType = "sha512";

                LogManager.Log.Info($"Hash file creation started - {hashType}");

                CreateHashFile(librariesHash, "LibrariesHash", hashType);
                CreateHashFile(exeHash, "ExeHash", hashType);
                CreateHashFile(exePdbHash, "ExePdbHash", hashType);

                LogManager.Log.Info($"Hash files creation completed - {hashType}");

                Current.Shutdown(0x0);
                return;
            }

            app.InitializeComponent();
            app.Run(EduOrgAMS.Client.MainWindow.Instance);
        }

        private static void ParseArgs(string[] args)
        {
            var wrapper = UnwrapArgs(args);

            foreach (var argEntry in wrapper.Enumerate())
            {
                switch (argEntry.Key)
                {
                    case "startupUri":
                        AppStartupUri = (string)argEntry.Value;
                        break;
                    case "createHashFiles":
                        CreateHashFiles = bool.Parse((string)argEntry.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void CreateHashFile(string hash, string hashName, string hashType)
        {
            string currentAppVersion = FileVersionInfo
                .GetVersionInfo(Environment.ExecAppAssemblyFilePath)
                .ProductVersion;
            string hashFileNameWithoutExtension = $"{Environment.ExecAppAssemblyFileNameWithoutExtension.Replace('.', '_')}." +
                                                  $"v{currentAppVersion.Replace('.', '_')}." +
                                                  $"{Environment.RuntimeIdentifier.Replace('.', '_')}." +
                                                  $"{(!Environment.IsStandalone ? "!" : string.Empty)}{nameof(Environment.IsStandalone)}." +
                                                  $"{(!Environment.IsSingleFile ? "!" : string.Empty)}{nameof(Environment.IsSingleFile)}";
            string hashFileDirectoryName =
                Path.Combine(Environment.ExecProcessDirectoryName ?? string.Empty, "hash", hashType);

            if (!Directory.Exists(hashFileDirectoryName))
                Directory.CreateDirectory(hashFileDirectoryName);

            using (var file = new StreamWriter(
                Path.Combine(hashFileDirectoryName, $"{hashName}.{hashFileNameWithoutExtension}.{hashType}"),
                false, SecureUtils.SecureUTF8))
            {
                file.WriteLine(hash);
            }
        }

        public static ArgsKeyedWrapper UnwrapArgs(string[] args)
        {
            var argsEntries = new List<(string Key, object Value)>(args.Length);

            foreach (var arg in args)
            {
                if (string.IsNullOrWhiteSpace(arg))
                    continue;

                int separatorPosition = arg.IndexOf(':');

                if (separatorPosition == -1)
                {
                    argsEntries.Add(
                        (arg.Trim(' ', '\'', '\"'),
                        string.Empty));

                    continue;
                }

                argsEntries.Add(
                    (arg.Substring(0, separatorPosition).Trim(' ', '\'', '\"'),
                    arg.Substring(separatorPosition + 1).Trim(' ', '\'', '\"')));
            }

            return new ArgsKeyedWrapper(argsEntries);
        }



#pragma warning disable SS001 // Async methods should return a Task to make them awaitable
        protected override async void OnStartup(StartupEventArgs e)
        {
            await EduOrgAMS.Client.MainWindow.Instance.ShowLoadingGrid(true)
                .ConfigureAwait(true);

            MainWindow = EduOrgAMS.Client.MainWindow.Instance;

            await Task.Delay(TimeSpan.FromMilliseconds(200))
                .ConfigureAwait(true);

            EduOrgAMS.Client.MainWindow.Instance.Show();

            base.OnStartup(e);

            await Task.Delay(TimeSpan.FromMilliseconds(500))
                .ConfigureAwait(true);

            if (EduOrgAMS.Client.MainWindow.Instance.Locales.Count == 0)
            {
                var message = LocalizationUtils.TryGetLocalized("NoLocalizationsFoundMessage")
                              ?? "No localizations found";

                await DialogManager.ShowErrorDialog(message)
                    .ConfigureAwait(true);

                Current.Shutdown(0x1);

                return;
            }

            await StorageManager.Initialize()
                .ConfigureAwait(true);
            await DatabaseManager.Initialize(
                    "eoams", 
                    "EOAMS-[Client]", "NDz3L25EkIT9mv7",
                    IPAddress.Parse("127.0.0.1"), 3306)
                .ConfigureAwait(true);

            ProtocolManager.RegisterAll();

            await Task.Run(async () =>
            {
                LogManager.Log.Info("Deleted older logs - " +
                                    $"{LogManager.DeleteLogs(Path.Combine(Environment.ExecProcessDirectoryName, "logs"), SettingsManager.AppSettings.LogRetentionDaysPeriod)}");
                LogManager.Log.Info("Deleted older debug logs - " +
                                    $"{LogManager.DeleteLogs(Path.Combine(Environment.ExecProcessDirectoryName, "logs", "debug"), SettingsManager.AppSettings.LogRetentionDaysPeriod)}");

                try
                {
                    if (string.IsNullOrEmpty(
                        SettingsManager.PersistentSettings.GetCurrentUserLogin()))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            NavigationController.Instance.RequestPage<LoginPage>();
                        });

                        return;
                    }

                    if (!SettingsManager.PersistentSettings.SetCurrentUser(
                        SettingsManager.PersistentSettings.GetCurrentUserLogin()))
                    {
                        SettingsManager.PersistentSettings.RemoveUser(
                            SettingsManager.PersistentSettings.GetCurrentUserLogin());

                        Dispatcher.Invoke(() =>
                        {
                            NavigationController.Instance.RequestPage<LoginPage>();
                        });

                        return;
                    }

                    var authInfo = await DatabaseHelper.Login(
                            SettingsManager.PersistentSettings.CurrentUser.Token)
                        .ConfigureAwait(false);

                    if (!authInfo.Success)
                    {
                        SettingsManager.PersistentSettings.RemoveUser(
                            SettingsManager.PersistentSettings.CurrentUser.Login);

                        Dispatcher.Invoke(() =>
                        {
                            NavigationController.Instance.RequestPage<LoginPage>();
                        });

                        return;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        NavigationController.Instance.RequestPage<MainPage>();
                    });

                    if (!string.IsNullOrEmpty(AppStartupUri))
                        ProtocolManager.ParseUri(AppStartupUri);
                }
                catch (Exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        NavigationController.Instance.RequestPage<LoginPage>();
                    });
                }
            }).ConfigureAwait(true);

            await Task.Delay(TimeSpan.FromSeconds(1.5))
                .ConfigureAwait(true);

            await EduOrgAMS.Client.MainWindow.Instance.ShowLoadingGrid(false)
                .ConfigureAwait(true);
        }
#pragma warning restore SS001 // Async methods should return a Task to make them awaitable

        protected override void OnExit(ExitEventArgs e)
        {
            SettingsManager.AppSettings.Save();

            LogManager.Log.Info($"App Exit Code - {e.ApplicationExitCode}");
            NLog.LogManager.Shutdown();

            base.OnExit(e);
        }



        private static void OnInformation(object sender, RInformationEventArgs e)
        {
            LogManager.DebugLog.Info($"{e.Message}");
        }

        private static void OnWarning(object sender, RWarningEventArgs e)
        {
            LogManager.Log.Warn($"{e.Message}");
        }

        private static void OnError(object sender, RErrorEventArgs e)
        {
            LogManager.Log.Error($"{e.SourceException?.GetType().Name ?? "Unknown"} - Message={e.Message ?? "Unknown"},HResult={e.SourceException?.HResult.ToString() ?? "Unknown"},StackTrace=\n{e.SourceException?.StackTrace ?? "Unknown"}");
        }



        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = null;

            if (e.ExceptionObject is Exception exceptionObject)
                exception = exceptionObject;

            LogManager.Log.Fatal($"{exception?.GetType().Name ?? "Unknown"} - Message={exception?.Message ?? "Unknown"},HResult={exception?.HResult.ToString() ?? "Unknown"},StackTrace=\n{exception?.StackTrace ?? "Unknown"}");
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogManager.Log.Fatal($"{e.Exception?.GetType().Name ?? "Unknown"} - Message={e.Exception?.Message ?? "Unknown"},HResult={e.Exception?.HResult.ToString() ?? "Unknown"},StackTrace=\n{e.Exception?.StackTrace ?? "Unknown"}");
        }

        private static void OnFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            LogManager.DebugLog.Error($"{e.Exception.GetType().Name} - Message={e.Exception.Message},HResult={e.Exception.HResult},StackTrace=\n{e.Exception.StackTrace ?? "Unknown"}");
        }



        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs e)
        {
            LogManager.DebugLog.Info($"Resolve - Name={e.Name ?? "Unknown"},RequestingAssembly={e.RequestingAssembly?.FullName ?? "Unknown"}");

            return e.RequestingAssembly;
        }

        private static Assembly OnResolve(object sender, ResolveEventArgs e)
        {
            LogManager.DebugLog.Info($"Resolve - Name={e.Name ?? "Unknown"},RequestingAssembly={e.RequestingAssembly?.FullName ?? "Unknown"}");

            return e.RequestingAssembly;
        }
    }
}
