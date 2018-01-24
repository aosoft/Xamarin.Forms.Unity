using System;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	[ProvideCompiled("Xamarin.Forms.Core.XamlC.PassthroughValueProvider")]
	[AcceptEmptyServiceProvider]
	public sealed class BindingCondition : Condition, IValueProvider
	{
		private readonly BindableProperty _boundProperty;

		private BindingBase _binding;
		private object _triggerValue;

		public BindingCondition()
		{
			_boundProperty = BindableProperty.CreateAttached("Bound", typeof(object), typeof(BindingCondition), null, propertyChanged: OnBoundPropertyChanged);
		}

		public BindingBase Binding
		{
			get { return _binding; }
			set
			{
				if (_binding == value)
					return;
				if (IsSealed)
					throw new InvalidOperationException("Can not change Binding once the Condition has been applied.");
				_binding = value;
			}
		}

		public object Value
		{
			get { return _triggerValue; }
			set
			{
				if (_triggerValue == value)
					return;
				if (IsSealed)
					throw new InvalidOperationException("Can not change Value once the Condition has been applied.");
				_triggerValue = value;
			}
		}

		object IValueProvider.ProvideValue(IServiceProvider serviceProvider)
		{
			//This is no longer required
			return this;
		}

		internal override bool GetState(BindableObject bindable)
		{
			object newValue = bindable.GetValue(_boundProperty);
			return EqualsToValue(newValue);
		}

		internal override void SetUp(BindableObject bindable)
		{
			if (Binding != null)
				bindable.SetBinding(_boundProperty, Binding.Clone());
		}

		internal override void TearDown(BindableObject bindable)
		{
			bindable.RemoveBinding(_boundProperty);
			bindable.ClearValue(_boundProperty);
		}

		private static IValueConverterProvider s_valueConverter = DependencyService.Get<IValueConverterProvider>();

		private bool EqualsToValue(object other)
		{
			if ((other == Value) || (other != null && other.Equals(Value)))
				return true;

			object converted = null;
			if (s_valueConverter != null)
				converted = s_valueConverter.Convert(Value, other != null ? other.GetType() : typeof(object), null, null);
			else
				return false;

			return (other == converted) || (other != null && other.Equals(converted));
		}

		private void OnBoundPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			bool oldState = EqualsToValue(oldValue);
			bool newState = EqualsToValue(newValue);

			if (newState == oldState)
				return;

			if (ConditionChanged != null)
				ConditionChanged(bindable, oldState, newState);
		}
	}
}