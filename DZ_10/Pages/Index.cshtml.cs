using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DZ_10;

namespace DZ_10.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IGeocoderService _geocoderService;

        public IndexModel(IGeocoderService geocoderService)
        {
            _geocoderService = geocoderService;
        }

        // Свойства для формы
        [BindProperty]
        public string CityName { get; set; } = string.Empty;

        // Результат геокодирования
        public GeoLocation? Location { get; set; }

        // Флаг успешного поиска
        public bool SearchPerformed { get; set; } = false;
        public bool CityNotFound { get; set; } = false;

        public void OnGet()
        {
            // При первом открытии страницы — ничего не делаем
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            SearchPerformed = true;
            CityNotFound = false;

            if (string.IsNullOrWhiteSpace(CityName))
            {
                ModelState.AddModelError("CityName", "Введите название города");
                return Page();
            }

            Location = await _geocoderService.GetCoordinatesAsync(CityName.Trim());

            if (Location == null)
            {
                CityNotFound = true;
                ModelState.AddModelError("CityName", $"Город '{CityName}' не найден в базе данных");
                return Page();
            }

            return Page();
        }
    }
}