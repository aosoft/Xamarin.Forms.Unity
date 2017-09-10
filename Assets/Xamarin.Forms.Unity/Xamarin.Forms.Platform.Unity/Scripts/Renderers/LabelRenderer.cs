using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class LabelRenderer : ViewRenderer<Label, UnityEngine.UI.Text>
	{
		/*-----------------------------------------------------------------*/
		#region Field

		UnityEngine.Color _defaultTextColor;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();

			var label = UnityComponent;
			if (label != null)
			{
				//	Prefab の設定値がデフォルトカラー
				_defaultTextColor = label.color;
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				//_isInitiallyDefault = Element.IsDefault();

				UpdateText();
				UpdateColor();
				UpdateAlign();
				UpdateFont();
				UpdateLineBreakMode();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Label.TextProperty.PropertyName ||
				e.PropertyName == Label.FormattedTextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Label.TextColorProperty.PropertyName)
			{
				UpdateColor();
			}
			else if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
				e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
			{
				UpdateAlign();
			}
			else if (e.PropertyName == Label.FontSizeProperty.PropertyName ||
				e.PropertyName == Label.FontAttributesProperty.PropertyName)
			{
				UpdateFont();
			}
			else if (e.PropertyName == Label.LineBreakModeProperty.PropertyName)
			{
				UpdateLineBreakMode();
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
			if (pair.IsAvailable)
			{
				pair.UnityComponent.text = pair.Element.Text;
			}
		}

		void UpdateColor()
		{
			var pair = Pair;
			if (pair.IsAvailable)
			{
				if (pair.Element.TextColor != Color.Default)
				{
					pair.UnityComponent.color = pair.Element.TextColor.ToUnityColor();
				}
				else
				{
					pair.UnityComponent.color = _defaultTextColor;
				}
			}
		}

		void UpdateFont()
		{
			var pair = Pair;
			if (pair.IsAvailable)
			{
				pair.UnityComponent.fontSize = (int)pair.Element.FontSize;
				pair.UnityComponent.fontStyle = pair.Element.FontAttributes.ToUnityFontStyle();
			}
		}

		void UpdateLineBreakMode()
		{
			var pair = Pair;
			if (pair.IsAvailable)
			{
				pair.UnityComponent.horizontalOverflow = pair.Element.LineBreakMode.ToUnityHorizontalWrapMode();
			}
		}

		void UpdateAlign()
		{
			var pair = Pair;
			if (pair.IsAvailable)
			{
				pair.UnityComponent.alignment =
					Platform.ToUnityTextAnchor(pair.Element.HorizontalTextAlignment, pair.Element.VerticalTextAlignment);
			}
		}

		#endregion
	}
}
