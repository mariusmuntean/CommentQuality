using System;
using CommentQuality.OouiForms.Views;
using Microsoft.AspNetCore.Mvc;
using Ooui.AspNetCore;
using Xamarin.Forms;

namespace CommentQuality.AspNetCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Comments()
        {
            try
            {
                return new ElementResult(new HomePage().GetOouiElement());
            }
            catch (Exception ex)
            {
                return new OkObjectResult($"Oops {ex}");
            }
        }
    }
}