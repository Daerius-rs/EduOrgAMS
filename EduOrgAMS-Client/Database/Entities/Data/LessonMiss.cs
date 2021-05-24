using System;

namespace EduOrgAMS.Client.Database.Entities.Data
{
    public class LessonMiss
    {
        public int EducationalSubjectId { get; set; }
        public DateTime Date { get; set; }
        public byte LessonNumber { get; set; }
        public bool ConsiderAsZeroGrade { get; set; }
    }
}
