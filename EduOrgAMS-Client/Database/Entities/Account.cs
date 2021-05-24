using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduOrgAMS.Client.Database.Entities
{
    public class Account : BaseEntity
    {
        [NotMapped]
        public static readonly Account Default = new Account
        {
            Id = -1,
            UserId = User.Default.Id
        };

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
        public string Login { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public ulong TokenExpirationDate { get; set; }
        [NotMapped]
        private int _userId;
        public int UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
                OnPropertyChanged(nameof(User));
            }
        }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
