using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class EducationalSubjectConfiguration : IEntityTypeConfiguration<EducationalSubject>
    {
        public void Configure(EntityTypeBuilder<EducationalSubject> builder)
        {
            builder.HasKey(educationalSubject => educationalSubject.Id);

            builder.Property(educationalSubject => educationalSubject.Id)
                .ValueGeneratedOnAdd();
            builder.Property(educationalSubject => educationalSubject.Name)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
        }
    }
}
