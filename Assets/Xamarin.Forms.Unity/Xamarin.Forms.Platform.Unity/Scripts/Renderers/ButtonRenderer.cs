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
	public class ButtonRenderer : ViewRenderer<Button, UnityEngine.UI.Button>
	{
		/*-----------------------------------------------------------------*/
		#region Field

		UnityEngine.UI.Text _componentText;
		UnityEngine.Color _defaultTextColor;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();

			var button = UnityComponent;
			if (button != null)
			{
				button.OnClickAsObservable()
					.Subscribe(_ => (Element as IButtonController)?.SendClicked())
					.AddTo(this);
			}

			_componentText = this.GetComponentInChildren<UnityEngine.UI.Text>();
			if (_componentText != null)
			{
				//	Prefab の設定値がデフォルトカラー
				_defaultTextColor = _componentText.color;
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
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
			if (e.PropertyName == Button.TextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Button.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == Button.FontSizeProperty.PropertyName ||
				e.PropertyName == Button.FontAttributesProperty.PropertyName)
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
			//_perfectSizeValid = false;
			var pair = Pair;
			if (pair.IsAvailable && _componentText != null)
			{
				_componentText.text = pair.Element.Text;
			}
		}

		void UpdateTextColor()
		{
			var pair = Pair;
			if (pair.IsAvailable && _componentText != null)
			{
				if (pair.Element.TextColor != Color.Default)
				{
					_componentText.color = pair.Element.TextColor.ToUnityColor();
				}
				else
				{
					_componentText.color = _defaultTextColor;
				}
			}
		}

		void UpdateFont()
		{
			var pair = Pair;
			if (pair.IsAvailable && _componentText != null)
			{
				_componentText.fontSize = (int)pair.Element.FontSize;
				_componentText.fontStyle = pair.Element.FontAttributes.ToUnityFontStyle();
			}
		}

		#endregion
	}
}
