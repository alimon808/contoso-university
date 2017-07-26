using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ContosoUniversity.Web
{
    // issue with unit testing controllers that use TryUpdateModelAsync
    // solution @ https://stackoverflow.com/questions/39259025/asp-net-core-mvc-controller-unit-testing-when-using-tryupdatemodel
    public class DefaultModelBindingHelaperAdaptor : IModelBindingHelperAdaptor
    {
        public Task<bool> TryUpdateModelAsync<TModel>(ControllerBase controller, TModel model, string prefix, params Expression<Func<TModel, object>>[] includeExpressions) where TModel : class
        {
            return controller.TryUpdateModelAsync(model, prefix, includeExpressions);
        }

        public Task<bool> TryUpdateModelAsync<TModel>(ControllerBase controller, TModel model) where TModel : class
        {
            return controller.TryUpdateModelAsync(model);
        }
    }
}
