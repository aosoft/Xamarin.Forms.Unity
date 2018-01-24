using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CarouselPageRenderer))]
	public class CarouselPage : MultiPage<ContentPage>, IElementConfiguration<CarouselPage>
	{
		private readonly Lazy<PlatformConfigurationRegistry<CarouselPage>> _platformConfigurationRegistry;

		public CarouselPage()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<CarouselPage>>(() => new PlatformConfigurationRegistry<CarouselPage>(this));
		}

		public new IPlatformElementConfiguration<T, CarouselPage> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		protected override ContentPage CreateDefault(object item)
		{
			var page = new ContentPage();
			if (item != null)
				page.Title = item.ToString();

			return page;
		}
	}
}