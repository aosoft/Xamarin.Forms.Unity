﻿using System.ComponentModel;

namespace Xamarin.Forms.Platform.Unity
{
    public class EditorRenderer : ViewRenderer<Editor, UnityEngine.UI.InputField>
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

            var inputField = Control;
            if (inputField != null)
            {
                inputField.lineType = UnityEngine.UI.InputField.LineType.MultiLineNewline;
                inputField.OnValueChangedAsObservable()
                    .BlockReenter()
                    .Subscribe(value =>
                    {
                        var element = Element;
                        if (element != null)
                        {
                            element.Text = value;
                        }
                    }).AddTo(inputField);

                _componentText = new TextTracker(inputField.textComponent);
            }
        }

        #endregion MonoBehavior

        /*-----------------------------------------------------------------*/

        #region Event Handler

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
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
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Editor.TextProperty.PropertyName)
            {
                UpdateText();
            }
            else if (e.PropertyName == Editor.TextColorProperty.PropertyName)
            {
                UpdateTextColor();
            }
            else if (e.PropertyName == Editor.FontSizeProperty.PropertyName ||
                e.PropertyName == Editor.FontAttributesProperty.PropertyName)
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
            var inputField = Control;
            if (inputField != null)
            {
                inputField.text = Element.Text;
            }
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