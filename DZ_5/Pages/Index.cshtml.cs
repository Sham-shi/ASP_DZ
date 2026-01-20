using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DZ_5.Pages
{
    public class IndexModel : PageModel
    {
        public List<long> FibonacciNumbers { get; private set; } = [0, 1];

        public string Message { get; private set; } = "";
        public string Output { get; private set; } = "";

        [BindProperty(SupportsGet = true)]
        public int FibonacciCount { get; set; } = 2;

        public void OnGet()
        {
            Message = "Введите количество чисел Фибоначчи";
            Fibonacci();

            Output = string.Join(" ", FibonacciNumbers);
        }

        public void OnPost()
        {
            Fibonacci();

            Output = string.Join(" ", FibonacciNumbers);
        }

        private void Fibonacci()
        {
            for (int i = 2; i <= FibonacciCount; i++)
            {
                FibonacciNumbers.Add(FibonacciNumbers[i - 2] + FibonacciNumbers[i - 1]);
            }
        }
    }
}
