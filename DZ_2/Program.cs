using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.Indexed;
using Microsoft.AspNetCore.Mvc;

/*
Реализуйте через DI (Autofac) вычисление функций
1) встроенными методами класса Math
2) по ряду Тейлора
Функции:
1. sin(x)
2. cos(x)
3. tan(x)
4. ln(x) натуральный логарифм
5. e(x) экспонента
*/

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(cb =>
{
    cb.RegisterType<MathCalculateService>().Named<ICalculateService>("math");
    cb.RegisterType<TaylorSeriesCalculateService>().Named<ICalculateService>("taylor");
});

var app = builder.Build();

app.MapGet("/", () => Results.Text(@"
Калькулятор математических функций

Формат: /calculate/{method}/{function}/{x}

Методы:
  - math     : встроенные функции System.Math
  - taylor   : вычисление через ряд Тейлора

Функции:
  - sin      : синус
  - cos      : косинус
  - tan      : тангенс
  - ln       : натуральный логарифм
  - exp      : экспонента

Примеры:
  /calculate/math/sin/
  /calculate/taylor/sin/
  /calculate/math/cos/
  /calculate/taylor/cos/
  /calculate/math/ln/
  /calculate/taylor/ln/
  /calculate/math/tan/
  /calculate/taylor/tan/
  /calculate/math/exp/
  /calculate/taylor/exp/

Параметр x должен быть числом (например: 3.14, -2, 0.5).
", contentType: "text/plain; charset=utf-8"));

app.MapGet("/calculate/{method}/{function}/{x:double}", (
    string method,
    string function,
    double x,
    [FromServices] IIndex<string, ICalculateService> calculatorIndex) =>
{
    if (!calculatorIndex.TryGetValue(method.ToLowerInvariant(), out var service))
    {
        return Results.BadRequest($"Неизвестный метод: {method}. Допустимые: math, taylor");
    }

    return function.ToLowerInvariant() switch
    {
        "sin" => Results.Ok(service.Sin(x)),
        "cos" => Results.Ok(service.Cos(x)),
        "tan" => Results.Ok(service.Tan(x)),
        "ln" => Results.Ok(service.Ln(x)),
        "exp" => Results.Ok(service.Exp(x)),
        _ => Results.BadRequest("Неизвестная функция. Допустимые: sin, cos, tan, ln, exp")
    };
});

app.Run();

public interface ICalculateService
{
    string GetName();
    string Sin(double x);
    string Cos(double x);
    string Tan(double x);
    string Ln(double x);
    string Exp(double x);
}

class MathCalculateService : ICalculateService
{
    public string GetName() => "Math";

    public string Sin(double x) => $"{GetName()}: sin({x}) = {Math.Sin(x)}";
    public string Cos(double x) => $"{GetName()}: cos({x}) = {Math.Cos(x)}";
    public string Tan(double x) => $"{GetName()}: tan({x}) = {Math.Tan(x)}";
    public string Ln(double x) => x <= 0
        ? "Ошибка: ln(x) определён только для x > 0"
        : $"{GetName()}: ln({x}) = {Math.Log(x)}";
    public string Exp(double x) => $"{GetName()}: e({x}) = {Math.Exp(x)}";
}

public class TaylorSeriesCalculateService : ICalculateService
{
    private const double Epsilon = 0.0001;

    public string GetName() => "Ряд Тейлора";

    public string Sin(double x) => $"{GetName()}: sin({x}) = {ComputeSinValue(x)}";
    public string Cos(double x) => $"{GetName()}: cos({x}) = {ComputeCosValue(x)}";

    public string Tan(double x)
    {
        double piHalf = Math.PI / 2;
        double normalized = x % Math.PI;
        if (Math.Abs(normalized - piHalf) < 0.001 || Math.Abs(normalized + piHalf) < 0.001)
            return "Ошибка: tan(x) не определён в этой точке (близко к π/2 + kπ)";

        double sin = ComputeSinValue(x);
        double cos = ComputeCosValue(x);

        if (Math.Abs(cos) < 1e-10)
            return "Ошибка: деление на ноль (cos(x) ≈ 0)";

        return $"{GetName()}: tan({x}) = {sin / cos}";
    }

    public string Ln(double x)
    {
        if (x <= 0)
            return "Ошибка: ln(x) определён только для x > 0";

        double y = (x - 1) / (x + 1);
        double y2 = y * y;
        double term = y;
        double result = term;
        int n = 1;

        while (Math.Abs(term) > Epsilon)
        {
            term *= y2;
            double nextTerm = term / (2 * n + 1);
            result += nextTerm;
            n++;
        }

        result *= 2;
        return $"{GetName()}: ln({x}) = {result}";
    }

    public string Exp(double x)
    {
        double result = 0;
        double current = 1;
        double factorial = 1;
        double power = 1;
        int n = 1;

        while (current > Epsilon)
        {
            current = power / factorial;
            result += current;
            power *= x;
            factorial *= n;
            n++;
        }

        return $"{GetName()}: e({x}) = {result}";
    }

    private double ComputeSinValue(double x)
    {
        double result = 0, lastResult = double.NegativeInfinity, a = x;
        for (int i = 0; Math.Abs(result - lastResult) > Epsilon; i++)
        {
            lastResult = result;
            result += a;
            a *= -x * x / (2 * (2 * i + 3) * (i + 1));
        }
        return result;
    }

    private double ComputeCosValue(double x)
    {
        double result = 0, nextStep = 1;
        var i = 0;
        while (Math.Abs(nextStep) > Epsilon)
        {
            result += nextStep;
            nextStep *= -x * x / ++i / ++i;
        }
        return result;
    }
}