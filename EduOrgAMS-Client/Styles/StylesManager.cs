using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using EduOrgAMS.Client.Generating;
using EduOrgAMS.Client.Styles.Loading.Entities;

namespace EduOrgAMS.Client.Styles
{
    public static class StylesManager
    {
        private static ReadOnlyCollection<LoadingStyle> LoadingStyles { get; }

        static StylesManager()
        {
            LoadingStyles = GetLoadingStyles();
        }



        private static string GetStyleXamlFilePath(
            string categoryName, string styleName)
        {
            string styleXamlFileName = $"{styleName}.xaml";
            categoryName = categoryName.Replace('\\', '/');

            return $"pack://application:,,,/Styles/{categoryName}/{styleXamlFileName}";
        }

        private static ResourceDictionary GetStyleResourceDictionary(
            string resourceFilePath)
        {
            if (!Uri.TryCreate(resourceFilePath, UriKind.RelativeOrAbsolute, out _))
                return null;

            ResourceDictionary styleDictionary = new ResourceDictionary
            {
                Source = new Uri(resourceFilePath)
            };

            return styleDictionary;
        }

        private static ReadOnlyCollection<LoadingStyle> GetLoadingStyles()
        {
            var loadingStyles = new List<LoadingStyle>();

            void AddStyle(string styleName)
            {
                styleName += "Theme";

                loadingStyles.Add(new LoadingStyle(
                    styleName, GetStyle("Loading", styleName)));
            }

            AddStyle("1");
            AddStyle("2");
            AddStyle("3");

            return new ReadOnlyCollection<LoadingStyle>(
                loadingStyles);
        }



        public static ResourceDictionary GetStyle(
            string categoryName, string styleName)
        {
            return GetStyleResourceDictionary(
                GetStyleXamlFilePath(
                    categoryName, styleName));
        }

        public static LoadingStyle GetRandomLoadingStyle()
        {
            var index = (int)GeneratingManager.RandomGenerator
                .GetNormalizedIndex((uint)LoadingStyles.Count);

            return LoadingStyles[index];
        }
    }
}
