using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class GenderConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        {
            builder.HasKey(gender => gender.Id);

            builder.Property(gender => gender.Id)
                .ValueGeneratedOnAdd();
            builder.Property(gender => gender.Name)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
        }
    }
}
