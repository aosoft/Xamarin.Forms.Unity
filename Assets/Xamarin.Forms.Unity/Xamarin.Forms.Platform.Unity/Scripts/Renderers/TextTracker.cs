using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
    internal struct TextTracker
    {
        private UnityEngine.UI.Text _componentText;
        private UnityEngine.Color _defaultTextColor;

        public TextTracker(UnityEngine.UI.Text componentText)
        {
            //	初期値がデフォルトカラー

            _componentText = componentText;
            _defaultTextColor = componentText != null ? componentText.color : new UnityEngine.Color();
        }

        public void UpdateText(string text)
        {
            if (_componentText != null)
            {
                _componentText.text = text;
            }
        }

        public void UpdateTextColor(Color color)
        {
            _componentText?.SetTextColor(color, _defaultTextColor);
        }

        public void UpdateFont(IFontElement element)
        {
            _componentText?.SetFont(element);
        }
    }
}