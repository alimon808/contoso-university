using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.Web.Helpers
{
    public interface IUrlHelperAdaptor
    {
        string Action(IUrlHelper helper, string action, string controller, object values, string protocol);
    }
}