using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_TabbedPageRenderer))]
	public class TabbedPage : MultiPage<Page>, IElementConfiguration<TabbedPage>
	{
		public static readonly BindableProperty BarBackgroundColorProperty = BindableProperty.Create(nameof(BarBackgroundColor), typeof(Color), typeof(TabbedPage), Color.Default);

		public static readonly BindableProperty BarTextColorProperty = BindableProperty.Create(nameof(BarTextColor), typeof(Color), typeof(TabbedPage), Color.Default);

		private readonly Lazy<PlatformConfigurationRegistry<TabbedPage>> _platformConfigurationRegistry;

		public Color BarBackgroundColor
		{
			get
			{
				return (Color)GetValue(BarBackgroundColorProperty);
			}
			set
			{
				SetValue(BarBackgroundColorProperty, value);
			}
		}

		public Color BarTextColor
		{
			get
			{
				return (Color)GetValue(BarTextColorProperty);
			}
			set
			{
				SetValue(BarTextColorProperty, value);
			}
		}

		protected override Page CreateDefault(object item)
		{
			var page = new Page();
			if (item != null)
				page.Title = item.ToString();

			return page;
		}

		public TabbedPage()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<TabbedPage>>(() => new PlatformConfigurationRegistry<TabbedPage>(this));
		}

		public new IPlatformElementConfiguration<T, TabbedPage> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}