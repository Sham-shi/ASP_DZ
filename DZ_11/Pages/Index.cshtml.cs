using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Rendering;

//ФИО(input text)
//Возраст(input number 16-100)
//Пол(radio)
//Рост(input number)
//Вес(input number)
//ИМТ(meter автоматически рассчитывается)
//Уровень подготовки(radio: новичок/средний/продвинутый)
//Цель тренировок(select: похудение/набор массы/поддержание формы)
//Предпочтительные дни (checkbox: пн, вт, ср...)
//Предпочтительное время (radio: утро/день/вечер)
//Тип тренировок(checkbox: кардио, силовые, йога, пилатес)
//Противопоказания (textarea)
//Травмы в анамнезе (checkbox)
//Тренер (select)
//Абонемент (radio: 4 занятия/8 занятий/безлимит)
//Телефон(input tel required)
//Email(input email)
//Согласие на обработку данных (checkbox)

namespace DZ_11.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public WorkoutSignUp WorkoutSignUp { get; set; } = new WorkoutSignUp();
        public string Message { get; private set; } = "Регистрация в фитнес клуб";

        public void OnPost()
        {
            if (WorkoutSignUp.Height is not null && WorkoutSignUp.Weight is not null)
            {
                WorkoutSignUp.BMI = Math.Round((double)(WorkoutSignUp.Weight / (WorkoutSignUp.Height * WorkoutSignUp.Height)), 1);
            }

            Message = $"Пользователь: {WorkoutSignUp.FIO} ({WorkoutSignUp.TrainingLevel}) зарегистрирован";
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
        public Gender Gender { get; set; }

        [Range(0, 3.0, ErrorMessage = "Рост должен быть от 0 до 3,0 м")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Рост, м")]
        public double? Height { get; set; }

        [Range(0, 300, ErrorMessage = "Вес должен быть положительным")]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        [Display(Name = "Вес, кг")]
        public double? Weight { get; set; }

        [Display(Name = "ИМТ (индекс массы тела)")]
        public double BMI { get; set; }

        [Required(ErrorMessage = "Поле уровень подготовки обязательно для заполнения")]
        [Display(Name = "Уровень подготовки")]
        public TrainingLevelType TrainingLevel { get; set; }

        [Required(ErrorMessage = "Поле цель обязательно для заполнения")]
        [Display(Name = "Цель")]
        public List<string> Purpose { get; } =
        [
            "Похудение",
            "Набор массы",
            "Поддержание формы"
        ];

        [Display(Name = "Предпочтительные дни")]
        public HashSet<WeekDays>? WeekDay { get; set; }

        [Required(ErrorMessage = "Поле противопоказания обязательно для заполнения")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Противопоказания должны содержать от 3 до 1000 символов")]
        [Display(Name = "Противопоказания")]
        public string Contraindications { get; set; }

        [Display(Name = "Тренер")]
        public string? TreinerPhotoPath { get; set; }

        [Required(ErrorMessage = "Поле абонемнт обязательно для заполнения")]
        [Display(Name = "Абонемнт")]
        public AbonementType Abonement {  get; set; }

        [Required(ErrorMessage = "Поле телефон обязательно для заполнения")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Согласие на обработку персональных данных")]
        public bool ConsentProcessing {  get; set; }
    }

    public enum Gender
    {
        Male = 1,
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

    public enum WeekDays
    {
        [Display(Name = "ПН")]
        Monday,
        [Display(Name = "ВТ")]
        Tuesday,
        [Display(Name = "СР")]
        Wednesday,
        [Display(Name = "ЧТ")]
        Thursday,
        [Display(Name = "ПТ")]
        Friday,
        [Display(Name = "СБ")]
        Saturday,
        [Display(Name = "ВС")]
        Sunday
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
