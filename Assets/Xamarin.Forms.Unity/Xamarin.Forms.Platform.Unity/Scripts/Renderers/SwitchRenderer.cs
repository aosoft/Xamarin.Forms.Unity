using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.Unity
{
	public class SwitchRenderer : ViewRenderer<Switch, UnityEngine.UI.Toggle>
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

			var Switch = UnityComponent;
			if (Switch != null)
			{
				Switch.OnClickAsObservable()
					.Subscribe(_ => (Element as ISwitchController)?.SendClicked())
					.AddTo(this);
			}

			_componentText = new TextTracker(this.GetComponentInChildren<UnityEngine.UI.Text>());
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
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
			/*
			if (e.PropertyName == Switch.TextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Switch.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == Switch.FontSizeProperty.PropertyName ||
				e.PropertyName == Switch.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}
			*/
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
