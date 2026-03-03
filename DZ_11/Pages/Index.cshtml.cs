using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DZ_11.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public WorkoutSignUp WorkoutSignUp { get; set; } = new WorkoutSignUp();
        public string Message { get; private set; } = "Регистрация в фитнес клуб";

        // ❗ Флаг: показывать ли результат вместо формы
        public bool ShowResult { get; private set; } = false;
        public SelectList Trainers { get; private set; } = new SelectList(
            new[]
            {
                "Иванов Иван Иванович",
                "Петрова Мария Сергеевна",
                "Смирнов Алексей Дмитриевич",
                "Кузнецова Елена Владимировна",
                "Соколов Денис Андреевич"
            }
        );

        public IActionResult OnPost()
        {
            // ❗ Проверяем валидацию перед обработкой данных
            if (!ModelState.IsValid)
            {
                // Возвращаем страницу с ошибками валидации
                return Page();
            }

            // Обработка данных только если всё валидно
            if (WorkoutSignUp.Height is not null && WorkoutSignUp.Weight is not null)
            {
                WorkoutSignUp.BMI = Math.Round(
                    (double)(WorkoutSignUp.Weight / (WorkoutSignUp.Height * WorkoutSignUp.Height)), 1);
            }

            Message = $"Пользователь: {WorkoutSignUp.FIO} ({WorkoutSignUp.TrainingLevel}) зарегистрирован";
            // ❗ Устанавливаем флаг для отображения карточки
            ShowResult = true;

            return Page();
        }
    }

    public class WorkoutSignUp
    {
        [Required(ErrorMessage = "Поле ФИО обязательно для заполнения")]
        [StringLength(200, ErrorMessage = "ФИО должны содержать от 3 до 200 символов")]
        [Display(Name = "ФИО")]
        public string FIO { get; set; }

        [Required(ErrorMessage = "Поле возраст обязательно для заполнения")]
        [Range(10, 120, ErrorMessage = "Возраст должен быть от 10 до 120 лет")]
        [Display(Name = "Возраст")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Поле пол обязательно для заполнения")]
        [Display(Name = "Пол")]
        public Gender? Gender { get; set; }

        [Range(0, 3.0, ErrorMessage = "Рост должен быть от 0 до 3,0 м")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Рост, м")]
        public double? Height { get; set; }

        [Range(0, 200, ErrorMessage = "Вес должен быть от 0 до 200кг")]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        [Display(Name = "Вес, кг")]
        public double? Weight { get; set; }

        [Display(Name = "ИМТ (индекс массы тела)")]
        public double BMI { get; set; }

        [Required(ErrorMessage = "Поле уровень подготовки обязательно для заполнения")]
        [Display(Name = "Уровень подготовки")]
        public TrainingLevelType TrainingLevel { get; set; }

        [Display(Name = "Тренер")]
        public string? Trainer { get; set; }

        [Required(ErrorMessage = "Поле абонемeнт обязательно для заполнения")]
        [Display(Name = "Абонемнт")]
        public AbonementType? Abonement {  get; set; }

        [Required(ErrorMessage = "Поле телефон обязательно для заполнения")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Согласие на обработку персональных данных")]
        public bool ConsentProcessing {  get; set; }
    }

    public enum Gender
    {
        [Display(Name = "Мужской")]
        Male = 1,
        [Display(Name = "Женский")]
        Female = 2
    }

    public enum TrainingLevelType
    {
        [Display(Name = "Начинающий")]
        Beginner,
        [Display(Name = "Средний")]
        Intermediate,
        [Display(Name = "Продвинутый")]
        Advanced
    }
    public enum AbonementType
    {
        [Display(Name = "4 занятия")]
        Four,
        [Display(Name = "8 занятий")]
        Eight,
        [Display(Name = "Безлимит")]
        Unlimited
    }
}
