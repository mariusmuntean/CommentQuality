using CommentQuality.OouiForms;
using Microsoft.AspNetCore.Mvc;
using Ooui.AspNetCore;
using Xamarin.Forms;

namespace CommentQuality.AspNetCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Comments()
        {
            
            return new ElementResult(new Comments().GetOouiElement());

        }
    }
}