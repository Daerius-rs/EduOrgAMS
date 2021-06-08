using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using EduOrgAMS.Client.Database.Entities.Data;
using RIS.Collections.Nestable;
using RIS.Extensions;

namespace EduOrgAMS.Client.Database.Entities
{
    public class Course : BaseEntity
    {
        [NotMapped]
        public static readonly Course Default = new Course
        {
            Id = -1
        };

        [NotMapped]
        public string Name
        {
            get
            {
                return $"{Group.Name} №{Number} {StartYear}-{EndYear}";
            }
        }
        [NotMapped]
        private Dictionary<int, Dictionary<int, LessonFinalGrade>> _lessonsFinalGradesList;
        [NotMapped]
        public Dictionary<int, Dictionary<int, LessonFinalGrade>> LessonsFinalGradesList
        {
            get
            {
                return _lessonsFinalGradesList
                       ?? new Dictionary<int, Dictionary<int, LessonFinalGrade>>();
            }
            set
            {
                _lessonsFinalGradesList = value;
                _lessonsFinalGrades = FromLessonsFinalGradesList(
                    value);

                OnPropertyChanged(nameof(LessonsFinalGradesList));
                OnPropertyChanged(nameof(LessonsFinalGrades));
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
        private int _groupId;
        public int GroupId
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
        [NotMapped]
        private byte _number;
        public byte Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;

                OnPropertyChanged(nameof(Number));
                OnPropertyChanged(nameof(Name));
            }
        }
        [NotMapped]
        private ushort _startYear;
        public ushort StartYear
        {
            get
            {
                return _startYear;
            }
            set
            {
                _startYear = value;

                OnPropertyChanged(nameof(StartYear));
                OnPropertyChanged(nameof(Name));
            }
        }
        [NotMapped]
        private ushort _endYear;
        public ushort EndYear
        {
            get
            {
                return _endYear;
            }
            set
            {
                _endYear = value;

                OnPropertyChanged(nameof(EndYear));
                OnPropertyChanged(nameof(Name));
            }
        }
        [NotMapped]
        private string _lessonsFinalGrades = "{NestableListL||}";
        public string LessonsFinalGrades
        {
            get
            {
                return _lessonsFinalGrades;
            }
            set
            {
                _lessonsFinalGrades = value;
                _lessonsFinalGradesList = ToLessonsFinalGradesList(
                    value);

                OnPropertyChanged(nameof(LessonsFinalGrades));
                OnPropertyChanged(nameof(LessonsFinalGradesList));
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
        public virtual ICollection<Semester> Semesters
        {
            get
            {
                using var context = DatabaseManager.CreateContext();

                return context.Semesters
                    .Where(semester =>
                        semester.CourseId == Id)
                    .ToList();
            }
        }



        private static Dictionary<int, Dictionary<int, LessonFinalGrade>> ToLessonsFinalGradesList(
           string lessonsFinalGrades)
        {
            var lessonsFinalGradesNode = new NestableListL<string>();

            lessonsFinalGradesNode.FromStringRepresent(
                lessonsFinalGrades);

            var lessonsFinalGradesList = new Dictionary<int, Dictionary<int, LessonFinalGrade>>(
                lessonsFinalGradesNode.Length);

            for (int i = 0; i < lessonsFinalGradesNode.Length; ++i)
            {
                var userLessonsFinalGradesNode = lessonsFinalGradesNode[i]
                    .GetCollection();
                var userLessonsFinalGradesList = new Dictionary<int, LessonFinalGrade>(
                    userLessonsFinalGradesNode.Length);
                var userId = userLessonsFinalGradesNode[0]
                    .GetElement()
                    .ToInt();

                for (int j = 1; j < userLessonsFinalGradesNode.Length; ++j)
                {
                    var userLessonFinalGradeNode = userLessonsFinalGradesNode[j]
                        .GetCollection();
                    var userLessonFinalGrade = new LessonFinalGrade();

                    userLessonFinalGrade.EducationalSubjectId = userLessonFinalGradeNode[0]
                        .GetElement()
                        .ToInt();
                    userLessonFinalGrade.Grade = userLessonFinalGradeNode[1]
                        .GetElement()
                        .ToByte();

                    userLessonsFinalGradesList.Add(
                        userLessonFinalGrade.EducationalSubjectId, userLessonFinalGrade);
                }

                lessonsFinalGradesList.Add(
                    userId, userLessonsFinalGradesList);
            }

            return lessonsFinalGradesList;
        }
        private static string FromLessonsFinalGradesList(
            Dictionary<int, Dictionary<int, LessonFinalGrade>> lessonsFinalGradesList)
        {
            var lessonsFinalGradesNode = new NestableListL<string>();

            foreach (var lessonsFinalGradesPair in lessonsFinalGradesList)
            {
                var userLessonsFinalGradesNode = new NestableListL<string>();
                var userLessonsFinalGradesList = lessonsFinalGradesPair.Value;
                var userId = lessonsFinalGradesPair.Key;

                userLessonsFinalGradesNode.Add(userId
                    .ToString());

                foreach (var userLessonFinalGradesPair in userLessonsFinalGradesList)
                {
                    var userLessonFinalGradeNode = new NestableListL<string>();
                    var userLessonFinalGrade = userLessonFinalGradesPair.Value;

                    userLessonFinalGradeNode.Add(userLessonFinalGrade.EducationalSubjectId
                        .ToString());
                    userLessonFinalGradeNode.Add(userLessonFinalGrade.Grade
                        .ToString());

                    userLessonsFinalGradesNode.Add(
                        userLessonFinalGradeNode);
                }

                lessonsFinalGradesNode.Add(
                    userLessonsFinalGradesNode);
            }

            var lessonsFinalGrades = lessonsFinalGradesNode
                .ToStringRepresent();

            return lessonsFinalGrades;
        }
    }
}
