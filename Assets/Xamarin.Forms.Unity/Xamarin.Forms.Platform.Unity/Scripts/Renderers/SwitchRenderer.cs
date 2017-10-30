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

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();

			var swtch = Control;
			if (swtch != null)
			{
				swtch.OnValueChangedAsObservable()
					.BlockReenter()
					.Subscribe(value =>
					{
						var elem = Element;
						if (elem != null)
						{
							elem.IsToggled = value;
						}
					}).AddTo(swtch);

				var text = swtch.GetComponentInChildren<UnityEngine.UI.Text>();
				if (text != null)
				{
					UnityEngine.Object.DestroyObject(text);
				}
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateToggle();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Switch.IsToggledProperty.PropertyName)
			{
				UpdateToggle();
			}
			base.OnElementPropertyChanged(sender, e);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateToggle()
		{
			if (Control != null && Element != null)
			{
				Control.isOn = Element.IsToggled;
			}
		}

		#endregion
	}
}
