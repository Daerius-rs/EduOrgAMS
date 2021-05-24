using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EduOrgAMS.Client.Database.Entities
{
    public class User : BaseEntity
    {
        [NotMapped]
        public static readonly User Default = new User
        {
            Id = -1,
            GenderId = Gender.Default.Id,
            RoleId = Role.Default.Id
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
                return $"{FirstName.FirstOrDefault()}.{Patronymic.FirstOrDefault()}. {LastName}";
            }
        }
        [NotMapped]
        public string InitialsLast
        {
            get
            {
                return $"{LastName} {FirstName.FirstOrDefault()}.{Patronymic.FirstOrDefault()}.";
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
        public ulong RegistrationDate { get; set; }
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
        public virtual Gender Gender { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public virtual ICollection<Group> CuratedGroups { get; set; }
    }
}
