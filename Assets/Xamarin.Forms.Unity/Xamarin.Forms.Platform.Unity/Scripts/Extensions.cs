using UniRx;

namespace Xamarin.Forms.Platform.Unity
{
	static public class Extensions
	{
		static public UnityEngine.Color ToUnityColor(this Color color)
		{
			return new UnityEngine.Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		static public UnityEngine.FontStyle ToUnityFontStyle(this FontAttributes attrs)
		{
			switch (attrs)
			{
				case FontAttributes.Bold:
					{
						return UnityEngine.FontStyle.Bold;
					}

				case FontAttributes.Italic:
					{
						return UnityEngine.FontStyle.Italic;
					}
			}
			return UnityEngine.FontStyle.Normal;
		}

		static public UnityEngine.HorizontalWrapMode ToUnityHorizontalWrapMode(this LineBreakMode mode)
		{
			return mode == LineBreakMode.CharacterWrap || mode == LineBreakMode.WordWrap ?
					UnityEngine.HorizontalWrapMode.Wrap : UnityEngine.HorizontalWrapMode.Overflow;
		}

		static public IObservable<T> BlockReenter<T>(this IObservable<T> self)
		{
			return Observable.Create<T>(observer =>
			{
				bool entered = false;
				return self.Subscribe(value =>
				{
					if (!entered)
					{
						entered = true;
						try
						{
							observer.OnNext(value);
						}
						finally
						{
							entered = false;
						}
					}
				});
			});
		}
	}
}
