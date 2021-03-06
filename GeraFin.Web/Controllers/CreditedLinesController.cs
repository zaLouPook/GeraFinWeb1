﻿using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.CreditedLines.RoleName)]
    public class CreditedLinesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}