using Microsoft.AspNetCore.Razor.TagHelpers;

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
                    $"<div class='alert alert-warning'>Город не найден в базе данных: {EscapeHtml(City)}</div>");
                return;
            }

            var content = $@"
            <h4>Координаты города: {EscapeHtml(location.City)}</h4>
            <ul class='list-unstyled'>
                <li><strong>Широта:</strong> {location.Latitude:F6}</li>
                <li><strong>Долгота:</strong> {location.Longitude:F6}</li>
                <li><strong>Страна:</strong> {EscapeHtml(location.Country)}</li>
                {(string.IsNullOrEmpty(location.State) ? "" : $"<li><strong>Регион:</strong> {EscapeHtml(location.State)}</li>")}
            </ul>";

            output.Content.SetHtmlContent(content);
        }

        private string EscapeHtml(string str) =>
            str.Replace("&", "&amp;")
               .Replace("<", "&lt;")
               .Replace(">", "&gt;")
               .Replace("\"", "&quot;")
               .Replace("'", "&#39;");
    }
}