using Bogus;
using DocuPOC.Database;
using DocuPOC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC
{
    public static class Helpers
    {
        public static void GenerateTestData()
        {
            using (var db = new Database.DataContext())
            {
                db.ChangeTracker.AutoDetectChangesEnabled = false;

                var fakerName = new Bogus.DataSets.Name("de");
                var fakerDate = new Bogus.DataSets.Date("de");
                var fakerLorem = new Bogus.DataSets.Lorem("de");
                var faker = new Faker("de");

                db.Rooms.Add(new Room() { Name = "100" });
                db.Rooms.Add(new Room() { Name = "101" });
                db.Rooms.Add(new Room() { Name = "102" });
                db.Rooms.Add(new Room() { Name = "200" });
                db.Rooms.Add(new Room() { Name = "201" });
                db.Rooms.Add(new Room() { Name = "205" });

                db.SaveChanges();

                int t = 0;
                int max = 5000;
                for (int n = 0; n < max; n++)
                {
                    t++;

                    if (t % 1000 == 0)
                    {
                        Debug.WriteLine(String.Format("Iteration {0} out of {1}, commiting changes", t, max));
                        db.SaveChanges();
                    }

                    var p = db.Patients.Add(new Patient()
                    {
                        Name = fakerName.FullName(),
                        Birthday = fakerDate.Past(100)
                    });


                    for (int a = 0; a < faker.Random.Number(1, 50); a++)
                    {
                        var admissionDate = fakerDate.Past(100);
                        var discharge = admissionDate + TimeSpan.FromDays(faker.Random.Double(1, 50));

                        var adm = db.Admissions.Add(new Admission()
                        {
                            Patient = p.Entity,
                            AdmissionDateTime = admissionDate,
                            DischargeDateTime = discharge
                        });

                        for (int d = 0; d < faker.Random.Number(1, 50); d++)
                        {
                            db.UpdateDiagnosis(adm.Entity, fakerLorem.Sentence(3, 10), fakerDate.Past(100));
                        }

                    }
                }

                db.SaveChanges();

                var maxMustermann = db.Patients.Add(new Patient()
                {
                    Birthday = DateTime.Now - new TimeSpan(70 * 365, 0, 0, 0),
                    Name = "Mustermann, Max",
                    Notes = "Allergie!"
                });

                var maggy = db.Patients.Add(new Patient()
                {
                    Birthday = DateTime.Now - new TimeSpan(100 * 365, 0, 0, 0),
                    Name = "Mustermann, Margarete"
                });

                var hans = db.Patients.Add(new Patient()
                {
                    Birthday = DateTime.Now - new TimeSpan(20 * 365, 0, 0, 0),
                    Name = "Wurst, Hans",
                    Notes = "Einbettzimmer"
                });


                var pat = db.Admissions.Add(new Admission()
                {
                    AdmissionDateTime = DateTime.Now - new TimeSpan(5, 0, 0, 0),
                    Patient = maxMustermann.Entity,
                    Room = db.Rooms.Where(r => r.Name == "100").First()
                });
                db.UpdateDiagnosis(pat.Entity, "Testeintrag");


                pat = db.Admissions.Add(new Admission()
                {
                    AdmissionDateTime = DateTime.Now - new TimeSpan(5, 0, 0, 0),
                    Patient = maggy.Entity,
                    Room = db.Rooms.Where(r => r.Name == "100").First()
                });
                db.UpdateDiagnosis(pat.Entity, "Testeintrag 2");

                pat = db.Admissions.Add(new Admission()
                {
                    AdmissionDateTime = DateTime.Now - new TimeSpan(10, 0, 0, 0),
                    DischargeDateTime = DateTime.Now - new TimeSpan(1, 0, 0, 0),
                    Abdominal = "Abdominaleintrag",
                    Cardiology = "Karidologieeintrag",
                    Patient = hans.Entity,
                });
                db.UpdateDiagnosis(pat.Entity, "Testdiagnose 1, **Fett**");


                db.SaveChanges();

            }
        }

        public static int YearsDifference(this DateTime date)
        {
            return Convert.ToInt32(Math.Floor(date.DaysDifference() / 365.25));
        }

        public static int YearsDifference(this DateTimeOffset date)
        {
            return date.DateTime.YearsDifference();
        }

        public static int DaysDifference(this DateTime date)
        {
            return Convert.ToInt32(Math.Floor((DateTime.Now - date).TotalDays));
        }

        public static int DaysDifference(this DateTimeOffset date)
        {
            return date.DateTime.DaysDifference();
        }
    }
}
