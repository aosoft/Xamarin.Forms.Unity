using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	internal class UnityDeviceInfo : DeviceInfo
	{
		Size _pixelScreenSize = new Size();
		Size _scaledScreenSize = new Size();
		double _scalingFactor = 1.0;

		internal UnityDeviceInfo()
		{
			var bounds = new Size();
			_pixelScreenSize = new Size(bounds.Width, bounds.Height);
			_scaledScreenSize = _pixelScreenSize;
		}

		public override Size PixelScreenSize => _pixelScreenSize;

		public override Size ScaledScreenSize => _scaledScreenSize;

		public override double ScalingFactor => _scalingFactor;
	}
}
