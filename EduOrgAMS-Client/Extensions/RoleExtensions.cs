using System;
using System.Linq;
using EduOrgAMS.Client.Database;
using EduOrgAMS.Client.Database.Entities;
using EduOrgAMS.Client.Utils;

namespace EduOrgAMS.Client.Extensions
{
    public static class RoleExtensions
    {
        public static string GetResourceKey(this Role target)
        {
            return $"Entity-{target.GetType().Name}-{target.Id}";
        }
        public static string GetResourceKey(this Role target, int id)
        {
            return $"Entity-{target.GetType().Name}-{id}";
        }

        public static string GetLocalizedName(this Role target)
        {
            string name =
                target.Name;
            string localizedName =
                LocalizationUtils.GetLocalized(GetResourceKey(target));

            return !string.IsNullOrEmpty(localizedName)
                ? localizedName
                : name;
        }

        public static string[] GetLocalizedNames(this Role target)
        {
            return GetLocalizedNames();
        }
        public static string[] GetLocalizedNames()
        {
            using var context = DatabaseManager.CreateContext();

            var roles = context.Roles.ToArray();
            var localizedNames = new string[roles.Length];

            for (var i = 0; i < roles.Length; ++i)
            {
                ref var category = ref roles[i];

                string name = category.Name;

                string localizedName =
                    LocalizationUtils.GetLocalized(GetResourceKey(category));

                localizedNames[i] = !string.IsNullOrEmpty(localizedName)
                    ? localizedName
                    : name;
            }

            return localizedNames;
        }

        public static Role ParseLocalizedName(this Role target,
            string localizedName)
        {
            return ParseLocalizedName(localizedName);
        }
        public static Role ParseLocalizedName(string localizedName)
        {
            using var context = DatabaseManager.CreateContext();

            var roles = context.Roles.ToArray();
            var localizedNames = GetLocalizedNames();

            for (int i = 0; i < localizedNames.Length; ++i)
            {
                if (localizedNames[i] != localizedName)
                    continue;

                return roles[i];
            }

            return new Role
            {
                Id = -1,
                Name = null
            };
        }
    }
}
