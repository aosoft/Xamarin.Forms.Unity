using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
    internal class UnityDeviceInfo : DeviceInfo
    {
        private Size _pixelScreenSize = new Size();
        private Size _scaledScreenSize = new Size();
        private double _scalingFactor = 1.0;

        internal UnityDeviceInfo()
        {
            UpdateProperties();
        }

        public override Size PixelScreenSize => _pixelScreenSize;

        public override Size ScaledScreenSize => _scaledScreenSize;

        public override double ScalingFactor => _scalingFactor;

        private void SetPixelScreenSize(Size value)
        {
            if (value != _pixelScreenSize)
            {
                _pixelScreenSize = value;
                OnPropertyChanged(nameof(PixelScreenSize));
            }
        }

        private void SetScaledScreenSize(Size value)
        {
            if (value != _scaledScreenSize)
            {
                _scaledScreenSize = value;
                OnPropertyChanged(nameof(ScaledScreenSize));
            }
        }

        private void SetScalingFactor(double value)
        {
            if (value != _scalingFactor)
            {
                _scalingFactor = value;
                OnPropertyChanged(nameof(ScalingFactor));
            }
        }

        private void UpdateProperties()
        {
            var current = UnityEngine.Screen.currentResolution;
            var dpi = UnityEngine.Screen.dpi;

            SetPixelScreenSize(new Size(current.width, current.height));
            SetScaledScreenSize(new Size(current.width * dpi, current.height * dpi));
            SetScalingFactor(dpi);
        }
    }
}