using DocuPOC.Database;
using DocuPOC.Models;
using DotLiquid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocuPOC.Services
{
    public static class ServiceManager
    {
        public static void SetUpServices()
        {
            Ioc.Default.ConfigureServices(new ServiceCollection()
                .AddSingleton<ISettingsService, SettingsService>()
                .AddSingleton<IPrintService, PrintService>()
                .BuildServiceProvider());

            Ioc.Default.GetService<IPrintService>().RegisterPrintMessaging();

            var db = new DataContext();

            if (!db.Database.CanConnect())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Rooms.Add(new Room() { Name = "100" });
                db.Rooms.Add(new Room() { Name = "101" });
                db.Rooms.Add(new Room() { Name = "102" });
                db.Rooms.Add(new Room() { Name = "200" });
                db.Rooms.Add(new Room() { Name = "201" });
                db.Rooms.Add(new Room() { Name = "205" });

                db.Patients.Add(new Patient()
                {
                    Birthday = DateTime.Now - new TimeSpan(70 * 365, 0, 0, 0),
                    Name = "Mustermann, Max",
                    Notes = "Allergie!"
                });

                db.Patients.Add(new Patient()
                {
                    Birthday = DateTime.Now - new TimeSpan(100 * 365, 0, 0, 0),
                    Name = "Mustermann, Margarete"
                });

                db.SaveChanges();

                db.Admissions.Add(new Admission()
                {
                    Diagnosis = "Testeintrag #1",
                    AdmissionDateTime = DateTime.Now - new TimeSpan(5, 0, 0, 0),
                    Patient = db.Patients.Where(p => p.Name == "Mustermann, Max").First(),
                    Room = db.Rooms.Where(r => r.Name == "100").First()
                });

                db.Admissions.Add(new Admission()
                {
                    Diagnosis = "Testeintrag #1",
                    AdmissionDateTime = DateTime.Now - new TimeSpan(5, 0, 0, 0),
                    Patient = db.Patients.Where(p => p.Name == "Mustermann, Margarete").First(),
                    Room = db.Rooms.Where(r => r.Name == "100").First()
                });

                db.SaveChanges();
            }
        }
    }
}
