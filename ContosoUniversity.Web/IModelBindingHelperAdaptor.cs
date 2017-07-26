using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContosoUniversity.Web
{
    public interface IModelBindingHelperAdaptor
    {
        Task<bool> TryUpdateModelAsync<TModel>(ControllerBase controller, TModel model, string prefix, params Expression<Func<TModel, object>>[] includeExpressions) where TModel : class;
        Task<bool> TryUpdateModelAsync<TModel>(ControllerBase controller, TModel model) where TModel : class;
    }
}
