using ClashRoyale;
using Currency;
using Football;
using QrCodeCreation;
using Strava;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace KomobotCore
{
    internal static class ServiceContainer
    {
        internal static UnityContainer Container = new UnityContainer();

        static ServiceContainer()
        {
            Container.RegisterType<ICurrencyService, CurrencyService>();
            Container.RegisterType<IQrCodeCreatorService, QrCodeCreatorService>();
            Container.RegisterType<IFootballDataService, FootballDataService>();
            Container.RegisterType<IStravaService, StravaService>();
            Container.RegisterType<IClashRoyaleService, ClashRoyaleService>();
        }
    }
}
