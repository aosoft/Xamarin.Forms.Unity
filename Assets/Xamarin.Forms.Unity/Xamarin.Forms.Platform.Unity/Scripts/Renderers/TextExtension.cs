using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
    internal static class TextExtension
    {
        static public void SetTextColor(
            this UnityEngine.UI.Text self,
            Color color,
            UnityEngine.Color defaultColor)
        {
            if (self != null)
            {
                if (color != Color.Default)
                {
                    self.color = color.ToUnityColor();
                }
                else
                {
                    self.color = defaultColor;
                }
            }
        }

        static public void SetFont(
            this UnityEngine.UI.Text self,
            IFontElement element)
        {
            if (self != null)
            {
                self.fontSize = (int)element.FontSize;
                self.fontStyle = element.FontAttributes.ToUnityFontStyle();
            }
        }

        static public void SetTextAlign(
            this UnityEngine.UI.Text self,
            Label label)
        {
            if (self != null && label != null)
            {
                self.alignment = Platform.ToUnityTextAnchor(label.HorizontalTextAlignment, label.VerticalTextAlignment);
            }
        }
    }
}