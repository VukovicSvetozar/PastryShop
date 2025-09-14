using System.Windows.Markup;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace PastryShop.Utility
{
    [MarkupExtensionReturnType(typeof(SolidColorPaint))]
    public class SolidColorPaintExtension : MarkupExtension
    {
        public string Hex { get; set; } = "#FF000000";
        public byte Alpha { get; set; } = 0xFF;
        public bool Bold { get; set; } = false;
        public float StrokeThickness { get; set; } = 1;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var color = SKColor.Parse(Hex);
            color = new SKColor(color.Red, color.Green, color.Blue, Alpha);
            var typeface = Bold
                ? SKTypeface.FromFamilyName(
                    null,
                    SKFontStyleWeight.SemiBold,
                    SKFontStyleWidth.Normal,
                    SKFontStyleSlant.Upright)
                : SKTypeface.Default;

            return new SolidColorPaint(color)
            {
                StrokeThickness = StrokeThickness,
                SKTypeface = typeface
            };
        }

    }
}