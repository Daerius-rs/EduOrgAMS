using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(course => course.Id);

            builder.Property(course => course.Id)
                .ValueGeneratedOnAdd();
            builder.Property(course => course.GroupId)
                .IsRequired();
            builder.Property(course => course.Number)
                .IsRequired();
            builder.Property(course => course.StartYear)
                .IsRequired();
            builder.Property(course => course.EndYear)
                .IsRequired();
            builder.Property(course => course.LessonsFinalGrades)
                .HasColumnType("LONGTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
        }
    }
}
