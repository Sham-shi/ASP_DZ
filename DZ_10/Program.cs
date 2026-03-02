using DZ_10;
using DZ_10.TagHelpers;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// ✅ Правильная регистрация типизированного клиента
builder.Services.AddHttpClient<IGeocoderService, OpenStreetMapGeocoder>(client =>
{
    client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/"); // Без пробелов!
    client.DefaultRequestHeaders.Add("User-Agent", "MyGeoApp/1.0 (your-email@example.com)");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Регистрация тег-хелпера (опционально, обычно не требуется)
builder.Services.AddTransient<GeocodeTagHelper>();


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
