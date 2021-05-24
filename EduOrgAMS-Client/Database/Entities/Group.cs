using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduOrgAMS.Client.Database.Entities
{
    public class Group : BaseEntity
    {
        [NotMapped]
        public static readonly Group Default = new Group
        {
            Id = -1,
            ProfessionId = Profession.Default.Id,
            CuratorId = User.Default.Id
        };

        [NotMapped]
        public string Name
        {
            get
            {
                return $"{RecruitYear.ToString()[2..]}{Profession.ShortName}-{BaseClassesCount}";
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
        private ushort _recruitYear;
        public ushort RecruitYear
        {
            get
            {
                return _recruitYear;
            }
            set
            {
                _recruitYear = value;
                OnPropertyChanged(nameof(RecruitYear));
                OnPropertyChanged(nameof(Name));
            }
        }
        [NotMapped]
        private byte _baseClassesCount;
        public byte BaseClassesCount
        {
            get
            {
                return _baseClassesCount;
            }
            set
            {
                _baseClassesCount = value;
                OnPropertyChanged(nameof(BaseClassesCount));
                OnPropertyChanged(nameof(Name));
            }
        }
        [NotMapped]
        private int _professionId;
        public int ProfessionId
        {
            get
            {
                return _professionId;
            }
            set
            {
                _professionId = value;
                OnPropertyChanged(nameof(ProfessionId));
                OnPropertyChanged(nameof(Profession));
                OnPropertyChanged(nameof(Name));
            }
        }
        [NotMapped]
        private int _curatorId;
        public int CuratorId
        {
            get
            {
                return _curatorId;
            }
            set
            {
                _curatorId = value;
                OnPropertyChanged(nameof(CuratorId));
                OnPropertyChanged(nameof(Curator));
            }
        }

        [ForeignKey("ProfessionId")]
        public virtual Profession Profession { get; set; }
        [ForeignKey("CuratorId")]
        public virtual User Curator { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}
