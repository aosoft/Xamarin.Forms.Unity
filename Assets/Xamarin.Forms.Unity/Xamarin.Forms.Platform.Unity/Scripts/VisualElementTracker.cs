using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class VisualElementTracker<TElement, TNativeElement>
		where TElement : VisualElement
		where TNativeElement : UnityEngine.Component
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		TNativeElement _control;
		TElement _element;
		VisualElementBehaviour _behaviour;

		bool _invalidateArrangeNeeded;

		#endregion

		/*-----------------------------------------------------------------*/
		#region Constructor

		public VisualElementTracker(TNativeElement control, VisualElementBehaviour behaviour)
		{
			_control = control;
			_behaviour = behaviour;

			/*
			_control.Tapped -= HandleTapped;
			_control.DoubleTapped -= HandleDoubleTapped;
			*/
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property

		public TNativeElement Control
		{
			get { return _control; }
		}

		public TElement Element
		{
			get { return _element; }
			set
			{
				if (_element == value)
					return;

				if (_element != null)
				{
					_element.BatchCommitted -= OnRedrawNeeded;
					_element.PropertyChanged -= OnPropertyChanged;
				}

				_element = value;

				if (_element != null)
				{
					_element.BatchCommitted += OnRedrawNeeded;
					_element.PropertyChanged += OnPropertyChanged;
				}

				UpdateNativeControl();
			}
		}

		public event EventHandler Updated;

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Element.Batched)
			{
				if (e.PropertyName == VisualElement.XProperty.PropertyName ||
					e.PropertyName == VisualElement.YProperty.PropertyName ||
					e.PropertyName == VisualElement.WidthProperty.PropertyName ||
					e.PropertyName == VisualElement.HeightProperty.PropertyName ||
					e.PropertyName == VisualElement.AnchorXProperty.PropertyName ||
					e.PropertyName == VisualElement.AnchorYProperty.PropertyName)
				{
					_invalidateArrangeNeeded = true;
				}
				return;
			}

			if (e.PropertyName == VisualElement.XProperty.PropertyName ||
				e.PropertyName == VisualElement.YProperty.PropertyName ||
				e.PropertyName == VisualElement.WidthProperty.PropertyName ||
				e.PropertyName == VisualElement.HeightProperty.PropertyName ||
				e.PropertyName == VisualElement.AnchorXProperty.PropertyName ||
				e.PropertyName == VisualElement.AnchorYProperty.PropertyName)
			{
				MaybeInvalidate();
			}
			else if (e.PropertyName == VisualElement.ScaleProperty.PropertyName)
			{
				UpdateScale(Element, _behaviour.RectTransform);
			}
			else if (e.PropertyName == VisualElement.TranslationXProperty.PropertyName ||
					 e.PropertyName == VisualElement.TranslationYProperty.PropertyName ||
					 e.PropertyName == VisualElement.RotationProperty.PropertyName ||
					 e.PropertyName == VisualElement.RotationXProperty.PropertyName ||
					 e.PropertyName == VisualElement.RotationYProperty.PropertyName)
			{
				UpdateRotation(Element, _behaviour.RectTransform);
			}
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
			{
				UpdateVisibility(Element, _behaviour.RectTransform);
			}
			else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
			{
				UpdateOpacity(Element, _behaviour);
			}
			else if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
			{
				UpdateInputTransparent(Element, _behaviour.RectTransform);
			}
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		protected virtual void UpdateNativeControl()
		{
			if (Element == null || _behaviour == null)
				return;

			UpdateVisibility(Element, _behaviour.RectTransform);
			UpdateOpacity(Element, _behaviour);
			UpdatePositionSizeAnchor(Element, _behaviour.RectTransform);
			UpdateScale(Element, _behaviour.RectTransform);
			UpdateRotation(Element, _behaviour.RectTransform);
			UpdateInputTransparent(Element, _behaviour.RectTransform);

			if (_invalidateArrangeNeeded)
			{
				MaybeInvalidate();
			}
			_invalidateArrangeNeeded = false;

			OnUpdated();
		}

		void OnUpdated()
		{
			Updated?.Invoke(this, EventArgs.Empty);
		}

		void OnRedrawNeeded(object sender, EventArgs e)
		{
			UpdateNativeControl();
		}

		void MaybeInvalidate()
		{
			if (Element.IsInNativeLayout)
				return;

			//var parent = (Control)Container.Parent;
			//parent?.InvalidateMeasure();
			//Container.InvalidateMeasure();
		}

		static void UpdateInputTransparent(VisualElement view, RectTransform rectTransform)
		{
			rectTransform.gameObject.SetActive(view.IsEnabled && view.IsVisible && !view.InputTransparent);
		}

		static void UpdateOpacity(VisualElement view, VisualElementBehaviour behaviour)
		{
			behaviour.Opacity = view.Opacity;
		}

		static void UpdatePositionSizeAnchor(VisualElement view, RectTransform rectTransform)
		{
			var position = new Vector2((float)view.X, (float)view.Y);
			var size = new Vector2(Mathf.Max((float)view.Width, 0.0f), Mathf.Max((float)view.Height, 0.0f));
			var pivot = new Vector2((float)view.AnchorX, (float)view.AnchorY);
			var ap = new Vector2(position.x + size.x * pivot.x, -(position.y + size.y * pivot.y));

			/*var parent = view.Parent as VisualElement;
			if (parent != null)
			{
				var parentRenderer = Platform.GetRenderer(parent);
				if (parentRenderer != null)
				{
					ap.y = -ap.y;
				}
			}*/

			rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
			rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
			rectTransform.anchoredPosition = ap;
			rectTransform.sizeDelta = size;
			rectTransform.pivot = pivot;

			//Debug.Log(string.Format("Layout: {0} ({1}) pt={2} sz={3} pivot={4} ancpt={5}",
			//	view.GetType(), rectTransform.GetInstanceID(),
			//	position, size, pivot, ap));
		}

		static void UpdateRotation(VisualElement view, RectTransform rectTransform)
		{
			rectTransform.localEulerAngles = new Vector3((float)view.RotationX, (float)view.RotationY, (float)view.Rotation);
			/*
			double anchorX = view.AnchorX;
			double anchorY = view.AnchorY;
			double rotationX = view.RotationX;
			double rotationY = view.RotationY;
			double rotation = view.Rotation;
			double translationX = view.TranslationX;
			double translationY = view.TranslationY;
			double scale = view.Scale;

			if (rotationX % 360 == 0 && rotationY % 360 == 0 && rotation % 360 == 0 && translationX == 0 && translationY == 0 && scale == 1)
			{
				control.Projection = null;
			}
			else
			{
				control.Projection = new PlaneProjection
				{
					CenterOfRotationX = anchorX,
					CenterOfRotationY = anchorY,
					GlobalOffsetX = scale == 0 ? 0 : translationX / scale,
					GlobalOffsetY = scale == 0 ? 0 : translationY / scale,
					RotationX = -rotationX,
					RotationY = -rotationY,
					RotationZ = -rotation
				};
			}
			*/
		}

		static void UpdateScale(VisualElement view, RectTransform rectTransform)
		{
			float scale = (float)view.Scale;
			rectTransform.localScale = new Vector3(scale, scale, 0.0f);
			rectTransform.localScale = new Vector3(scale, scale, 0.0f);
		}

		static void UpdateVisibility(VisualElement view, RectTransform rectTransform)
		{
			UpdateInputTransparent(view, rectTransform);
		}

		#endregion
	}
}
