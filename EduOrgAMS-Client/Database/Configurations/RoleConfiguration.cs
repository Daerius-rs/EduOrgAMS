using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(role => role.Id);

            builder.Property(role => role.Id)
                .ValueGeneratedOnAdd();
            builder.Property(role => role.Name)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(role => role.Permissions)
                .HasColumnType("MEDIUMTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(role => role.IsAdmin)
                .HasDefaultValue(false);
        }
    }
}
