using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);

            builder.Property(user => user.Id)
                .ValueGeneratedOnAdd();
            builder.Property(user => user.LastName)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(user => user.FirstName)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(user => user.Patronymic)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(user => user.GenderId)
                .IsRequired();
            builder.Property(user => user.BirthDate)
                .HasDefaultValue(0UL)
                .IsRequired();
            builder.Property(user => user.PhoneNumber)
                .HasColumnType("TINYTEXT");
            builder.Property(user => user.Email)
                .HasColumnType("TINYTEXT");
            builder.Property(user => user.Address)
                .HasColumnType("TEXT");
            builder.Property(user => user.AvatarUrl)
                .HasColumnType("TEXT");
            builder.Property(user => user.RoleId)
                .IsRequired();
            builder.Property(user => user.RegistrationDate)
                .HasDefaultValue(0UL)
                .IsRequired();
            builder.Property(user => user.Dismissed)
                .HasDefaultValue(false);
            builder.Property(user => user.GroupId)
                .HasDefaultValue(null);
        }
    }
}
