using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
    internal class UnitySystemResourcesProvider : ISystemResourcesProvider
    {
        private ResourceDictionary _dictionary;

        public IResourceDictionary GetSystemResources()
        {
            _dictionary = new ResourceDictionary();

            return _dictionary;
        }
    }
}