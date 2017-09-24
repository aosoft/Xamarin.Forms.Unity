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
				picker.onValueChanged.AsObservable()
					.BlockReenter()
					.Subscribe(value =>
					{
						var elem = Element;
						if (elem != null)
						{
							elem.SelectedIndex = value;
						}
					}).AddTo(this);

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
				UnityComponent.options = CreateOptionDatas(e.NewElement.Items);

				UpdateTextColor();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Picker.SelectedItemProperty.PropertyName)
			{
				UpdateSelectedIndex();
			}
			else if (e.PropertyName == Picker.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		void UpdateSelectedIndex()
		{
			if (UnityComponent != null && Element != null)
			{
				UnityComponent.value = Element.SelectedIndex; 
			}
		}

			void UpdateTextColor()
		{
			_componentText.UpdateTextColor(Element.TextColor);
		}

		static List<UnityEngine.UI.Dropdown.OptionData> CreateOptionDatas(IList<string> source)
		{
			if (source != null)
			{
				int count = source.Count;
				var ret = new List<UnityEngine.UI.Dropdown.OptionData>(count);
				for (int i = 0; i < count; i++)
				{
					ret.Add(new UnityEngine.UI.Dropdown.OptionData(source[i]));
				}
				return ret;
			}
			return null;
		}

		#endregion
	}
}
