using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TopicTask.Models;
using TopicTask.Redis.Abstruction;
using TopicTask.Redis.Concrete;

namespace TopicTask.Controllers
{
    public class HomeController : Controller
    {
        private List<CategoryModel> categoryModels;

        private readonly IRedisManager redisManager;
        private readonly IRedisServer redisService;
        public HomeController(IRedisManager redisManager, IRedisServer redisService)
        {
            this.redisManager = redisManager;
            this.redisService = redisService;
        }

        public IActionResult Index()
        {
            categoryModels = this.redisManager.GetCategoryModelList();
            return View(categoryModels);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessageRabbitMQ([FromBody] SendMessageModel model)
        {

            var _ = await redisService.SendMessageTopic(model.DivisionName, model.Message);
            return Ok(new { message = "Message sent successfully!" });

        }
        [HttpPost]
        public IActionResult AddRabbitMQDivision(string name, string parentName)
        {
            if (name != null)
            {
                redisManager.SaveList(name, parentName);

            }

            return RedirectToAction("Index");
        }
    }
}
