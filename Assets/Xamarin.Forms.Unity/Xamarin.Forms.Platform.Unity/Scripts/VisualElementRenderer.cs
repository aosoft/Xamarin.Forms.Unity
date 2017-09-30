using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	public class VisualElementRenderer<TElement, TNativeElement> :
		MonoBehaviour, IVisualElementRenderer, IEffectControlProvider
		where TElement : VisualElement
		where TNativeElement : UnityEngine.Component
	{
		/*-----------------------------------------------------------------*/
		#region Field

		VisualElementTracker<TElement, TNativeElement> _tracker;

		protected RectTransform _rectTransform;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected virtual void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			if (_rectTransform == null)
			{
				_rectTransform = this.gameObject.AddComponent<RectTransform>();
			}
			UnityComponent = GetComponent<TNativeElement>();
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property

		VisualElementPackager Packager { get; set; }

		public TNativeElement UnityComponent { get; private set; }

		public TElement Element { get; private set; }

		#endregion

		/*-----------------------------------------------------------------*/
		#region IVisualElementRenderer

		VisualElement IVisualElementRenderer.Element => Element;

		UnityEngine.Component IVisualElementRenderer.UnityComponent => this.UnityComponent;

		public virtual Transform UnityContainerTransform => UnityComponent?.transform;

		public RectTransform UnityRectTransform => _rectTransform;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			//	暫定
			if (_rectTransform != null)
			{
				return new SizeRequest(
					new Size(
						Math.Min(_rectTransform.rect.width, widthConstraint),
						Math.Min(_rectTransform.rect.height, heightConstraint)));
			}
			return new SizeRequest(new Size(widthConstraint, heightConstraint));
		}

		public void SetElement(VisualElement element)
		{
			TElement oldElement = Element;
			Element = (TElement)element;

			if (oldElement != null)
			{
				oldElement.PropertyChanged -= OnElementPropertyChanged;
				oldElement.FocusChangeRequested -= OnElementFocusChangeRequested;
			}

			if (element != null)
			{
				Element.PropertyChanged += OnElementPropertyChanged;
				Element.FocusChangeRequested += OnElementFocusChangeRequested;

				if (AutoPackage && Packager == null)
					Packager = new VisualElementPackager(this);

				if (AutoTrack && Tracker == null)
				{
					Tracker = new VisualElementTracker<TElement, TNativeElement>(UnityComponent);
				}

				// Disabled until reason for crashes with unhandled exceptions is discovered
				// Without this some layouts may end up with improper sizes, however their children
				// will position correctly
				//Loaded += (sender, args) => {
				if (Packager != null)
					Packager.Load();
				//};
			}

			OnElementChanged(new ElementChangedEventArgs<TElement>(oldElement, Element));

			var controller = (IElementController)oldElement;
			if (controller != null && controller.EffectControlProvider == (IEffectControlProvider)this)
			{
				controller.EffectControlProvider = null;
			}

			controller = element;
			if (controller != null)
				controller.EffectControlProvider = this;
		}

		public Vector2 GetAnchorPoint()
		{
			var element = Element;
			if (element == null)
			{
				return new Vector2();
			}

			var position = new Vector2((float)element.X, (float)element.Y);
			var size = new Vector2(Mathf.Max((float)element.Width, 0.0f), Mathf.Max((float)element.Height, 0.0f));
			var pivot = (_rectTransform?.pivot).GetValueOrDefault();

			return new Vector2(position.x + size.x * pivot.x, position.y + size.y * pivot.y);
		}

		public virtual Vector2 GetChildAnchorPoint(IVisualElementRenderer child)
		{
			if (child == null)
			{
				return new Vector2();
			}

			var ap = child.GetAnchorPoint();
			var parentElement = this.Element;
			if (parentElement == null)
			{
				return ap;
			}

			var parentHeight = Mathf.Max((float)parentElement.Height, 0.0f);
			return new Vector2(ap.x, parentHeight - ap.y);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region IEffectControlProvider

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			throw new NotImplementedException();
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		protected bool AutoPackage { get; set; } = true;

		protected bool AutoTrack { get; set; } = true;

		protected VisualElementTracker<TElement, TNativeElement> Tracker
		{
			get { return _tracker; }
			set
			{
				if (_tracker == value)
					return;

				if (_tracker != null)
				{
					_tracker.Updated -= OnTrackerUpdated;
				}

				_tracker = value;

				if (_tracker != null)
				{
					_tracker.Updated += OnTrackerUpdated;
					UpdateTracker();
				}
			}
		}

		protected virtual void UpdateBackgroundColor()
		{
			/*
			Color backgroundColor = Element.BackgroundColor;
			var control = Control as Control;
			if (control != null)
			{
				if (!backgroundColor.IsDefault)
				{
					control.BackColor = backgroundColor.ToWindowsColor();
				}
				else
				{
					control.BackColor = System.Drawing.SystemColors.Window;
				}
			}
			else
			{
				if (!backgroundColor.IsDefault)
				{
					BackColor = backgroundColor.ToWindowsColor();
				}
				else
				{
					BackColor = System.Drawing.SystemColors.Window;
				}
			}
			*/
		}

		protected virtual void UpdateNativeControl()
		{
			UpdateEnabled();
			/*
			SetAutomationPropertiesHelpText();
			SetAutomationPropertiesName();
			SetAutomationPropertiesAccessibilityView();
			SetAutomationPropertiesLabeledBy();
			*/
		}


		void UpdateEnabled()
		{
			if (UnityComponent != null)
				UnityComponent.gameObject.SetActive(Element.IsEnabled);
			/*else
				IsHitTestVisible = Element.IsEnabled && !Element.InputTransparent;*/
		}

		void UpdateTracker()
		{
			if (_tracker == null)
				return;

			//_tracker.PreventGestureBubbling = PreventGestureBubbling;
			_tracker.Element = Element;
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected virtual void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			var args = new VisualElementChangedEventArgs(e.OldElement, e.NewElement);
			ElementChanged?.Invoke(this, args);
		}

		protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
				UpdateEnabled();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
				UpdateBackgroundColor();
			/*
			else if (e.PropertyName == AutomationProperties.HelpTextProperty.PropertyName)
				SetAutomationPropertiesHelpText();
			else if (e.PropertyName == AutomationProperties.NameProperty.PropertyName)
				SetAutomationPropertiesName();
			else if (e.PropertyName == AutomationProperties.IsInAccessibleTreeProperty.PropertyName)
				SetAutomationPropertiesAccessibilityView();
			else if (e.PropertyName == AutomationProperties.LabeledByProperty.PropertyName)
				SetAutomationPropertiesLabeledBy();
			*/
		}

		void OnControlGotFocus(object sender, EventArgs args)
		{
			((IVisualElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
		}

		void OnControlLoaded(object sender, EventArgs args)
		{
			Element.IsNativeStateConsistent = true;
		}

		void OnControlLostFocus(object sender, EventArgs args)
		{
			((IVisualElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
		}

		internal virtual void OnElementFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
		{
			/*
			var control = Control as Control;
			if (control == null)
				return;

			if (args.Focus)
				args.Result = control.Focus(FocusState.Programmatic);
			else
			{
				UnfocusControl(control);
				args.Result = true;
			}
			*/
		}

		void OnTrackerUpdated(object sender, EventArgs e)
		{
			UpdateNativeControl();
		}

		#endregion
	}
}
