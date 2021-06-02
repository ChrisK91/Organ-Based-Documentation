using DocuPOC.Models;
using DocuPOC.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Database
{
    public class DataContext : DbContext
    {
        private string defaultPath { get; } = System.IO.Path.Combine(AppContext.BaseDirectory, "database.db");

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Admission> Admissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var builder = new SqliteConnectionStringBuilder();
            builder.DataSource = Ioc.Default.GetService<ISettingsService>().GetSettingWithDefault("DatabaseLocation", defaultPath);

            options.UseSqlite(builder.ToString());
        }
    }
}
