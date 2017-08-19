using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.Web.Helpers
{
    public class UrlHelperAdaptor : IUrlHelperAdaptor
    {
        //
        // Summary:
        //     Generates a URL with an absolute path for an action method, which contains the
        //     specified action name, controller name, route values, and protocol to use.
        //
        // Parameters:
        //   helper:
        //     The Microsoft.AspNetCore.Mvc.IUrlHelper.
        //
        //   action:
        //     The name of the action method.
        //
        //   controller:
        //     The name of the controller.
        //
        //   values:
        //     An object that contains route values.
        //
        //   protocol:
        //     The protocol for the URL, such as "http" or "https".
        //
        // Returns:
        //     The generated URL.
        public string Action(IUrlHelper helper, string action, string controller, object values, string protocol)
        {
            var url = GenerateUrl(helper, action, controller, values, protocol);
            return url;
        }

        protected virtual string GenerateUrl(IUrlHelper helper, string action, string controller, object values, string protocol)
        {
            return helper.Action(action, controller, values, protocol);
        }
    }
}
