using Autofac;
using Autofac.Extensions.DependencyInjection;

/*
На базе примера ASP_Map реализовать несколько маршрутов с DI (Autofac)
по городам (странам)
1) широта / долгота
2) погода
3) площадь
4) население
5) архитектурные памятники (м.б. фото)
6) любая информация на ваш выбор
*/

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(cb =>
{
    cb.Register(c => new CoordinateService()).As<ICoordinateService>().InstancePerLifetimeScope();
});

var app = builder.Build();

Dictionary<string, string> citiesCoordinates = new()
    {
        {"Москва", "/coord/moscow" },
        {"Казань", "/coord/kazan" },
        {"Владивосток", "/coord/vladivostok" },
        {"Якутск", "/coord/yakutsk" },
        {"Нью Йорк", "/coord/newYork" }
    };


app.MapGet("/", () => "Координаты городов\n\n" + string.Join("\n", citiesCoordinates.Select(c => $"{c.Key}-> {c.Value}")));

app.MapGet("/coord/{city}", (string city, ILifetimeScope lifetimeScope) => 
{
    if (lifetimeScope.TryResolve<ICoordinateService>(out var service))
    {
        return Results.Ok(service.GetCoordinates(city));
    }

    return Results.NotFound("Город не найден. Доступные: moscow, kazan, vladivostok, yakutsk, newYork");
});

app.Run();

interface ICoordinateService
{
    string GetCoordinates(string city);
}

class CoordinateService : ICoordinateService
{
    public string GetCoordinates(string city)
    {
        string cityCoordinate = city.ToLowerInvariant() switch
        {
            "moscow" => "Москва: 55°45′21″ с. ш., 37°37′04″ в. д.",
            "kazan" => "Казань: 55°47′27″ с. ш., 49°06′52″ в. д.",
            "vladivostok" => "Владивосток: 43°06′54″ с. ш., 131°53′07″ в. д.",
            "yakutsk" => "Якутск: 62°01′38″ с. ш., 129°43′55″ в. д.",
            "newYork" => "Нью Йорк: 40°43′42″ с. ш., 73°59′39″ з. д.",
            _ => "такой город не найден"
        };

        return $"Географические координаты города {cityCoordinate}";
    }
}