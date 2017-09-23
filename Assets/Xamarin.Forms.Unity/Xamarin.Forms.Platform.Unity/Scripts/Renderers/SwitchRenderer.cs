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

			var swtch = UnityComponent;
			if (swtch != null)
			{
				swtch.OnValueChangedAsObservable()
					.BlockReenter()
					.Subscribe(_ =>
					{
						var elem = Element;
						if (elem != null)
						{
							elem.IsToggled = swtch.isOn;
						}
					}).AddTo(this);
			}

			var text = this.GetComponentInChildren<UnityEngine.UI.Text>();
			if (text != null)
			{
				DestroyObject(text);
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
			if (UnityComponent != null && Element != null)
			{
				UnityComponent.isOn = Element.IsToggled;
			}
		}

		#endregion
	}
}
