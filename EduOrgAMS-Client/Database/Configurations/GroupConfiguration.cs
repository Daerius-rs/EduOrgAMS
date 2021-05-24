using System;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduOrgAMS.Client.Database.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(group => group.Id);

            builder.HasOne(group => group.Curator)
                .WithMany(user => user.CuratedGroups);

            builder.Property(group => group.Id)
                .ValueGeneratedOnAdd();
            builder.Property(group => group.RecruitYear)
                .IsRequired();
            builder.Property(group => group.BaseClassesCount)
                .HasDefaultValue(9)
                .IsRequired();
            builder.Property(group => group.ProfessionId)
                .IsRequired();
            builder.Property(group => group.CuratorId)
                .IsRequired();
        }
    }
}
