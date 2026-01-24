using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DZ_4.Pages
{
    public class IndexModel : PageModel
    {
        public double Ounces { get; set; }
        public double Kilos { get; set; }
        public string? Output { get; set; } = "";

        public void OnGet(string? kg)
        {
            if (kg is null)
                return;

            if (double.TryParse(kg, out double result))
            {
                Kilos = result;
                Ounces = Math.Round(ConvertKgToOunce(result), 3);

                Output = $"{Kilos} кг = {Ounces} унций";
            }
            else
            {
                Output = "Некорректный ввод";
            }
        }

        private double ConvertKgToOunce(double weight)
        {
            return weight * 35.27396;
        }
    }
}
