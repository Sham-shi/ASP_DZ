using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DZ_8.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Cities"] = new List<string>
            {
                "Москва", "Токио", "Нью-Йорк", "Лондон", "Париж",
                "Пекин", "Стамбул", "Шанхай", "Дели", "Сан-Паулу"
            };
        }
    }
}
