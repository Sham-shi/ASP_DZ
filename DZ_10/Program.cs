using DZ_10;
using DZ_10.TagHelpers;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Регистрация локального геокодера
builder.Services.AddSingleton<IGeocoderService, LocalGeocoder>();

var app = builder.Build();

// Проверка наличия базы данных при запуске
var dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "cities15000.txt");
if (File.Exists(dataPath))
{
    Console.WriteLine($"✅ База данных найдена: {dataPath}");
    Console.WriteLine($"   Размер: {new FileInfo(dataPath).Length / 1024} KB");
}
else
{
    Console.WriteLine($"❌ База данных НЕ НАЙДЕНА: {dataPath}");
    Console.WriteLine($"   Попробуйте скопировать файл вручную в эту папку");
}

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
