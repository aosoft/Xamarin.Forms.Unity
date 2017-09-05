
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
	}
}
