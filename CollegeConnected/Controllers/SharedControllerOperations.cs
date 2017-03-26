using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CollegeConnected.Controllers
{
    public class SharedControllerOperations : Controller
    {
        public SelectList GenerateGradYearList()
        {
            return new SelectList(Enumerable.Range(1940, 100).Select(x =>
                new SelectListItem
                {
                    Text = x.ToString(),
                    Value = x.ToString()
                }), "Value", "Text"); ;
        }

        public bool IsAuthenticated(HttpCookie cookie)
        {
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);

                if (ticket != null)
                    return true;
            }
            return false;
        }
    }
}