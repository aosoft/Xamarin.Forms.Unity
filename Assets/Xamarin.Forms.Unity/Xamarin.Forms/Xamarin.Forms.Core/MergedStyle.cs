using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms
{
	public partial class VisualElement
	{
		private sealed class MergedStyle : IStyle
		{
			////If the base type is one of these, stop registering dynamic resources further
			////The last one (typeof(Element)) is a safety guard as we might be creating VisualElement directly in internal code
			private static readonly IList<Type> s_stopAtTypes = new List<Type> { typeof(View), typeof(Layout<>), typeof(VisualElement), typeof(Element) };

			private IList<BindableProperty> _classStyleProperties;

			private readonly List<BindableProperty> _implicitStyles = new List<BindableProperty>();

			private IList<Style> _classStyles;

			private IStyle _implicitStyle;

			private IStyle _style;

			private IList<string> _styleClass;

			public MergedStyle(Type targetType, BindableObject target)
			{
				Target = target;
				TargetType = targetType;
				RegisterImplicitStyles();
				Apply(Target);
			}

			public IStyle Style
			{
				get { return _style; }
				set { SetStyle(ImplicitStyle, ClassStyles, value); }
			}

			public IList<string> StyleClass
			{
				get { return _styleClass; }
				set
				{
					if (_styleClass == value)
						return;

					if (_styleClass != null && _classStyles != null)
						foreach (var classStyleProperty in _classStyleProperties)
							Target.RemoveDynamicResource(classStyleProperty);

					_styleClass = value;

					if (_styleClass != null)
					{
						_classStyleProperties = new List<BindableProperty>();
						foreach (var styleClass in _styleClass)
						{
							var classStyleProperty = BindableProperty.Create("ClassStyle", typeof(IList<Style>), typeof(VisualElement), default(IList<Style>),
								propertyChanged: (bindable, oldvalue, newvalue) => ((VisualElement)bindable)._mergedStyle.OnClassStyleChanged());
							_classStyleProperties.Add(classStyleProperty);
							Target.OnSetDynamicResource(classStyleProperty, Forms.Style.StyleClassPrefix + styleClass);
						}
					}
				}
			}

			public BindableObject Target { get; }

			private IList<Style> ClassStyles
			{
				get { return _classStyles; }
				set { SetStyle(ImplicitStyle, value, Style); }
			}

			private IStyle ImplicitStyle
			{
				get { return _implicitStyle; }
				set { SetStyle(value, ClassStyles, Style); }
			}

			public void Apply(BindableObject bindable)
			{
				ImplicitStyle?.Apply(bindable);
				if (ClassStyles != null)
					foreach (var classStyle in ClassStyles)
						((IStyle)classStyle)?.Apply(bindable);
				Style?.Apply(bindable);
			}

			public Type TargetType { get; }

			public void UnApply(BindableObject bindable)
			{
				Style?.UnApply(bindable);
				if (ClassStyles != null)
					foreach (var classStyle in ClassStyles)
						((IStyle)classStyle)?.UnApply(bindable);
				ImplicitStyle?.UnApply(bindable);
			}

			private void OnClassStyleChanged()
			{
				ClassStyles = _classStyleProperties.Select(p => (Target.GetValue(p) as IList<Style>)?.FirstOrDefault(s => s.CanBeAppliedTo(TargetType))).ToList();
			}

			private void OnImplicitStyleChanged()
			{
				var first = true;
				foreach (BindableProperty implicitStyleProperty in _implicitStyles)
				{
					var implicitStyle = (Style)Target.GetValue(implicitStyleProperty);
					if (implicitStyle != null)
					{
						if (first || implicitStyle.ApplyToDerivedTypes)
						{
							ImplicitStyle = implicitStyle;
							return;
						}
					}
					first = false;
				}
			}

			private void RegisterImplicitStyles()
			{
				Type type = TargetType;
				while (true)
				{
					BindableProperty implicitStyleProperty = BindableProperty.Create("ImplicitStyle", typeof(Style), typeof(VisualElement), default(Style),
						 propertyChanged: (bindable, oldvalue, newvalue) => OnImplicitStyleChanged());
					_implicitStyles.Add(implicitStyleProperty);
					Target.SetDynamicResource(implicitStyleProperty, type.FullName);
					type = type.GetTypeInfo().BaseType;
					if (s_stopAtTypes.Contains(type))
						return;
				}
			}

			private void SetStyle(IStyle implicitStyle, IList<Style> classStyles, IStyle style)
			{
				bool shouldReApplyStyle = implicitStyle != ImplicitStyle || classStyles != ClassStyles || Style != style;
				bool shouldReApplyClassStyle = implicitStyle != ImplicitStyle || classStyles != ClassStyles;
				bool shouldReApplyImplicitStyle = implicitStyle != ImplicitStyle && (Style as Style == null || ((Style)Style).CanCascade);

				if (shouldReApplyStyle)
					Style?.UnApply(Target);
				if (shouldReApplyClassStyle && ClassStyles != null)
					foreach (var classStyle in ClassStyles)
						((IStyle)classStyle)?.UnApply(Target);
				if (shouldReApplyImplicitStyle)
					ImplicitStyle?.UnApply(Target);

				_implicitStyle = implicitStyle;
				_classStyles = classStyles;
				_style = style;

				if (shouldReApplyImplicitStyle)
					ImplicitStyle?.Apply(Target);
				if (shouldReApplyClassStyle && ClassStyles != null)
					foreach (var classStyle in ClassStyles)
						((IStyle)classStyle)?.Apply(Target);
				if (shouldReApplyStyle)
					Style?.Apply(Target);
			}
		}
	}
}