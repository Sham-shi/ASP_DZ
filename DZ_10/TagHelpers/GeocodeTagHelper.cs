using Microsoft.AspNetCore.Razor.TagHelpers;
using DZ_10;

namespace DZ_10.TagHelpers
{
    [HtmlTargetElement("geocode", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GeocodeTagHelper : TagHelper
    {
        private readonly IGeocoderService _geocoderService;

        [HtmlAttributeName("city")]
        public string City { get; set; } = string.Empty;

        [HtmlAttributeName("show-map")]
        public bool ShowMap { get; set; } = false;

        [HtmlAttributeName("map-width")]
        public string MapWidth { get; set; } = "400px";

        [HtmlAttributeName("map-height")]
        public string MapHeight { get; set; } = "300px";

        public GeocodeTagHelper(IGeocoderService geocoderService)
        {
            _geocoderService = geocoderService;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(City))
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "geocode-result");

            var location = await _geocoderService.GetCoordinatesAsync(City);

            if (location == null)
            {
                output.Content.SetHtmlContent(
                    $"<div class='alert alert-warning'>Не удалось найти координаты для города: {City.EscapeHtml()}</div>");
                return;
            }

            // ✅ Исправлено: убраны пробелы в URL iframe
            var mapHtml = ShowMap
                ? $@"
                <div class='mt-3'>
                    <iframe 
                        width='{MapWidth}' 
                        height='{MapHeight}' 
                        style='border:1px solid #ccc; border-radius:4px;'
                        loading='lazy'
                        src='https://www.openstreetmap.org/export/embed.html?bbox={location.Longitude - 0.05}%2C{location.Latitude - 0.05}%2C{location.Longitude + 0.05}%2C{location.Latitude + 0.05}&layer=mapnik&marker={location.Latitude}%2C{location.Longitude}'>
                    </iframe>
                    <br/><small><a href='https://www.openstreetmap.org/?mlat={location.Latitude}&mlon={location.Longitude}#map=14/{location.Latitude}/{location.Longitude}'>Посмотреть на OpenStreetMap</a></small>
                </div>"
                : string.Empty;

            var content = $@"
            <h4>📍 Координаты города: {City.EscapeHtml()}</h4>
            <ul class='list-unstyled ms-3'>
                <li><strong>Широта:</strong> {location.Latitude:F6}</li>
                <li><strong>Долгота:</strong> {location.Longitude:F6}</li>
                <li><strong>Страна:</strong> {location.Country.EscapeHtml()}</li>
                {(string.IsNullOrEmpty(location.State) ? "" : $"<li><strong>Регион:</strong> {location.State.EscapeHtml()}</li>")}
            </ul>
            {mapHtml}";

            output.Content.SetHtmlContent(content);
        }
    }

    // Вспомогательный метод для экранирования HTML
    internal static class StringExtensions
    {
        public static string EscapeHtml(this string str) =>
            str.Replace("&", "&amp;")
               .Replace("<", "&lt;")
               .Replace(">", "&gt;")
               .Replace("\"", "&quot;")
               .Replace("'", "&#39;");
    }
}