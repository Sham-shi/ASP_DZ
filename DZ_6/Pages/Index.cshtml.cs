using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DZ_6.Pages
{
    public class IndexModel : PageModel
    {
        private Dictionary<string, Dish> dishes = new()
        {
            { "carbonara", new Dish("Паста Карбонара", 350.50m) },
            { "borscht", new Dish("Борщ", 220.00m) },
            { "caesar_chicken", new Dish("Цезарь с курицей", 280.75m) },
            { "margherita_pizza", new Dish("Пицца Маргарита", 450.00m) },
            { "olivier_salad", new Dish("Салат Оливье", 180.30m) }
        };

        public List<Dish> DisplayedDishes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Handler { get; set; }

        public void OnGet()
        {
            DisplayedDishes = [.. dishes.Values];
        }

        public void OnGetByName(string name)
        {
            //var name = _name.ToLowerInvariant();
            DisplayedDishes.Clear();
            DisplayedDishes.AddRange([.. dishes.Where(n => n.Key == name).Select(v => v.Value)]);
        }

        public void OnGetByCost(decimal cost)
        {
            DisplayedDishes.Clear();
            DisplayedDishes.AddRange([.. dishes.Where(k => k.Value.Cost == cost).Select(v => v.Value)]);
        }
    }

    public record class Dish(string Name, decimal Cost);
}
