using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Platform.Unity
{
	public class VisualElementTracker<TElement, TNativeElement> : IDisposable
		where TElement : VisualElement
		where TNativeElement : UnityEngine.Component
	{
		TNativeElement _component;
		TElement _element;

		bool _invalidateArrangeNeeded;

		public VisualElementTracker()
		{
		}

		#region IDisposable Support

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion



		public TNativeElement Component
		{
			get { return _component; }
			set
			{
				if (_component == value)
					return;

				if (_component != null)
				{
					/*
					_control.Tapped -= HandleTapped;
					_control.DoubleTapped -= HandleDoubleTapped;
					*/
				}

				_component = value;
				UpdateNativeControl();
			}
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


		protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Element.Batched)
			{
				if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
					e.PropertyName == VisualElement.HeightProperty.PropertyName)
				{
					_invalidateArrangeNeeded = true;
				}
				return;
			}

			if (e.PropertyName == VisualElement.XProperty.PropertyName || e.PropertyName == VisualElement.YProperty.PropertyName || e.PropertyName == VisualElement.WidthProperty.PropertyName ||
				e.PropertyName == VisualElement.HeightProperty.PropertyName)
			{
				MaybeInvalidate();
			}
			else if (e.PropertyName == VisualElement.AnchorXProperty.PropertyName || e.PropertyName == VisualElement.AnchorYProperty.PropertyName)
			{
				UpdateScaleAndRotation(Element, Component);
			}
			else if (e.PropertyName == VisualElement.ScaleProperty.PropertyName)
			{
				UpdateScaleAndRotation(Element, Component);
			}
			else if (e.PropertyName == VisualElement.TranslationXProperty.PropertyName || e.PropertyName == VisualElement.TranslationYProperty.PropertyName ||
					 e.PropertyName == VisualElement.RotationProperty.PropertyName || e.PropertyName == VisualElement.RotationXProperty.PropertyName || e.PropertyName == VisualElement.RotationYProperty.PropertyName)
			{
				UpdateRotation(Element, Component);
			}
			else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
			{
				UpdateVisibility(Element, Component);
			}
			else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
			{
				UpdateOpacity(Element, Component);
			}
			else if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
			{
				UpdateInputTransparent(Element, Component);
			}
		}

		protected virtual void UpdateNativeControl()
		{
			if (Element == null || Component == null)
				return;

			UpdateVisibility(Element, Component);
			UpdateOpacity(Element, Component);
			UpdateScaleAndRotation(Element, Component);
			UpdateInputTransparent(Element, Component);

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

		static void UpdateInputTransparent(VisualElement view, UnityEngine.Component component)
		{
			component.gameObject.SetActive(view.IsEnabled && view.IsVisible && !view.InputTransparent);
		}

		static void UpdateOpacity(VisualElement view, UnityEngine.Component component)
		{
			//control.Opacity = view.Opacity;
		}

		static void UpdateRotation(VisualElement view, UnityEngine.Component component)
		{
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

		static void UpdateScaleAndRotation(VisualElement view, UnityEngine.Component component)
		{
			double anchorX = view.AnchorX;
			double anchorY = view.AnchorY;
			double scale = view.Scale;
			//control.RenderTransformOrigin = new Windows.Foundation.Point(anchorX, anchorY);
			//control.RenderTransform = new ScaleTransform { ScaleX = scale, ScaleY = scale };

			UpdateRotation(view, component);
		}

		static void UpdateVisibility(VisualElement view, UnityEngine.Component component)
		{
			UpdateInputTransparent(view, component);
		}

	}
}
