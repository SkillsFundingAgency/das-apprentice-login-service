using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Controllers.CreateAccount
{
    public class CreateAccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
