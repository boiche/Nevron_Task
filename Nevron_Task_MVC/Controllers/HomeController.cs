using Microsoft.AspNetCore.Mvc;
using Nevron_Task_MVC.Models;
using System.Text.Json;

namespace Nevron_Task_MVC.Controllers
{
    public class HomeController : Controller
    {
        private const string _numbersKey = "numbers";
        private const string _sumKey = "sum";

        public HomeController() { }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ClearNumbers()
        {
            HttpContext.Session.Clear();

            return GetPartial(new NumbersModel());
        }

        [HttpGet]
        public IActionResult SumNumbers()
        {
            var numbers = GetSessionValue<List<int>>(_numbersKey) ?? [];
            SetSessionValue(_sumKey, numbers.Sum());

            return GetPartial(new NumbersModel()
            { 
                Numbers = numbers,
                Sum = GetSessionValue<int>(_sumKey)
            });
        }

        [HttpGet]
        public IActionResult AddNumber()
        {
            int next = new Random().Next(0, 1000);

            var numbers = GetSessionValue<List<int>>(_numbersKey) ?? [];
            numbers.Add(next);
            SetSessionValue(_numbersKey, numbers);

            return GetPartial(new NumbersModel()
            { 
                Numbers = numbers,
                Sum = GetSessionValue<int>(_sumKey)
            });
        }

        private PartialViewResult GetPartial(NumbersModel model)
        {
            return PartialView("_Numbers", model);
        }

        private void SetSessionValue<T>(string key, T value)
        {
            if (typeof(T).IsAssignableFrom(typeof(int)))
            {
                HttpContext.Session.SetInt32(key, int.Parse(value?.ToString() ?? "0"));
            }
            else
            {
                var json = JsonSerializer.Serialize(value);
                HttpContext.Session.SetString(key, json);
            }
        }

        private T? GetSessionValue<T>(string key)
        {
            if (typeof(T).IsAssignableFrom(typeof(int)))
            {
                return (T)Convert.ChangeType(HttpContext.Session.GetInt32(_sumKey) ?? 0, typeof(T));
            }
            else
            {
                var json = HttpContext.Session.GetString(key);

                if (json is null)
                    return default;
                return JsonSerializer.Deserialize<T>(json);
            }
        }
    }
}
