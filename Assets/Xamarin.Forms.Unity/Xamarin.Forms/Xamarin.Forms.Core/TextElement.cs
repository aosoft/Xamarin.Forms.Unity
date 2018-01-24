namespace Xamarin.Forms
{
	internal static class TextElement
	{
		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create("TextColor", typeof(Color), typeof(ITextElement), Color.Default,
									propertyChanged: OnTextColorPropertyChanged);

		private static void OnTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((ITextElement)bindable).OnTextColorPropertyChanged((Color)oldValue, (Color)newValue);
		}
	}
}