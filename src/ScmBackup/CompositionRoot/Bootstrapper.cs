﻿using ScmBackup.Hosters;
using ScmBackup.Http;
using ScmBackup.Resources;
using SimpleInjector;
using System.Globalization;

namespace ScmBackup.CompositionRoot
{
    public class Bootstrapper
    {
        public static void SetupResources()
        {
            // TODO: determine current culture
            var culture = new CultureInfo("en-US");

            Resource.Initialize(new ResourceProvider(), culture);
        }

        /// <summary>
        /// Registers IoC dependencies and returns the initialized container
        /// </summary>
        /// <returns></returns>
        public static Container BuildContainer()
        {
            var container = new Container();
            container.Register<IScmBackup, ScmBackup>();
            container.RegisterDecorator<IScmBackup, ErrorHandlingScmBackup>();

            container.RegisterCollection<ILogger>(new ConsoleLogger(), new NLogLogger());
            container.Register<ILogger, CompositeLogger>(Lifestyle.Singleton);

            container.Register<IConfigReader, ConfigReader>();
            container.RegisterDecorator<IConfigReader, ValidatingConfigReader>();

            container.Register<IHttpRequest, HttpRequest>();

            var hosterFactory = new HosterFactory(container);
            hosterFactory.Register<GithubHoster>();

            container.RegisterSingleton<IHosterValidator>(new HosterValidator(hosterFactory));

            container.Verify();

            return container;
        }
    }
}
