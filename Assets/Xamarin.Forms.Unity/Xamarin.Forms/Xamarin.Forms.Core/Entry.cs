using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_EntryRenderer))]
	public class Entry : InputView, IFontElement, ITextElement, IEntryController, IElementConfiguration<Entry>
	{
		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create("Placeholder", typeof(string), typeof(Entry), default(string));

		public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create("IsPassword", typeof(bool), typeof(Entry), default(bool));

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(Entry), null, BindingMode.TwoWay, propertyChanged: OnTextChanged);

		public static readonly BindableProperty TextColorProperty = TextElement.TextColorProperty;

		public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create("HorizontalTextAlignment", typeof(TextAlignment), typeof(Entry), TextAlignment.Start);

		public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create("PlaceholderColor", typeof(Color), typeof(Entry), Color.Default);

		public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

		public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

		public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

		private readonly Lazy<PlatformConfigurationRegistry<Entry>> _platformConfigurationRegistry;

		public Entry()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Entry>>(() => new PlatformConfigurationRegistry<Entry>(this));
		}

		public TextAlignment HorizontalTextAlignment
		{
			get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
			set { SetValue(HorizontalTextAlignmentProperty, value); }
		}

		public bool IsPassword
		{
			get { return (bool)GetValue(IsPasswordProperty); }
			set { SetValue(IsPasswordProperty, value); }
		}

		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}

		public Color PlaceholderColor
		{
			get { return (Color)GetValue(PlaceholderColorProperty); }
			set { SetValue(PlaceholderColorProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextElement.TextColorProperty); }
			set { SetValue(TextElement.TextColorProperty, value); }
		}

		public FontAttributes FontAttributes
		{
			get { return (FontAttributes)GetValue(FontAttributesProperty); }
			set { SetValue(FontAttributesProperty, value); }
		}

		public string FontFamily
		{
			get { return (string)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		double IFontElement.FontSizeDefaultValueCreator() =>
			Device.GetNamedSize(NamedSize.Default, (Entry)this);

		void IFontElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontFamilyChanged(string oldValue, string newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontSizeChanged(double oldValue, double newValue) =>
			InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		void IFontElement.OnFontChanged(Font oldValue, Font newValue) =>
			 InvalidateMeasureInternal(InvalidationTrigger.MeasureChanged);

		public event EventHandler Completed;

		public event EventHandler<TextChangedEventArgs> TextChanged;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendCompleted()
		{
			Completed?.Invoke(this, EventArgs.Empty);
		}

		private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var entry = (Entry)bindable;

			entry.TextChanged?.Invoke(entry, new TextChangedEventArgs((string)oldValue, (string)newValue));
		}

		public IPlatformElementConfiguration<T, Entry> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void ITextElement.OnTextColorPropertyChanged(Color oldValue, Color newValue)
		{
		}
	}
}