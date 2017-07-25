using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq;

namespace ContosoUniversity
{
    // see issue @ https://stackoverflow.com/questions/27038253/mock-asnotracking-entity-framework
    // workaround @ https://github.com/aspnet/EntityFramework/issues/7937
    public static class QueryableExtensions
    {
        public static IQueryable<T> AsGatedNoTracking<T>(this IQueryable<T> source) where T : class
        {
            if (source.Provider is EntityQueryProvider)
                return source.AsNoTracking<T>();
            return source;
        }
    }
}
