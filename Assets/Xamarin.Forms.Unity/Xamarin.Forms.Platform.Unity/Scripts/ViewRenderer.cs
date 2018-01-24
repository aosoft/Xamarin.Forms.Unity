using System.ComponentModel;

namespace Xamarin.Forms.Platform.Unity
{
    public class ViewRenderer<TElement, TNativeElement> :
        VisualElementRenderer<TElement, TNativeElement>
        where TElement : View
        where TNativeElement : UnityEngine.Component
    {
        /*-----------------------------------------------------------------*/

        /*-----------------------------------------------------------------*/

        /*-----------------------------------------------------------------*/

        #region Event Handler

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == View.HorizontalOptionsProperty.PropertyName ||
                e.PropertyName == View.VerticalOptionsProperty.PropertyName)
            {
                UpdateNativeControl();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                UpdateBackgroundColor();
            }
        }

        #endregion Event Handler

        /*-----------------------------------------------------------------*/
    }
}