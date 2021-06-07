using DocuPOC.Database;
using DocuPOC.Models;
using DotLiquid;
using DotLiquid.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

                Helpers.GenerateTestData();

            }
        }
    }
}
