using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace IntegrationTest.Infrastructure
{
    public interface IBaseAttribute : IFilter
    {
        int Position { get; set; }
    }

    public class BaseActionFilterAttribute : ActionFilterAttribute, IBaseAttribute
    {
        public int Position { get; set; }

        public BaseActionFilterAttribute()
        {
            Position = 0;
        }
        public BaseActionFilterAttribute(int positon)
        {
            Position = positon;
        }
    }

    public class BaseAuthenticationAttribute : Attribute, IAuthenticationFilter, IBaseAttribute
    {
        public int Position { get; set; }

        public BaseAuthenticationAttribute()
        {
            Position = 0;
        }
        public BaseAuthenticationAttribute(int position)
        {
            Position = position;
        }

        public virtual bool AllowMultiple => false;

        public virtual Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class BaseAuthorizationAttribute : AuthorizationFilterAttribute, IBaseAttribute
    {
        public int Position { get; set; }

        public BaseAuthorizationAttribute()
        {
            Position = 0;
        }
        public BaseAuthorizationAttribute(int position)
        {
            Position = position;
        }
    }

    public class BaseExceptionFilterAttribute : ExceptionFilterAttribute, IBaseAttribute
    {
        public int Position { get; set; }

        public BaseExceptionFilterAttribute()
        {
            Position = 0;
        }
        public BaseExceptionFilterAttribute(int positon)
        {
            Position = positon;
        }
    }
}