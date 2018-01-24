#define DO_NOT_CHECK_FOR_BINDING_REUSE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Xamarin.Forms.Internals
{
	//FIXME: need a better name for this, and share with Binding, so we can share more unittests
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class TypedBindingBase : BindingBase
	{
		private IValueConverter _converter;
		private object _converterParameter;
		private object _source;
		private string _updateSourceEventName;

		public IValueConverter Converter
		{
			get { return _converter; }
			set
			{
				ThrowIfApplied();
				_converter = value;
			}
		}

		public object ConverterParameter
		{
			get { return _converterParameter; }
			set
			{
				ThrowIfApplied();
				_converterParameter = value;
			}
		}

		public object Source
		{
			get { return _source; }
			set
			{
				ThrowIfApplied();
				_source = value;
			}
		}

		internal string UpdateSourceEventName
		{
			get { return _updateSourceEventName; }
			set
			{
				ThrowIfApplied();
				_updateSourceEventName = value;
			}
		}

		internal TypedBindingBase()
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class TypedBinding<TSource, TProperty> : TypedBindingBase
	{
		private readonly Func<TSource, TProperty> _getter;
		private readonly Action<TSource, TProperty> _setter;
		private readonly PropertyChangedProxy[] _handlers;

		public TypedBinding(Func<TSource, TProperty> getter, Action<TSource, TProperty> setter, Tuple<Func<TSource, object>, string>[] handlers)
		{
			if (getter == null)
				throw new ArgumentNullException(nameof(getter));

			_getter = getter;
			_setter = setter;

			if (handlers == null)
				return;

			_handlers = new PropertyChangedProxy[handlers.Length];
			for (var i = 0; i < handlers.Length; i++)
				_handlers[i] = new PropertyChangedProxy(handlers[i].Item1, handlers[i].Item2, this);
		}

		private readonly WeakReference<object> _weakSource = new WeakReference<object>(null);
		private readonly WeakReference<BindableObject> _weakTarget = new WeakReference<BindableObject>(null);
		private BindableProperty _targetProperty;

		// Applies the binding to a previously set source and target.
		internal override void Apply(bool fromTarget = false)
		{
			base.Apply(fromTarget);

			BindableObject target;
#if DO_NOT_CHECK_FOR_BINDING_REUSE
			if (!_weakTarget.TryGetTarget(out target))
				throw new InvalidOperationException();
#else
			if (!_weakTarget.TryGetTarget(out target) || target == null) {
				Unapply();
				return;
			}
#endif
			object source;
			if (_weakSource.TryGetTarget(out source) && source != null)
				ApplyCore(source, target, _targetProperty, fromTarget);
		}

		// Applies the binding to a new source or target.
		internal override void Apply(object context, BindableObject bindObj, BindableProperty targetProperty)
		{
			_targetProperty = targetProperty;
			var source = Source ?? Context ?? context;

#if (!DO_NOT_CHECK_FOR_BINDING_REUSE)
			base.Apply(source, bindObj, targetProperty);

			BindableObject prevTarget;
			if (_weakTarget.TryGetTarget(out prevTarget) && !ReferenceEquals(prevTarget, bindObj))
				throw new InvalidOperationException("Binding instances can not be reused");

			object previousSource;
			if (_weakSource.TryGetTarget(out previousSource) && !ReferenceEquals(previousSource, source))
				throw new InvalidOperationException("Binding instances can not be reused");
#endif
			_weakSource.SetTarget(source);
			_weakTarget.SetTarget(bindObj);

			ApplyCore(source, bindObj, targetProperty);
		}

		internal override BindingBase Clone()
		{
			Tuple<Func<TSource, object>, string>[] handlers = _handlers == null ? null : new Tuple<Func<TSource, object>, string>[_handlers.Length];
			if (handlers != null)
			{
				for (var i = 0; i < _handlers.Length; i++)
					handlers[i] = new Tuple<Func<TSource, object>, string>(_handlers[i].PartGetter, _handlers[i].PropertyName);
			}
			return new TypedBinding<TSource, TProperty>(_getter, _setter, handlers)
			{
				Mode = Mode,
				Converter = Converter,
				ConverterParameter = ConverterParameter,
				StringFormat = StringFormat,
				Source = Source,
				UpdateSourceEventName = UpdateSourceEventName,
			};
		}

		internal override object GetSourceValue(object value, Type targetPropertyType)
		{
			if (Converter != null)
				value = Converter.Convert(value, targetPropertyType, ConverterParameter, CultureInfo.CurrentUICulture);

			//return base.GetSourceValue(value, targetPropertyType);
			if (StringFormat != null)
				return string.Format(StringFormat, value);

			return value;
		}

		internal override object GetTargetValue(object value, Type sourcePropertyType)
		{
			if (Converter != null)
				value = Converter.ConvertBack(value, sourcePropertyType, ConverterParameter, CultureInfo.CurrentUICulture);

			//return base.GetTargetValue(value, sourcePropertyType);
			return value;
		}

		internal override void Unapply()
		{
#if (!DO_NOT_CHECK_FOR_BINDING_REUSE)
			base.Unapply();
#endif
			if (_handlers != null)
				Unsubscribe();

#if (!DO_NOT_CHECK_FOR_BINDING_REUSE)
			_weakSource.SetTarget(null);
			_weakTarget.SetTarget(null);
#endif
		}

		// ApplyCore is as slim as it should be:
		// Setting  100000 values						: 17ms.
		// ApplyCore  100000 (w/o INPC, w/o unnapply)	: 20ms.
		internal void ApplyCore(object sourceObject, BindableObject target, BindableProperty property, bool fromTarget = false)
		{
			var isTSource = sourceObject != null && sourceObject is TSource;
			var mode = this.GetRealizedMode(property);
			if (mode == BindingMode.OneWay && fromTarget)
				return;

			var needsGetter = (mode == BindingMode.TwoWay && !fromTarget) || mode == BindingMode.OneWay;

			if (isTSource && (mode == BindingMode.OneWay || mode == BindingMode.TwoWay) && _handlers != null)
				Subscribe((TSource)sourceObject);

			if (needsGetter)
			{
				var value = property.DefaultValue;
				if (isTSource)
				{
					try
					{
						value = GetSourceValue(_getter((TSource)sourceObject), property.ReturnType);
					}
					catch (Exception ex) when (ex is NullReferenceException || ex is KeyNotFoundException)
					{
					}
				}
				if (!TryConvert(ref value, property, property.ReturnType, true))
				{
					Log.Warning("Binding", "{0} can not be converted to type '{1}'", value, property.ReturnType);
					return;
				}
				target.SetValueCore(property, value, SetValueFlags.ClearDynamicResource, BindableObject.SetValuePrivateFlags.Default | BindableObject.SetValuePrivateFlags.Converted);
				return;
			}

			var needsSetter = (mode == BindingMode.TwoWay && fromTarget) || mode == BindingMode.OneWayToSource;
			if (needsSetter && _setter != null && isTSource)
			{
				var value = GetTargetValue(target.GetValue(property), typeof(TProperty));
				if (!TryConvert(ref value, property, typeof(TProperty), false))
				{
					Log.Warning("Binding", "{0} can not be converted to type '{1}'", value, typeof(TProperty));
					return;
				}
				_setter((TSource)sourceObject, (TProperty)value);
			}
		}

		private static bool TryConvert(ref object value, BindableProperty targetProperty, Type convertTo, bool toTarget)
		{
			if (value == null)
				return true;
			if ((toTarget && targetProperty.TryConvert(ref value)) || (!toTarget && convertTo.IsInstanceOfType(value)))
				return true;

			object original = value;
			try
			{
				value = Convert.ChangeType(value, convertTo, CultureInfo.InvariantCulture);
				return true;
			}
			catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
			{
				value = original;
				return false;
			}
		}

		private class PropertyChangedProxy
		{
			public Func<TSource, object> PartGetter { get; }
			public string PropertyName { get; }
			public BindingExpression.WeakPropertyChangedProxy Listener { get; }
			private WeakReference<INotifyPropertyChanged> _weakPart = new WeakReference<INotifyPropertyChanged>(null);
			private readonly BindingBase _binding;

			public INotifyPropertyChanged Part
			{
				get
				{
					INotifyPropertyChanged target;
					if (_weakPart.TryGetTarget(out target))
						return target;
					return null;
				}
				set
				{
					_weakPart.SetTarget(value);
					Listener.SubscribeTo(value, OnPropertyChanged);
				}
			}

			public PropertyChangedProxy(Func<TSource, object> partGetter, string propertyName, BindingBase binding)
			{
				PartGetter = partGetter;
				PropertyName = propertyName;
				_binding = binding;
				Listener = new BindingExpression.WeakPropertyChangedProxy();
			}

			private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.PropertyName) && string.CompareOrdinal(e.PropertyName, PropertyName) != 0)
					return;
				Device.BeginInvokeOnMainThread(() => _binding.Apply(false));
			}
		}

		private void Subscribe(TSource sourceObject)
		{
			for (var i = 0; i < _handlers.Length; i++)
			{
				var part = _handlers[i].PartGetter(sourceObject);
				if (part == null)
					break;
				var inpc = part as INotifyPropertyChanged;
				if (inpc == null)
					continue;
				_handlers[i].Part = (inpc);
			}
		}

		private void Unsubscribe()
		{
			for (var i = 0; i < _handlers.Length; i++)
				_handlers[i].Listener.Unsubscribe();
		}
	}
}