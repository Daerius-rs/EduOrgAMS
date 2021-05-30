using System;
using System.Linq;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Utils;

namespace EduOrgAMS.Client.Extensions
{
    public static class GenderExtensions
    {
        public static string GetResourceKey(this Gender target)
        {
            if (target == null)
                return null;

            return $"Entity-{target.GetType().Name}-{target.Id}";
        }
        public static string GetResourceKey(this Gender target, int id)
        {
            if (target == null)
                return null;

            return $"Entity-{target.GetType().Name}-{id}";
        }

        public static string GetLocalizedName(this Gender target)
        {
            if (target == null)
                return null;

            string name =
                target.Name;
            string localizedName =
                LocalizationUtils.GetLocalized(GetResourceKey(target));

            return !string.IsNullOrEmpty(localizedName)
                ? localizedName
                : name;
        }

        public static string[] GetLocalizedNames(this Gender target)
        {
            return GetLocalizedNames();
        }
        public static string[] GetLocalizedNames()
        {
            using var context = DatabaseManager.CreateContext();

            var genders = context.Genders.ToArray();
            var localizedNames = new string[genders.Length];

            for (var i = 0; i < genders.Length; ++i)
            {
                ref var category = ref genders[i];

                string name = category.Name;

                string localizedName =
                    LocalizationUtils.GetLocalized(GetResourceKey(category));

                localizedNames[i] = !string.IsNullOrEmpty(localizedName)
                    ? localizedName
                    : name;
            }

            return localizedNames;
        }

        public static Gender ParseLocalizedName(this Gender target,
            string localizedName)
        {
            return ParseLocalizedName(localizedName);
        }
        public static Gender ParseLocalizedName(string localizedName)
        {
            using var context = DatabaseManager.CreateContext();

            var genders = context.Genders.ToArray();
            var localizedNames = GetLocalizedNames();

            for (int i = 0; i < localizedNames.Length; ++i)
            {
                if (localizedNames[i] != localizedName)
                    continue;

                return genders[i];
            }

            return new Gender
            {
                Id = -1,
                Name = null
            };
        }
    }
}
