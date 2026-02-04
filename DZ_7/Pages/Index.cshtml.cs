using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace DZ_7.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Handler { get; set; }

        public string FileContent { get; set; } = string.Empty;

        private readonly Dictionary<string, string> poetNames = new()
        {
            {"esenin", "Esenin_Sergey"},
            {"mayakovsky", "Mayakovsky_Vladimir"},
            {"valiullin", "Valiullin_Rinat"}
        };

        public IActionResult OnGetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                FileContent = "<div class='alert alert-danger'><strong>Ошибка:</strong> Пустая строка</div>";
                return Page();
            }

            if (poetNames.ContainsKey(name))
            {
                return ShowFile(poetNames[name]);
            }
            else
            {
                FileContent = $@"
                    <div class='alert alert-warning'>
                        <h5>Автор '{name}' не найден</h5>
                        <a href='https://yandex.ru/search/?text={Uri.EscapeDataString(name)}' target='_blank' class='alert-link'>Поиск в Яндексе</a>
                    </div>";
                return Page();
            }
        }

        public IActionResult OnGetSaveFile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                FileContent = "<div class='alert alert-danger'><strong>Ошибка:</strong> Имя автора не указано</div";
                return Page();
            }

            if (!poetNames.TryGetValue(name, out string? fileName))
            {
                FileContent = $"<div class='alert alert-danger'><strong>Ошибка:</strong> Автора '{name}' нет в нашей базе данных</div>";
                return Page();
            }

            try
            {
                string file_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Files/{fileName}.txt");
                string file_type = "text/plain";
                string file_name = $"{fileName}.txt";

                return PhysicalFile(file_path, file_type, file_name);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult OnGetJson(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                FileContent = "<div class='alert alert-danger'><strong>Ошибка:</strong> Имя автора не указано</div>";
                return Page();
            }

            if (!poetNames.TryGetValue(name, out string? fileName))
                return BadRequest($"Автора '{name}' нет в нашей базе данных.");

            //FileContent = ReadFileContent(fileName);
            var jsonData = new
            {
                Author = fileName,
                Text = ReadFileContent(fileName)
            };

            return new JsonResult(jsonData);
        }

        private IActionResult ShowFile(string fileName)
        {
            string content = ReadFileContent(fileName);
            FileContent = $"<pre class='card-text'>{content}</pre>";
            return Page();
        }

        private string ReadFileContent(string fileName)
        {
            try
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Files/{fileName}.txt");

                return System.IO.File.Exists(filePath)
                    ? System.IO.File.ReadAllText(filePath, Encoding.UTF8)
                    : $"Файл '{fileName}.txt' не найден.";
            }
            catch (Exception ex)
            {
                return $"Ошибка при чтении файла: {ex.Message}";
            }
        }
    }
}
