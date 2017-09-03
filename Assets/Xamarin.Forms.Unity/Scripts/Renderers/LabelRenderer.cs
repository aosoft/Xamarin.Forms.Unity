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
		public LabelRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				//_isInitiallyDefault = Element.IsDefault();

				UpdateText();
				//UpdateColor(Component);
				UpdateAlign();
				//UpdateFont(Component);
				//UpdateLineBreakMode(Component);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Label.TextProperty.PropertyName ||
				e.PropertyName == Label.FormattedTextProperty.PropertyName)
			{
				UpdateText();
			}
			else if (e.PropertyName == Label.HorizontalTextAlignmentProperty.PropertyName ||
				e.PropertyName == Label.VerticalTextAlignmentProperty.PropertyName)
			{
				UpdateAlign();
			}
			base.OnElementPropertyChanged(sender, e);
		}

		void UpdateText()
		{
			//_perfectSizeValid = false;

			var nativeElement = Component;
			var label = Element;
			if (nativeElement == null || label == null)
			{
				return;
			}

			nativeElement.text = label.Text;
		}

		void UpdateAlign()
		{
			var nativeElement = Component;
			var label = Element;
			if (nativeElement == null || label == null)
			{
				return;
			}

			switch (label.HorizontalTextAlignment)
			{
				case TextAlignment.Start:
					switch (label.VerticalTextAlignment)
					{
						case TextAlignment.Start:
							nativeElement.alignment = TextAnchor.UpperLeft;
							break;

						case TextAlignment.Center:
							nativeElement.alignment = TextAnchor.MiddleLeft;
							break;

						case TextAlignment.End:
							nativeElement.alignment = TextAnchor.LowerLeft;
							break;
					}
					break;

				case TextAlignment.Center:
					switch (label.VerticalTextAlignment)
					{
						case TextAlignment.Start:
							nativeElement.alignment = TextAnchor.UpperCenter;
							break;

						case TextAlignment.Center:
							nativeElement.alignment = TextAnchor.MiddleCenter;
							break;

						case TextAlignment.End:
							nativeElement.alignment = TextAnchor.LowerCenter;
							break;
					}
					break;

				case TextAlignment.End:
					switch (label.VerticalTextAlignment)
					{
						case TextAlignment.Start:
							nativeElement.alignment = TextAnchor.UpperRight;
							break;

						case TextAlignment.Center:
							nativeElement.alignment = TextAnchor.MiddleRight;
							break;

						case TextAlignment.End:
							nativeElement.alignment = TextAnchor.LowerRight;
							break;
					}
					break;

			}
		}
	}
}
