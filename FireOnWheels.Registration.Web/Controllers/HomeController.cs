using FireOnWheels.Registration.Web.Messages;
using FireOnWheels.Registration.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FireOnWheels.Registration.Web.Controllers
{
    public class HomeController : Controller
    {
        public RabbitMqManager RabbitMqManager { get; set; }

        public HomeController()
        {
            RabbitMqManager = new RabbitMqManager();
        }

        public IActionResult Index()
        {
            return View("RegisterOrder");
        }

        [HttpPost]
        public IActionResult RegisterOrder(OrderViewModel model)
        {
            var registerOrderCommand = new RegisterOrderCommand(model);

            //Send RegisterOrderCommand
            RabbitMqManager.SendRegisterOrderCommand(registerOrderCommand);

            return View("Thanks");
        }
    }
}
