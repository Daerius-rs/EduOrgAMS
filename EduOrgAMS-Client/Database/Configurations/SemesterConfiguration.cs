using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
    {
        public void Configure(EntityTypeBuilder<Semester> builder)
        {
            builder.HasKey(semester => semester.Id);

            builder.Property(semester => semester.Id)
                .ValueGeneratedOnAdd();
            builder.Property(semester => semester.CourseId)
                .IsRequired();
            builder.Property(semester => semester.Number)
                .IsRequired();
            builder.Property(semester => semester.StartDate)
                .HasDefaultValue(0UL)
                .IsRequired();
            builder.Property(semester => semester.EndDate)
                .HasDefaultValue(0UL)
                .IsRequired();
            builder.Property(semester => semester.LessonsMisses)
                .HasColumnType("LONGTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(semester => semester.LessonsGrades)
                .HasColumnType("LONGTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(semester => semester.LessonsFinalGrades)
                .HasColumnType("LONGTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
        }
    }
}
