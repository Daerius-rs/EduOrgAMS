using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using EduOrgAMS.Client.Utils;

namespace EduOrgAMS.Client.Database.Entities
{
    public class User : BaseEntity
    {
        [NotMapped]
        public static readonly User Default = new User
        {
            Id = -1,
            GenderId = Gender.Default.Id,
            RoleId = Role.Default.Id,
            GroupId = Group.Default.Id
        };

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{LastName} {FirstName} {Patronymic}";
            }
        }
        [NotMapped]
        public string InitialsFirst
        {
            get
            {
                return $"{FirstName?.FirstOrDefault()}.{Patronymic?.FirstOrDefault()}. {LastName}";
            }
        }
        [NotMapped]
        public string InitialsLast
        {
            get
            {
                return $"{LastName} {FirstName?.FirstOrDefault()}.{Patronymic?.FirstOrDefault()}.";
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

                var user = value != -1 && value != 0
                    ? DatabaseManager.Find<User>(value)
                    : null;

                LastName = user?.LastName ?? null;
                FirstName = user?.FirstName ?? null;
                Patronymic = user?.Patronymic ?? null;
                GenderId = user?.GenderId ?? Gender.Default.Id;
                BirthDate = user?.BirthDate ?? 0;
                PhoneNumber = user?.PhoneNumber ?? null;
                Email = user?.Email ?? null;
                Address = user?.Address ?? null;
                AvatarUrl = user?.AvatarUrl ?? null;
                RoleId = user?.RoleId ?? Role.Default.Id;
                RegistrationDate = user?.RegistrationDate ?? 0;
                Dismissed = user?.Dismissed ?? false;
                GroupId = user?.GroupId ?? Group.Default.Id;

                OnPropertyChanged(nameof(Id));
                OnAllPropertiesChanged();
            }
        }
        [NotMapped]
        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(InitialsFirst));
                OnPropertyChanged(nameof(InitialsLast));
            }
        }
        [NotMapped]
        private string _firstName;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(InitialsFirst));
                OnPropertyChanged(nameof(InitialsLast));
            }
        }
        [NotMapped]
        private string _patronymic;
        public string Patronymic
        {
            get
            {
                return _patronymic;
            }
            set
            {
                _patronymic = value;
                OnPropertyChanged(nameof(Patronymic));
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(InitialsFirst));
                OnPropertyChanged(nameof(InitialsLast));
            }
        }
        [NotMapped]
        private int _genderId;
        public int GenderId
        {
            get
            {
                return _genderId;
            }
            set
            {
                _genderId = value;
                OnPropertyChanged(nameof(GenderId));
                OnPropertyChanged(nameof(Gender));
            }
        }
        public ulong BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AvatarUrl { get; set; }
        [NotMapped]
        private int _roleId;
        public int RoleId
        {
            get
            {
                return _roleId;
            }
            set
            {
                _roleId = value;
                OnPropertyChanged(nameof(RoleId));
                OnPropertyChanged(nameof(Role));
            }
        }
        [NotMapped]
        private ulong _registrationDate = TimeUtils.ToUnixTimeStamp(DateTime.UtcNow);
        public ulong RegistrationDate
        {
            get
            {
                return _registrationDate;
            }
            set
            {
                _registrationDate = value;

                OnPropertyChanged(nameof(RegistrationDate));
            }
        }
        public bool Dismissed { get; set; }
        [NotMapped]
        private int? _groupId;
        public int? GroupId
        {
            get
            {
                return _groupId;
            }
            set
            {
                _groupId = value;
                OnPropertyChanged(nameof(GroupId));
                OnPropertyChanged(nameof(Group));
            }
        }

        [ForeignKey("GenderId")]
        public virtual Gender Gender
        {
            get
            {
                return DatabaseManager.Find<Gender>(
                    GenderId);
            }
            set
            {
                GenderId = value.Id;
            }
        }
        [ForeignKey("RoleId")]
        public virtual Role Role
        {
            get
            {
                return DatabaseManager.Find<Role>(
                    RoleId);
            }
            set
            {
                RoleId = value.Id;
            }
        }
        [ForeignKey("GroupId")]
        public virtual Group Group
        {
            get
            {
                return DatabaseManager.Find<Group>(
                    GroupId);
            }
            set
            {
                GroupId = value.Id;
            }
        }

        [NotMapped]
        public virtual ICollection<Group> CuratedGroups
        {
            get
            {
                using var context = DatabaseManager.CreateContext();

                return context.Groups
                    .Where(group =>
                        group.CuratorId == Id)
                    .ToList();
            }
        }
    }
}
