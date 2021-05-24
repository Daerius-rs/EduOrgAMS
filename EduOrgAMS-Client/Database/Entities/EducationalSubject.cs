using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduOrgAMS.Client.Database.Entities
{
    public class EducationalSubject : BaseEntity
    {
        [NotMapped]
        public static readonly EducationalSubject Default = new EducationalSubject
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
        public string Name { get; set; }
    }
}
