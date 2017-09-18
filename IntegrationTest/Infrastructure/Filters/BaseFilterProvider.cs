using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace IntegrationTest.Infrastructure
{
    public class BaseFilterProvider : IFilterProvider
    {
        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration,
            HttpActionDescriptor actionDescriptor)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration), "Configuration is null");

            if (actionDescriptor == null)
                throw new ArgumentNullException(nameof(actionDescriptor), "ActionDescriptor is null");

            var actionFilters = actionDescriptor.GetFilters()
                .Select(i => new BaseFilterInfo(i, FilterScope.Controller));
            var controllerFilters = actionDescriptor.ControllerDescriptor.GetFilters()
                .Select(i => new BaseFilterInfo(i, FilterScope.Controller));

            return controllerFilters.Concat(actionFilters).OrderBy(i => i)
                .Select(i => i.ConvertToFilterInfo());
        }
    }
}