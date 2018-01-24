using System;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	[ProvideCompiled("Xamarin.Forms.Core.XamlC.PassthroughValueProvider")]
	[AcceptEmptyServiceProvider]
	public sealed class PropertyCondition : Condition, IValueProvider
	{
		private readonly BindableProperty _stateProperty;

		private BindableProperty _property;
		private object _triggerValue;

		public PropertyCondition()
		{
			_stateProperty = BindableProperty.CreateAttached("State", typeof(bool), typeof(PropertyCondition), false, propertyChanged: OnStatePropertyChanged);
		}

		public BindableProperty Property
		{
			get { return _property; }
			set
			{
				if (_property == value)
					return;
				if (IsSealed)
					throw new InvalidOperationException("Can not change Property once the Trigger has been applied.");
				_property = value;

				//convert the value
				if (_property != null && s_valueConverter != null)
				{
					Func<MemberInfo> minforetriever = () => Property.DeclaringType.GetRuntimeProperty(Property.PropertyName);
					Value = s_valueConverter.Convert(Value, Property.ReturnType, minforetriever, null);
				}
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
					throw new InvalidOperationException("Can not change Value once the Trigger has been applied.");

				//convert the value
				if (_property != null && s_valueConverter != null)
				{
					Func<MemberInfo> minforetriever = () => Property.DeclaringType.GetRuntimeProperty(Property.PropertyName);
					value = s_valueConverter.Convert(value, Property.ReturnType, minforetriever, null);
				}
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
			return (bool)bindable.GetValue(_stateProperty);
		}

		private static IValueConverterProvider s_valueConverter = DependencyService.Get<IValueConverterProvider>();

		internal override void SetUp(BindableObject bindable)
		{
			object newvalue = bindable.GetValue(Property);
			bool newState = (newvalue == Value) || (newvalue != null && newvalue.Equals(Value));
			bindable.SetValue(_stateProperty, newState);
			bindable.PropertyChanged += OnAttachedObjectPropertyChanged;
		}

		internal override void TearDown(BindableObject bindable)
		{
			bindable.ClearValue(_stateProperty);
			bindable.PropertyChanged -= OnAttachedObjectPropertyChanged;
		}

		private void OnAttachedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var bindable = (BindableObject)sender;
			var oldState = (bool)bindable.GetValue(_stateProperty);

			if (Property == null)
				return;
			if (e.PropertyName != Property.PropertyName)
				return;
			object newvalue = bindable.GetValue(Property);
			bool newstate = (newvalue == Value) || (newvalue != null && newvalue.Equals(Value));
			if (oldState != newstate)
				bindable.SetValue(_stateProperty, newstate);
		}

		private void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if ((bool)oldValue == (bool)newValue)
				return;

			if (ConditionChanged != null)
				ConditionChanged(bindable, (bool)oldValue, (bool)newValue);
		}
	}
}