using System;
using System.Reflection;
using System.Windows;
using EduOrgAMS.Client.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduOrgAMS.Client.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<EducationalSubject> EducationalSubjects { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Profession> Professions { get; set; }

        public DatabaseContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(DatabaseManager.ConnectionString,
                DatabaseManager.ServerVersion);

            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), type =>
            {
                if (string.IsNullOrEmpty(type.Namespace))
                    return false;

                return type.Namespace.StartsWith(
                    GetType().Namespace ?? string.Empty);
            });
        }
    }
}
