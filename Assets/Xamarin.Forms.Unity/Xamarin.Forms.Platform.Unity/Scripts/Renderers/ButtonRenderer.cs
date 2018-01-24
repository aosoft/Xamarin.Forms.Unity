using System.ComponentModel;

namespace Xamarin.Forms.Platform.Unity
{
    public class ButtonRenderer : ViewRenderer<Button, UnityEngine.UI.Button>
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

            var button = Control;
            if (button != null)
            {
                button.OnClickAsObservable()
                    .Subscribe(_ => (Element as IButtonController)?.SendClicked())
                    .AddTo(button);
            }

            _componentText = new TextTracker(button.GetComponentInChildren<UnityEngine.UI.Text>());
        }

        #endregion MonoBehavior

        /*-----------------------------------------------------------------*/

        #region Event Handler

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                //_isInitiallyDefault = Element.IsDefault();

                UpdateText();
                UpdateTextColor();
                UpdateFont();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Button.TextProperty.PropertyName)
            {
                UpdateText();
            }
            else if (e.PropertyName == Button.TextColorProperty.PropertyName)
            {
                UpdateTextColor();
            }
            else if (e.PropertyName == Button.FontSizeProperty.PropertyName ||
                e.PropertyName == Button.FontAttributesProperty.PropertyName)
            {
                UpdateFont();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        #endregion Event Handler

        /*-----------------------------------------------------------------*/

        #region Internals

        private void UpdateText()
        {
            _componentText.UpdateText(Element.Text);
        }

        private void UpdateTextColor()
        {
            _componentText.UpdateTextColor(Element.TextColor);
        }

        private void UpdateFont()
        {
            _componentText.UpdateFont(Element);
        }

        #endregion Internals
    }
}