using System.ComponentModel;

namespace Xamarin.Forms.Platform.Unity
{
    public class LabelRenderer : ViewRenderer<Label, UnityEngine.UI.Text>
    {
        /*-----------------------------------------------------------------*/

        #region Field

        private TextTracker _componentText;

        #endregion Field

        /*-----------------------------------------------------------------*/

        #region MonoBehavior

        protected override void Awake()
        {
            base.Awake();

            _componentText = new TextTracker(Control);
        }

        #endregion MonoBehavior

        /*-----------------------------------------------------------------*/

        #region Event Handler

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                //_isInitiallyDefault = Element.IsDefault();

                UpdateText();
                UpdateColor();
                UpdateAlign();
                UpdateFont();
                UpdateLineBreakMode();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Label.TextProperty.PropertyName ||
                e.PropertyName == Label.FormattedTextProperty.PropertyName)
            {
                UpdateText();
            }
            else if (e.PropertyName == Label.TextColorProperty.PropertyName)
            {
                UpdateColor();
            }
            else if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
                e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
            {
                UpdateAlign();
            }
            else if (e.PropertyName == Label.FontSizeProperty.PropertyName ||
                e.PropertyName == Label.FontAttributesProperty.PropertyName)
            {
                UpdateFont();
            }
            else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
            {
                UpdateLineBreakMode();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        #endregion Event Handler

        /*-----------------------------------------------------------------*/

        #region Internals

        private void UpdateText()
        {
            //_perfectSizeValid = false;
            _componentText.UpdateText(Element.Text);
        }

        private void UpdateColor()
        {
            _componentText.UpdateTextColor(Element.TextColor);
        }

        private void UpdateFont()
        {
            _componentText.UpdateFont(Element);
        }

        private void UpdateLineBreakMode()
        {
            Control.horizontalOverflow = Element.LineBreakMode.ToUnityHorizontalWrapMode();
        }

        private void UpdateAlign()
        {
            Control?.SetTextAlign(Element);
        }

        #endregion Internals
    }
}