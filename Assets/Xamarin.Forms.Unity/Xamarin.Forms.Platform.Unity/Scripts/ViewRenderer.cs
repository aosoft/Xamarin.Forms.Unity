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
			var size = new Vector2(Mathf.Max((float)view.Width, 0.0f), Mathf.Max((float)view.Height, 0.0f));

			var pivot = _rectTransform.pivot;

			_rectTransform.anchorMin = new Vector2();
			_rectTransform.anchorMax = new Vector2();
			_rectTransform.anchoredPosition =
				new Vector2(
					                position.x + size.x * pivot.x,
					parentHeight - (position.y + size.y * pivot.y));
			_rectTransform.sizeDelta = size;

			/*var margin = view.Margin;
			var marginstr = string.Format("(l={0}, t={1}, r={2}, b={3})", margin.Left, margin.Top, margin.Right, margin.Bottom);

			Debug.Log(string.Format("Layout5: {0} ({1}) pt={2} sz={3} pivot={4} ancpt={5} mg={6} (parent {7}, {8})",
				view.GetType(), _rectTransform.GetInstanceID(), position, size, pivot, _rectTransform.anchoredPosition, marginstr,
				parent?.Width, parent?.Height));*/
		}

		#endregion

	}
}