using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.ComponentModel;
using UnityEngine.EventSystems;
using System.Collections.Specialized;

namespace Xamarin.Forms.Platform.Unity
{
	public class PickerRenderer : ViewRenderer<Picker, UnityEngine.UI.Dropdown>, IPointerClickHandler
	{
		/*-----------------------------------------------------------------*/
		#region Field

		bool _requiredUpdateOptions;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();

			_requiredUpdateOptions = false;

			var picker = Control;
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
					}).AddTo(picker);
				picker.template.anchorMin = new Vector2(0.0f, 1.0f);
				picker.template.anchorMax = new Vector2(1.0f, 1.0f);
				picker.template.anchoredPosition = new Vector2();
				picker.template.pivot = new Vector2(0.5f, 0.0f);
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				e.OldElement.Items.RemoveCollectionChangedEvent(OnCollectionChanged);
			}
			if (e.NewElement != null)
			{
				e.NewElement.Items.AddCollectionChangedEvent(OnCollectionChanged);

				Control.options = CreateOptionDatas(e.NewElement.Items);

				UpdateSelectedIndex();
				UpdatePicker();
				UpdateTextColor();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
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
			if (Control != null && Element != null)
			{
				Control.value = Element.SelectedIndex; 
			}
		}

		void UpdateTextColor()
		{
			var text = Control?.captionText;
			if (text != null && Element != null)
			{
				text.color = Element.TextColor.ToUnityColor();
			}
		}

		void UpdatePicker()
		{
			_requiredUpdateOptions = true;

			var element = Element;
			if (element != null)
			{
				var text = Control?.captionText;
				if (text != null)
				{
					var index = element.SelectedIndex;
					if (index < 0 || index >= element.Items.Count)
					{
						text.text = string.Empty;
					}
					else
					{
						text.text = element.Items[index];
					}
				}
			}
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdatePicker();
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

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			var picker = Control;
			var element = Element;
			if (_requiredUpdateOptions && element != null && picker != null)
			{
				var index = picker.value;
				picker.options = CreateOptionDatas(Element.Items);
				element.SelectedIndex = index;
				picker.value = index;
			}
		}

		#endregion
	}
}
