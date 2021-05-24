using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduOrgAMS.Client.Database.Entities
{
    public class Profession : BaseEntity
    {
        [NotMapped]
        public static readonly Profession Default = new Profession
        {
            Id = -1
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
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
