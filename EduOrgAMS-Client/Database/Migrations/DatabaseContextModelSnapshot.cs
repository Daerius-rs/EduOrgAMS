﻿// <auto-generated />
using System;
using EduOrgAMS.Client.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EduOrgAMS.Client.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("Password")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("Token")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<ulong>("TokenExpirationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasDefaultValue(0ul);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ushort>("EndYear")
                        .HasColumnType("smallint unsigned");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("LessonsFinalGrades")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("LONGTEXT")
                        .HasDefaultValue("");

                    b.Property<byte>("Number")
                        .HasColumnType("tinyint unsigned");

                    b.Property<ushort>("StartYear")
                        .HasColumnType("smallint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.EducationalSubject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.HasKey("Id");

                    b.ToTable("EducationalSubjects");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Gender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.HasKey("Id");

                    b.ToTable("Genders");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte>("BaseClassesCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint unsigned")
                        .HasDefaultValue((byte)9);

                    b.Property<int>("CuratorId")
                        .HasColumnType("int");

                    b.Property<int>("ProfessionId")
                        .HasColumnType("int");

                    b.Property<ushort>("RecruitYear")
                        .HasColumnType("smallint unsigned");

                    b.HasKey("Id");

                    b.HasIndex("CuratorId");

                    b.HasIndex("ProfessionId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Profession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.HasKey("Id");

                    b.ToTable("Professions");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsAdmin")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("Permissions")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("MEDIUMTEXT")
                        .HasDefaultValue("");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Semester", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<ulong>("EndDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasDefaultValue(0ul);

                    b.Property<string>("LessonsFinalGrades")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("LONGTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("LessonsGrades")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("LONGTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("LessonsMisses")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("LONGTEXT")
                        .HasDefaultValue("");

                    b.Property<byte>("Number")
                        .HasColumnType("tinyint unsigned");

                    b.Property<ulong>("StartDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasDefaultValue(0ul);

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Semesters");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("BirthDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasDefaultValue(0ul);

                    b.Property<bool>("Dismissed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Email")
                        .HasColumnType("TINYTEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<int>("GenderId")
                        .HasColumnType("int");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TINYTEXT")
                        .HasDefaultValue("");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TINYTEXT");

                    b.Property<ulong>("RegistrationDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint unsigned")
                        .HasDefaultValue(0ul);

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GenderId");

                    b.HasIndex("GroupId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Account", b =>
                {
                    b.HasOne("EduOrgAMS.Client.Database.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Course", b =>
                {
                    b.HasOne("EduOrgAMS.Client.Database.Entities.Group", "Group")
                        .WithMany("Courses")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Group", b =>
                {
                    b.HasOne("EduOrgAMS.Client.Database.Entities.User", "Curator")
                        .WithMany("CuratedGroups")
                        .HasForeignKey("CuratorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EduOrgAMS.Client.Database.Entities.Profession", "Profession")
                        .WithMany()
                        .HasForeignKey("ProfessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Curator");

                    b.Navigation("Profession");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Semester", b =>
                {
                    b.HasOne("EduOrgAMS.Client.Database.Entities.Course", "Course")
                        .WithMany("Semesters")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.User", b =>
                {
                    b.HasOne("EduOrgAMS.Client.Database.Entities.Gender", "Gender")
                        .WithMany()
                        .HasForeignKey("GenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EduOrgAMS.Client.Database.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("EduOrgAMS.Client.Database.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Gender");

                    b.Navigation("Group");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Course", b =>
                {
                    b.Navigation("Semesters");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.Group", b =>
                {
                    b.Navigation("Courses");
                });

            modelBuilder.Entity("EduOrgAMS.Client.Database.Entities.User", b =>
                {
                    b.Navigation("CuratedGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
