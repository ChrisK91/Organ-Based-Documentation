using Bogus;
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

                db.Patients.Add(new Patient()
                {
                    Birthday = DateTime.Now - new TimeSpan(20 * 365, 0, 0, 0),
                    Name = "Wurst, Hans",
                    Notes = "Einbettzimmer"
                });


                int t = 0;
                int max = 5000;
                for (int n = 0; n < max; n++)
                {
                    t++;

                    if(t % 100 == 0)
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
                        var diagnosis = new List<VersionedStringEntry>();

                        for (int d = 0; d < faker.Random.Number(1, 50); d++)
                        {
                            diagnosis.Add(new VersionedStringEntry(fakerLorem.Sentence(3, 10), fakerDate.Past(100)));
                        }

                        var adm = db.Admissions.Add(new Admission()
                        {
                            Patient = p.Entity,
                            AdmissionDateTime = admissionDate,
                            DischargeDateTime = discharge,
                            Diagnosis = diagnosis
                        });
                    }
                }

                db.SaveChanges();

                var DiagnosisList = new List<EntityEntry<VersionedStringEntry>>();
                DiagnosisList.Add(db.VersionedStringEntries.Add(new VersionedStringEntry("Testeintrag")));
                DiagnosisList.Add(db.VersionedStringEntries.Add(new VersionedStringEntry("Testeintrag 2")));
                DiagnosisList.Add(db.VersionedStringEntries.Add(new VersionedStringEntry("Testdiagnose 1, **Fett**")));

                db.Admissions.Add(new Admission()
                {
                    AdmissionDateTime = DateTime.Now - new TimeSpan(5, 0, 0, 0),
                    Patient = db.Patients.Where(p => p.Name == "Mustermann, Max").First(),
                    Room = db.Rooms.Where(r => r.Name == "100").First()
                });


                db.Admissions.Add(new Admission()
                {
                    AdmissionDateTime = DateTime.Now - new TimeSpan(5, 0, 0, 0),
                    Patient = db.Patients.Where(p => p.Name == "Mustermann, Margarete").First(),
                    Room = db.Rooms.Where(r => r.Name == "100").First()
                });

                db.Admissions.Add(new Admission()
                {
                    AdmissionDateTime = DateTime.Now - new TimeSpan(10, 0, 0, 0),
                    DischargeDateTime = DateTime.Now - new TimeSpan(1, 0, 0, 0),
                    Abdominal = "Abdominaleintrag",
                    Cardiology = "Karidologieeintrag",
                    Patient = db.Patients.Where(p => p.Name == "Wurst, Hans").First(),
                });
                db.SaveChanges();

                int i = 0;
                foreach (var a in db.Admissions.OrderByDescending(a => a.AdmissionId).Take(3).Include(a => a.Diagnosis))
                {
                    a.Diagnosis.Add(DiagnosisList[i].Entity);
                    i++;
                }
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

        public static string GetLastVersionedEntry(this IEnumerable<VersionedStringEntry> versionedStringEntries)
        {
            if(versionedStringEntries.Count() == 1)
            {
                return versionedStringEntries.First().Value;
            } else if (versionedStringEntries.Count() == 0)
            {
                return null;
            }

            return versionedStringEntries.OrderByDescending(v => v.Timestamp).FirstOrDefault()?.Value; //TODO: Improve handling of empty collections
        }

        public static IIncludableQueryable<Admission, IEnumerable<VersionedStringEntry>> IncludeVersionedProperties(this IQueryable<Admission> admissions)
        {
           return admissions.Include(a => a.Diagnosis.OrderByDescending(v => v.Timestamp));
        }



        public static IIncludableQueryable<Room, IEnumerable<VersionedStringEntry>> ThenIncludeVersionedProperties(this IIncludableQueryable<Room, List<Admission>> admissions)
        {
            return admissions.ThenInclude(a => a.Diagnosis.OrderByDescending(v => v.Timestamp));
        }
    }
}
