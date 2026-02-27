var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddTransient<IPopulationService, SimplePopulationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();


public interface IPopulationService
{
    string Population(string city);
}

public class SimplePopulationService : IPopulationService
{
    public string Population(string city) => city switch
    {
        "Москва" => "12 678 079",
        "Токио" => "37 274 000",
        "Нью-Йорк" => "8 804 190",
        "Лондон" => "8 799 728",
        "Париж" => "2 161 000",
        "Пекин" => "21 893 095",
        "Стамбул" => "15 462 452",
        "Шанхай" => "24 870 895",
        "Дели" => "32 941 000",
        "Сан-Паулу" => "12 396 372",
        _ => "Информация о населении отсутствует"
    };
}