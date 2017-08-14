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
			UpdateProperties();
		}

		public override Size PixelScreenSize => _pixelScreenSize;

		public override Size ScaledScreenSize => _scaledScreenSize;

		public override double ScalingFactor => _scalingFactor;

		void SetPixelScreenSize(Size value)
		{
			if (value != _pixelScreenSize)
			{
				_pixelScreenSize = value;
				OnPropertyChanged(nameof(PixelScreenSize));
			}
		}

		void SetScaledScreenSize(Size value)
		{
			if (value != _scaledScreenSize)
			{
				_scaledScreenSize = value;
				OnPropertyChanged(nameof(ScaledScreenSize));
			}
		}

		void SetScalingFactor(double value)
		{
			if (value != _scalingFactor)
			{
				_scalingFactor = value;
				OnPropertyChanged(nameof(ScalingFactor));
			}
		}

		void UpdateProperties()
		{
			var current = UnityEngine.Screen.currentResolution;
			var dpi = UnityEngine.Screen.dpi;

			SetPixelScreenSize(new Size(current.width, current.height));
			SetScaledScreenSize(new Size(current.width * dpi, current.height * dpi));
			SetScalingFactor(dpi);
		}
	}
}
