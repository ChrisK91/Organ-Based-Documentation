using DocuPOC.Models;
using DocuPOC.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.Database
{
    public class DataContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<VersionedStringEntry> VersionedStringEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var builder = new SqliteConnectionStringBuilder();
            builder.DataSource = Ioc.Default.GetService<ISettingsService>().GetSettingWithDefault("DatabaseLocation", SettingsService.DefaultDatabaseLocation);
            

            options.UseSqlite(builder.ToString(), o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

#if true
            options.LogTo(m => Debugger.Log(0, null, m), LogLevel.Information);
#endif
        }
    }
}
