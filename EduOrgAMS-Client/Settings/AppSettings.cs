using System;
using System.IO;
using System.Windows;
using RIS.Settings;
using RIS.Settings.Ini;
using Environment = RIS.Environment;

namespace EduOrgAMS.Client.Settings
{
    public class AppSettings : IniSettings
    {
        public const string SettingsFileName = "AppSettings.config";

        [SettingCategory("Localization")]
        public string Language { get; set; }
        [SettingCategory("Window")]
        public double WindowPositionX { get; set; }
        public double WindowPositionY { get; set; }
        public int WindowState { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        [SettingCategory("Log")]
        public int LogRetentionDaysPeriod { get; set; }

        public AppSettings()
            : base(Path.Combine(Environment.ExecProcessDirectoryName, SettingsFileName))
        {
            Language = "en-US";
            WindowPositionX = SystemParameters.PrimaryScreenWidth / 2.0;
            WindowPositionY = SystemParameters.PrimaryScreenHeight / 2.0;
            WindowState = (int)System.Windows.WindowState.Normal;
            WindowWidth = 800;
            WindowHeight = 450;
            LogRetentionDaysPeriod = 7;

            Load();
        }
    }
}
