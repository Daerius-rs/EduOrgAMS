using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using EduOrgAMS.Client.Database.Entities.Data;
using RIS.Collections.Nestable;
using RIS.Extensions;

namespace EduOrgAMS.Client.Database.Entities
{
    public class Semester : BaseEntity
    {
        [NotMapped]
        public static readonly Semester Default = new Semester
        {
            Id = -1
        };

        [NotMapped]
        private Dictionary<int, Dictionary<int, List<LessonMiss>>> _lessonsMissesList;
        [NotMapped]
        public Dictionary<int, Dictionary<int, List<LessonMiss>>> LessonsMissesList
        {
            get
            {
                return _lessonsMissesList
                       ?? new Dictionary<int, Dictionary<int, List<LessonMiss>>>();
            }
            set
            {
                _lessonsMissesList = value;
                _lessonsMisses = FromLessonsMissesList(
                    value);

                OnPropertyChanged(nameof(LessonsMissesList));
                OnPropertyChanged(nameof(LessonsMisses));
            }
        }
        [NotMapped]
        private Dictionary<int, Dictionary<int, List<LessonGrade>>> _lessonsGradesList;
        [NotMapped]
        public Dictionary<int, Dictionary<int, List<LessonGrade>>> LessonsGradesList
        {
            get
            {
                return _lessonsGradesList
                       ?? new Dictionary<int, Dictionary<int, List<LessonGrade>>>();
            }
            set
            {
                _lessonsGradesList = value;
                _lessonsGrades = FromLessonsGradesList(
                    value);

                OnPropertyChanged(nameof(LessonsGradesList));
                OnPropertyChanged(nameof(LessonsGrades));
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
        private int _courseId;
        public int CourseId
        {
            get
            {
                return _courseId;
            }
            set
            {
                _courseId = value;
                OnPropertyChanged(nameof(CourseId));
                OnPropertyChanged(nameof(Course));
            }
        }
        public byte Number { get; set; }
        public ulong StartDate { get; set; }
        public ulong EndDate { get; set; }
        [NotMapped]
        private string _lessonsMisses;
        public string LessonsMisses
        {
            get
            {
                return _lessonsMisses;
            }
            set
            {
                _lessonsMisses = value;
                _lessonsMissesList = ToLessonsMissesList(
                    value);

                OnPropertyChanged(nameof(LessonsMisses));
                OnPropertyChanged(nameof(LessonsMissesList));
            }
        }
        [NotMapped]
        private string _lessonsGrades;
        public string LessonsGrades
        {
            get
            {
                return _lessonsGrades;
            }
            set
            {
                _lessonsGrades = value;
                _lessonsGradesList = ToLessonsGradesList(
                    value);

                OnPropertyChanged(nameof(LessonsGrades));
                OnPropertyChanged(nameof(LessonsGradesList));
            }
        }
        [NotMapped]
        private string _lessonsFinalGrades;
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

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }



        private static Dictionary<int, Dictionary<int, List<LessonMiss>>> ToLessonsMissesList(
            string lessonsMisses)
        {
            var lessonsMissesNode = new NestableListL<string>();

            lessonsMissesNode.FromStringRepresent(
                lessonsMisses);

            var lessonsMissesList = new Dictionary<int, Dictionary<int, List<LessonMiss>>>(
                lessonsMissesNode.Length);

            for (int i = 0; i < lessonsMissesNode.Length; ++i)
            {
                var userLessonsMissesNode = lessonsMissesNode[i]
                    .GetCollection();
                var userLessonsMissesList = new Dictionary<int, List<LessonMiss>>(
                    userLessonsMissesNode.Length);
                var userId = userLessonsMissesNode[0]
                    .GetElement()
                    .ToInt();

                for (int j = 1; j < userLessonsMissesNode.Length; ++j)
                {
                    var userLessonMissesNode = userLessonsMissesNode[j]
                        .GetCollection();
                    var userLessonMissesList = new List<LessonMiss>(
                        userLessonMissesNode.Length);
                    var educationalSubjectId = userLessonMissesNode[0]
                        .GetElement()
                        .ToInt();

                    for (int k = 1; k < userLessonMissesNode.Length; ++k)
                    {
                        var userLessonMissNode = userLessonMissesNode[k]
                            .GetCollection();
                        var userLessonMiss = new LessonMiss();

                        userLessonMiss.EducationalSubjectId = educationalSubjectId;

                        userLessonMiss.Date = DateTime.Parse(userLessonMissNode[0]
                            .GetElement(), CultureInfo.InvariantCulture);
                        userLessonMiss.LessonNumber = userLessonMissNode[1]
                            .GetElement()
                            .ToByte();
                        userLessonMiss.ConsiderAsZeroGrade = userLessonMissNode[2]
                            .GetElement()
                            .ToBoolean();

                        userLessonMissesList.Add(
                            userLessonMiss);
                    }

                    userLessonsMissesList.Add(
                       educationalSubjectId, userLessonMissesList);
                }

                lessonsMissesList.Add(
                    userId, userLessonsMissesList);
            }

            return lessonsMissesList;
        }
        private static string FromLessonsMissesList(
            Dictionary<int, Dictionary<int, List<LessonMiss>>> lessonsMissesList)
        {
            var lessonsMissesNode = new NestableListL<string>();

            foreach (var lessonsMissesPair in lessonsMissesList)
            {
                var userLessonsMissesNode = new NestableListL<string>();
                var userLessonsMissesList = lessonsMissesPair.Value;
                var userId = lessonsMissesPair.Key;

                userLessonsMissesNode.Add(userId
                    .ToString());

                foreach (var userLessonMissesPair in userLessonsMissesList)
                {
                    var userLessonMissesNode = new NestableListL<string>();
                    var userLessonMissesList = userLessonMissesPair.Value;
                    var educationalSubjectId = userLessonMissesPair.Key;

                    userLessonMissesNode.Add(educationalSubjectId
                        .ToString());

                    foreach (var userLessonMiss in userLessonMissesList)
                    {
                        var userLessonMissNode = new NestableListL<string>();

                        userLessonMissNode.Add(userLessonMiss.Date
                            .ToString(CultureInfo.InvariantCulture));
                        userLessonMissNode.Add(userLessonMiss.LessonNumber
                            .ToString());
                        userLessonMissNode.Add(userLessonMiss.ConsiderAsZeroGrade
                            .ToString());

                        userLessonMissesNode.Add(
                            userLessonMissNode);
                    }

                    userLessonsMissesNode.Add(
                        userLessonMissesNode);
                }

                lessonsMissesNode.Add(
                    userLessonsMissesNode);
            }

            var lessonsMisses = lessonsMissesNode
                .ToStringRepresent();

            return lessonsMisses;
        }

        private static Dictionary<int, Dictionary<int, List<LessonGrade>>> ToLessonsGradesList(
            string lessonsGrades)
        {
            var lessonsGradesNode = new NestableListL<string>();

            lessonsGradesNode.FromStringRepresent(
                lessonsGrades);

            var lessonsGradesList = new Dictionary<int, Dictionary<int, List<LessonGrade>>>(
                lessonsGradesNode.Length);

            for (int i = 0; i < lessonsGradesNode.Length; ++i)
            {
                var userLessonsGradesNode = lessonsGradesNode[i]
                    .GetCollection();
                var userLessonsGradesList = new Dictionary<int, List<LessonGrade>>(
                    userLessonsGradesNode.Length);
                var userId = userLessonsGradesNode[0]
                    .GetElement()
                    .ToInt();

                for (int j = 1; j < userLessonsGradesNode.Length; ++j)
                {
                    var userLessonGradesNode = userLessonsGradesNode[j]
                        .GetCollection();
                    var userLessonGradesList = new List<LessonGrade>(
                        userLessonGradesNode.Length);
                    var educationalSubjectId = userLessonGradesNode[0]
                        .GetElement()
                        .ToInt();

                    for (int k = 1; k < userLessonGradesNode.Length; ++k)
                    {
                        var userLessonGradeNode = userLessonGradesNode[k]
                            .GetCollection();
                        var userLessonGrade = new LessonGrade();

                        userLessonGrade.EducationalSubjectId = educationalSubjectId;

                        userLessonGrade.Date = DateTime.Parse(userLessonGradeNode[0]
                            .GetElement(), CultureInfo.InvariantCulture);
                        userLessonGrade.LessonNumber = userLessonGradeNode[1]
                            .GetElement()
                            .ToByte();
                        userLessonGrade.Grade = userLessonGradeNode[2]
                            .GetElement()
                            .ToByte();

                        userLessonGradesList.Add(
                            userLessonGrade);
                    }

                    userLessonsGradesList.Add(
                       educationalSubjectId, userLessonGradesList);
                }

                lessonsGradesList.Add(
                    userId, userLessonsGradesList);
            }

            return lessonsGradesList;
        }
        private static string FromLessonsGradesList(
            Dictionary<int, Dictionary<int, List<LessonGrade>>> lessonsGradesList)
        {
            var lessonsGradesNode = new NestableListL<string>();

            foreach (var lessonsGradesPair in lessonsGradesList)
            {
                var userLessonsGradesNode = new NestableListL<string>();
                var userLessonsGradesList = lessonsGradesPair.Value;
                var userId = lessonsGradesPair.Key;

                userLessonsGradesNode.Add(userId
                    .ToString());

                foreach (var userLessonGradesPair in userLessonsGradesList)
                {
                    var userLessonGradesNode = new NestableListL<string>();
                    var userLessonGradesList = userLessonGradesPair.Value;
                    var educationalSubjectId = userLessonGradesPair.Key;

                    userLessonGradesNode.Add(educationalSubjectId
                        .ToString());

                    foreach (var userLessonGrade in userLessonGradesList)
                    {
                        var userLessonGradeNode = new NestableListL<string>();

                        userLessonGradeNode.Add(userLessonGrade.Date
                            .ToString(CultureInfo.InvariantCulture));
                        userLessonGradeNode.Add(userLessonGrade.LessonNumber
                            .ToString());
                        userLessonGradeNode.Add(userLessonGrade.Grade
                            .ToString());

                        userLessonGradesNode.Add(
                            userLessonGradeNode);
                    }

                    userLessonsGradesNode.Add(
                       userLessonGradesNode);
                }

                lessonsGradesNode.Add(
                    userLessonsGradesNode);
            }

            var lessonsGrades = lessonsGradesNode
                .ToStringRepresent();

            return lessonsGrades;
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
