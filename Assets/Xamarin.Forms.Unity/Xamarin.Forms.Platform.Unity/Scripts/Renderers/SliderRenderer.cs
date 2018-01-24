using System.ComponentModel;

namespace Xamarin.Forms.Platform.Unity
{
    public class SliderRenderer : ViewRenderer<Slider, UnityEngine.UI.Slider>
    {
        /*-----------------------------------------------------------------*/

        /*-----------------------------------------------------------------*/

        #region MonoBehavior

        protected override void Awake()
        {
            base.Awake();

            var slider = Control;
            if (slider != null)
            {
                slider.OnValueChangedAsObservable()
                    .BlockReenter()
                    .Subscribe(value =>
                    {
                        if (Element != null)
                        {
                            Element.Value = value;
                        }
                    }).AddTo(slider);

                slider.ObserveEveryValueChanged(x => x.minValue)
                    .BlockReenter()
                    .Subscribe(value =>
                    {
                        if (Element != null)
                        {
                            Element.Minimum = value;
                        }
                    }).AddTo(slider);

                slider.ObserveEveryValueChanged(x => x.maxValue)
                    .BlockReenter()
                    .Subscribe(value =>
                    {
                        if (Element != null)
                        {
                            Element.Maximum = value;
                        }
                    }).AddTo(slider);
            }
        }

        #endregion MonoBehavior

        /*-----------------------------------------------------------------*/

        #region Event Handler

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                UpdateValue();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Slider.ValueProperty.PropertyName)
                UpdateValue();
            else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
                UpdateMinimum();
            else if (e.PropertyName == Slider.MaximumProperty.PropertyName)
                UpdateMaximum();
            base.OnElementPropertyChanged(sender, e);
        }

        #endregion Event Handler

        /*-----------------------------------------------------------------*/

        #region Internals

        private void UpdateValue()
        {
            if (Control != null && Element != null)
            {
                Control.value = (float)Element.Value;
            }
        }

        private void UpdateMinimum()
        {
            if (Control != null && Element != null)
            {
                Control.minValue = (float)Element.Minimum;
            }
        }

        private void UpdateMaximum()
        {
            if (Control != null && Element != null)
            {
                Control.maxValue = (float)Element.Maximum;
            }
        }

        #endregion Internals
    }
}