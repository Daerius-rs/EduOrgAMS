using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EduOrgAMS.Client.Database.Entities.Data;
using RIS.Collections.Nestable;
using RIS.Extensions;

namespace EduOrgAMS.Client.Database.Entities
{
    public class Role : BaseEntity
    {
        [NotMapped]
        public static readonly Role Default = new Role
        {
            Id = 1,
            Name = "Student"
        };

        [NotMapped]
        private Dictionary<string, Permission> _permissionsList;
        [NotMapped]
        public Dictionary<string, Permission> PermissionsList
        {
            get
            {
                return _permissionsList
                       ?? new Dictionary<string, Permission>();
            }
            set
            {
                _permissionsList = value;
                _permissions = FromPermissionsList(
                    value);

                OnPropertyChanged(nameof(PermissionsList));
                OnPropertyChanged(nameof(Permissions));
            }
        }

        [NotMapped]
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
                OnAllPropertiesChanged();
            }
        }
        public string Name { get; set; }
        [NotMapped]
        private string _permissions = "{NestableListL||}";
        public string Permissions
        {
            get
            {
                return _permissions;
            }
            set
            {
                _permissions = value;
                _permissionsList = ToPermissionsList(
                    value);

                OnPropertyChanged(nameof(Permissions));
                OnPropertyChanged(nameof(PermissionsList));
            }
        }
        public bool IsAdmin { get; set; }



        private static Dictionary<string, Permission> ToPermissionsList(
            string permissions)
        {
            var permissionsNode = new NestableListL<string>();

            permissionsNode.FromStringRepresent(
                permissions);

            var permissionsList = new Dictionary<string, Permission>(
                permissionsNode.Length);

            for (int i = 0; i < permissionsNode.Length; ++i)
            {
                var permissionNode = permissionsNode[i]
                    .GetCollection();
                var permission = new Permission();

                permission.TableName = permissionNode[0]
                    .GetElement();
                permission.AddAccess = permissionNode[1]
                    .GetElement()
                    .ToBoolean();
                permission.EditAccess = permissionNode[2]
                    .GetElement()
                    .ToBoolean();
                permission.RemoveAccess = permissionNode[3]
                    .GetElement()
                    .ToBoolean();

                permissionsList.Add(
                    permission.TableName, permission);
            }

            return permissionsList;
        }
        private static string FromPermissionsList(
            Dictionary<string, Permission> permissionsList)
        {
            var permissionsNode = new NestableListL<string>();

            foreach (var permissionsPair in permissionsList)
            {
                var permissionNode = new NestableListL<string>();
                var permission = permissionsPair.Value;

                permissionNode.Add(permission.TableName);
                permissionNode.Add(permission.AddAccess
                    .ToString());
                permissionNode.Add(permission.EditAccess
                    .ToString());
                permissionNode.Add(permission.RemoveAccess
                    .ToString());

                permissionsNode.Add(permissionNode);
            }

            var permissions = permissionsNode
                .ToStringRepresent();

            return permissions;
        }
    }
}
