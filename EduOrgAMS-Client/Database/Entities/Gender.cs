using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduOrgAMS.Client.Database.Entities
{
    public class Gender : BaseEntity
    {
        [NotMapped]
        public static readonly Gender Default = new Gender
        {
            Id = 1,
            Name = "Unknown"
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
        public string Name { get; set; }
    }
}
