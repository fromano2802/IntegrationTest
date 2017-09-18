using System;
using System.Web.Http.Filters;

namespace IntegrationTest.Infrastructure
{
    public class BaseFilterInfo : IComparable
    {
        public BaseFilterInfo(IFilter instance, FilterScope scope)
        {
            Instance = instance;
            Scope = scope;
        }

        public IFilter Instance { get; set; }
        public FilterScope Scope { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is BaseFilterInfo)
            {
                var item = (BaseFilterInfo) obj;

                if (item.Instance is IBaseAttribute)
                {
                    var attr = item.Instance as IBaseAttribute;
                    var baseAttribute = Instance as IBaseAttribute;
                    if (baseAttribute != null)
                        return baseAttribute.Position.CompareTo(attr.Position);
                }
                return 0;
            }
            throw new ArgumentException("Object is of wrong type");
        }

        public FilterInfo ConvertToFilterInfo()
        {
            return new FilterInfo(Instance, Scope);
        }
    }
}