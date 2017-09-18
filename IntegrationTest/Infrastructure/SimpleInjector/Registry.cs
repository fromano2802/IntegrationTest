using IntegrationTest.Models;
using IntegrationTest.Services;
using LazyCache;
using SimpleInjector;

namespace IntegrationTest.Infrastructure
{
    public static class Registry
    {
        public static void RegisterApplicationDependencies(this Container container)
        {
            container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            container.RegisterConditional(typeof(ILogger), 
                context => typeof(NLogProxy<>).MakeGenericType(context.Consumer.ImplementationType), 
                Lifestyle.Singleton, context => true);
 
            container.Register<IAppCache>(() => new CachingService(), Lifestyle.Singleton);
            container.Register(() => new AppDBContext(), Lifestyle.Singleton);
            container.Register<ITrelloRestClient>(TrelloRestClientFactory.Create, Lifestyle.Scoped);
            container.Register<ITrelloService, TrelloService>(Lifestyle.Scoped);
        }
    }
}