﻿using Xamarin.Forms;

namespace XamlPad
{
    public class XamlPadPage : ContentPage
    {
        public XamlPadPage()
        {
            var path = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, "XamlPadPage.xaml");
            var xaml = System.IO.File.ReadAllText(path);
            Xamarin.Forms.Platform.Unity.XamlLoader.LoadXaml(this, xaml);

            _root = this.FindByName<Grid>("_root");
        }

        private Grid _root;

        public static readonly BindableProperty InnerContentProperty = BindableProperty.Create(
            nameof(InnerContent),
            typeof(View),
            typeof(XamlPadPage),
            null,
            propertyChanged: (b, o, n) =>
            {
                var t = b as XamlPadPage;
                if (t != null && t._root != null)
                {
                    var oldView = o as View;
                    var newView = n as View;
                    if (oldView != null)
                    {
                        t._root.Children.Remove(oldView);
                    }
                    if (newView != null)
                    {
                        t._root.Children.Insert(1, newView);
                        Grid.SetRow(newView, 1);
                    }
                }
            });

        public View InnerContent
        {
            get { return (View)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }
    }
}