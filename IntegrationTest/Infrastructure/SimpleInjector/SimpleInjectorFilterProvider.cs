using SimpleInjector;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace IntegrationTest.Infrastructure
{
    public class SimpleInjectorFilterProvider : ActionDescriptorFilterProvider, IFilterProvider
    {
        private readonly BaseFilterProvider _defaultProvider = new BaseFilterProvider();

        private readonly Container _container;

        public SimpleInjectorFilterProvider(Container container)
        {
            _container = container;
        }

        public new IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, 
            HttpActionDescriptor actionDescriptor)
        {
            var filterInfos = _defaultProvider.GetFilters(configuration, actionDescriptor).ToArray();

            var filters = filterInfos.Select(filter => filter.Instance);

            foreach (var instance in filters)
            {
                var producer = _container.GetRegistration(instance.GetType(), true);

                producer.Registration.InitializeInstance(instance);
            }

            return filterInfos;
        }
    }
}