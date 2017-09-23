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
	public class PickerRenderer : ViewRenderer<Picker, UnityEngine.UI.Dropdown>
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

			var picker = UnityComponent;
			if (picker != null)
			{
				/*picker.OnClickAsObservable()
					.Subscribe(_ =>
					{
					})
					.AddTo(this);*/
			}

			_componentText = new TextTracker(this.GetComponentInChildren<UnityEngine.UI.Text>());
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateTextColor();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Picker.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateTextColor()
		{
			_componentText.UpdateTextColor(Element.TextColor);
		}

		#endregion
	}
}
