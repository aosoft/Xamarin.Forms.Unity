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
		#region Private Field

		VisualElementTracker<TElement, TNativeElement> _tracker;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Start()
		{
			Component = GetComponent<TNativeElement>();
		}

		private void OnDestroy()
		{
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property

		VisualElementPackager Packager { get; set; }

		public TNativeElement Component { get; private set; }

		public TElement Element { get; private set; }

		protected bool AutoPackage { get; set; } = true;

		protected bool AutoTrack { get; set; } = true;

		#endregion

		/*-----------------------------------------------------------------*/
		#region IVisualElementRenderer

		VisualElement IVisualElementRenderer.Element => Element;

		UnityEngine.Component IVisualElementRenderer.Component => this.Component;

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			throw new NotImplementedException();
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
					Tracker = new VisualElementTracker<TElement, TNativeElement>();
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

		protected VisualElementTracker<TElement, TNativeElement> Tracker
		{
			get { return _tracker; }
			set
			{
				if (_tracker == value)
					return;

				if (_tracker != null)
				{
					_tracker.Dispose();
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

		void SetNativeControl(TNativeElement control)
		{
			this.Component = control;

			UpdateTracker();


			//Control.HorizontalAlignment = HorizontalAlignment.Stretch;
			//Control.VerticalAlignment = VerticalAlignment.Stretch;

			control.transform.parent = this.transform;

			if (Element == null)
				throw new InvalidOperationException(
					"Cannot assign a native control without an Element; Renderer unbound and/or disposed. " +
					"Please consult Xamarin.Forms renderers for reference implementation of OnElementChanged.");

			Element.IsNativeStateConsistent = false;
			//control.Loaded += OnControlLoaded;

			//control.GotFocus += OnControlGotFocus;
			//control.LostFocus += OnControlLostFocus;

			UpdateBackgroundColor();

			//if (Element != null && !string.IsNullOrEmpty(Element.AutomationId))
			//	SetAutomationId(Element.AutomationId);
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
			if (Component != null)
				Component.gameObject.SetActive(Element.IsEnabled);
			/*else
				IsHitTestVisible = Element.IsEnabled && !Element.InputTransparent;*/
		}

		void UpdateTracker()
		{
			if (_tracker == null)
				return;

			//_tracker.PreventGestureBubbling = PreventGestureBubbling;
			_tracker.Component = Component;
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
