using ContactsManager.Core.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries"); //optionally we can define table name

            modelBuilder.Entity<Person>().ToTable("Persons");//optionally we can define table name


            //Seed Data to country

            string countrystr = System.IO.File.ReadAllText("Countries.json");

            List<Country>? objcountry = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countrystr);

            foreach (Country country in objcountry!)
            {
                modelBuilder.Entity<Country>().HasData(country);

            }


            //Seed Data to person
            string personstr = System.IO.File.ReadAllText("Persons.json");

            List<Person> objpersons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personstr)!;

            foreach (Person person in objpersons)
            {
                modelBuilder.Entity<Person>().HasData(person);

            }


            //Fluent api
            modelBuilder.Entity<Person>().Property(temp => temp.Gender)
                .HasColumnType("varchar(20)");

            //Table Relation
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne<Country>(c => c.Country)
            //    .WithMany(p => p.Parsons)
            //    .HasForeignKey(p => p.CountryId);
            //});


        }

        public List<Person> Sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("Execute dbo.GetPersons_StoredProcedure").ToList();
        }

        public int Sp_InsertPerson(Person person)
        {
            SqlParameter[] sqlParameters = new SqlParameter[] 
            {
                new SqlParameter("@PersonId", person.PersonId),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters),
            };
            return Database.ExecuteSqlRaw("Execute [dbo].[Sp_InsertPerson] @PersonId,@PersonName,@Email," +
                "@DateOfBirth,@Gender,@CountryId,@Address,@ReceiveNewsLetters", sqlParameters);

        }
    }

}
