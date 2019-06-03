Xamarin.Forms for Unity
=====

Copyright (c) 2017 Yasuhiro Taniuchi  
Copyright (c) .NET Foundation Contributors

[LICENSE (MIT)](LICENSE)  
[Demo (WebGL Build / 2017/9/8 Version)](https://aosoft.github.io/XFUnitySample/170908/)

## About

Xamarin.Forms Platform implementation for Unity Game Engine (Unity3D).  

## Requirements

* Unity 2018.4.1f1 (or later)
* [UniRx 5.5.0 (or later)](https://www.assetstore.unity3d.com/#!/content/17276)

## Try the sample

1. Import "Xamarin.Forms" from NuGet.
2. Import "UniRx" from Unity Asset Store or GitHub.
3. Open "Assets/main.unity" or "Assets/XamlPad.unity" scene file.
4. Edit sample source code.
  * main.unity
    * Assets/Scripts/Page1.xaml.cs
    * Assets/Scripts/SampleBindingContext.cs
  * XamlPad.unity
    * Assets/Scripts/XamlPad/XamlPadBindingContext.cs
    * Assets/Scripts/XamlPad/XamlPadPage.cs
    * Assets/StreamingAssets/XamlPadPage.xaml
5. Run.

## Getting Started

1. Create New Unity Project.
2. Changes "Script Runtime Version" at Player Settings to ".NET 4.6".
3. Copy 'Assets/Xamarin.Forms.Unity' to Unity Project.
4. Import "Xamarin.Forms".
    1. Download from [NuGet Package (Ver.4.0.0.425677)](https://www.nuget.org/api/v2/package/Xamarin.Forms/4.0.0.425677) (https://www.nuget.org/packages/Xamarin.Forms) .
    2. Change the extension from nupkg to zip. And unpack.
    3. Copy dlls from lib/netstandard2.0 to Unity Project (ex. Assets/Xamarin.Forms.Unity/Plugins) .
5. Import "UniRx".
6. Create App class that inherits Xamarin.Forms.Application.
7. Create FormsApplicationActivity class that inherits Xamarin.Forms.Platform.Unity.UnityFormsApplicationActivity<T>. T is Application class that implemented earlier.
8. Create Prefabs of UI Comoponent. (Canvas, Button, Text, InputField etc.)
9. Create "UI - Canvas" on the Hierarchy of Unity scene. This Canvas is UI Root of Xamarin.Forms.
10. Create "Xamarin.Forms Application Activity(User defined)" on the Hierarchy of Unity scene. This Activity must be singleton.  
11. Set Prefabs of UI Component and Root Canvas to Activity.

## Known Issues

* XAML Loader does not work with IL2CPP Build. (maybe due to Unity (2017.1.1f1))
* NavigationPage is not implemented in the current version.
* ~~Custom Renderers is not supported in the current version.~~  
  (Please apply the design with the component of Prefab.)
* Standard Renderers are under development.
