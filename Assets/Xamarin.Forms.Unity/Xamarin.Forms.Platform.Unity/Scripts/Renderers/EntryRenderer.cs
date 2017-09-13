using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UniRx;

namespace Xamarin.Forms.Platform.Unity
{
	public class EntryRenderer : ViewRenderer<Entry, UnityEngine.UI.InputField>
	{
		/*-----------------------------------------------------------------*/
		#region Field

		TextTracker _componentText;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();

			var inputField = UnityComponent;
			if (inputField != null)
			{
				inputField.lineType = UnityEngine.UI.InputField.LineType.SingleLine;
				inputField.OnValueChangedAsObservable().Subscribe(value =>
				{
					if (Element != null && Element.Text != value)
					{
						Element.Text = value;
					}
				}).AddTo(this);

				_componentText = new TextTracker(inputField.textComponent);
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
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
			if (e.PropertyName == Entry.TextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == Entry.FontSizeProperty.PropertyName ||
				e.PropertyName == Entry.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateText()
		{
			_componentText.UpdateText(Element.Text);
		}

		void UpdateTextColor()
		{
			_componentText.UpdateTextColor(Element.TextColor);
		}

		void UpdateFont()
		{
			_componentText.UpdateFont(Element);
		}

		#endregion
	}
}
