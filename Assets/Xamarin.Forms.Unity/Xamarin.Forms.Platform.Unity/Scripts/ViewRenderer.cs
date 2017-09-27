using System;
using System.ComponentModel;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	public class ViewRenderer<TElement, TNativeElement> :
		VisualElementRenderer<TElement, TNativeElement>
		where TElement : View
		where TNativeElement : UnityEngine.Component
	{
		/*-----------------------------------------------------------------*/
		#region Field



		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == View.HorizontalOptionsProperty.PropertyName ||
				e.PropertyName == View.VerticalOptionsProperty.PropertyName)
			{
				UpdateNativeControl();
			}

			base.OnElementPropertyChanged(sender, e);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateBackgroundColor();
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		protected override void UpdateNativeControl()
		{
			base.UpdateNativeControl();

			UpdateLayout();
		}

		protected virtual void UpdateLayout()
		{
			var view = Element;
			if (_rectTransform == null || view == null)
			{
				return;
			}

			var parent = view.Parent as VisualElement;
			float parentHeight = 0.0f;
			if (parent != null)
			{
				parentHeight = Mathf.Max((float)parent.Height, 0.0f);
			}

			var position = new Vector2((float)view.X, (float)view.Y);
			var size = new Vector2((float)view.Width, (float)view.Height);

			//	サイズ不定時のみ、一応 Unity の Layout System にのる
			var anchorMin = new Vector2(                       0.0f, size.y < 0.0f ? 0.0f : 1.0f);
			var anchorMax = new Vector2(size.x < 0.0f ? 1.0f : 0.0f,                        1.0f);
			size = new Vector2(Mathf.Max(size.x, 0.0f), Mathf.Max(size.y, 0.0f));

			var pivot = _rectTransform.pivot;

			_rectTransform.anchorMin = anchorMin;
			_rectTransform.anchorMax = anchorMax;
			_rectTransform.anchoredPosition =
				new Vector2(
					 position.x + size.x * pivot.x,
					-position.y + (parentHeight - size.y) * pivot.y);
			_rectTransform.sizeDelta = size;
		}

		#endregion

	}
}