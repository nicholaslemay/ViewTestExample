using Microsoft.AspNetCore.Mvc;

namespace ExampleWebApplication.Players
{
    public class PlayersController : Controller
    {
        public ViewResult Show(int id)
        {
            return View("~/Players/Views/_Player.cshtml", new HockeyPlayer(Number:id, "Some dude"));
        }
    }
}