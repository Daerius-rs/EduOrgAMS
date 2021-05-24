using System;

namespace EduOrgAMS.Client.Database.Entities.Data
{
    public class LessonGrade
    {
        public int EducationalSubjectId { get; set; }
        public DateTime Date { get; set; }
        public byte LessonNumber { get; set; }
        public byte Grade { get; set; }
    }
}
