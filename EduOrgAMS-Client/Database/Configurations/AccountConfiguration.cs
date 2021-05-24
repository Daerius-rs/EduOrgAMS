using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(account => account.Id);

            builder.HasIndex(account => account.Login)
                .IsUnique();
            builder.HasIndex(account => account.Token)
                .IsUnique();

            builder.Property(account => account.Id)
                .ValueGeneratedOnAdd();
            builder.Property(account => account.Login)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(account => account.Password)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(account => account.Token)
                .HasColumnType("TINYTEXT")
                .HasDefaultValue(string.Empty)
                .IsRequired();
            builder.Property(account => account.TokenExpirationDate)
                .HasDefaultValue(0UL)
                .IsRequired();
            builder.Property(account => account.UserId)
                .IsRequired();
        }
    }
}
