using Entities.Enum;
using Entities.IdentityEntites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;


namespace Entities
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Country> Countries { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Employee>().ToTable("Employee");

            //seed data
            //string countriesJson = System.IO.File.ReadAllText("Countries.json");
            //List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            //string employeeJson = System.IO.File.ReadAllText("Employee.json");
            //List<Employee>? employeesData = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(employeeJson);

            //foreach (Country country in countries)
            //{
            //    modelBuilder.Entity<Country>().HasData(country);
            //}

            //foreach (Employee employee in employeesData)
            //{
            //    modelBuilder.Entity<Employee>().HasData(employee);
            //}

            modelBuilder.Entity<Country>().HasData(new Country()
            {
                CountryId = Guid.Parse("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                CountryName = "India"
            },
            new Country()
            {
                CountryId = Guid.Parse("12e15727-d369-49a9-8b13-bc22e9362179"),
                CountryName = "USA"
            });

            modelBuilder.Entity<Employee>().HasData(new Employee()
            {
                EmployeeId = Guid.Parse("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                EmployeeName = "Rushikesh",
                Address = "Pune",
                CountryID = Guid.Parse("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                Email = "rushikesh@gmail.com",
                DateOfBirth = new DateTime(1996, 03, 13),
                CountryName = "India",
                Gender = GenderEnum.Male.ToString(),
                ReceiveNewsLetters = true
            },
           new Employee()
           {
               EmployeeId = Guid.Parse("12e15727-d369-49a9-8b13-bc22e9362179"),
               EmployeeName = "Dilip",
               Address = "Nagpur",
               CountryID = Guid.Parse("12e15727-d369-49a9-8b13-bc22e9362179"),
               Email = "dilip@gmail.com",
               DateOfBirth = new DateTime(1996, 03, 13),
               CountryName = "India",
               Gender = Enum.GenderEnum.Male.ToString(),
               ReceiveNewsLetters = true
           });


            this.SeedRoles(modelBuilder);
        }
        private  void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
              new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
              new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" },
              new IdentityRole() { Name = "HR", ConcurrencyStamp = "3", NormalizedName = "HR" }
              );
        }
    }
}
