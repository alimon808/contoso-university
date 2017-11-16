using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace ContosoUniversity.Web.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; private set; } = "PageModel in C#";
        public void OnGet()
        {
            Message += $" Server time is {DateTime.Now}";
        }
    }
}