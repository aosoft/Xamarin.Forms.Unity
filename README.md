Xamarin.Forms for Unity
=====

Copyright (c) 2017 Yasuhiro Taniuchi  
Copyright (c) .NET Foundation Contributors

[LICENSE (MIT)](LICENSE)  
[Demo (WebGL Build / 2017/9/8 Version)](https://aosoft.github.io/XFUnitySample/170908/)

## About

Xamarin.Forms Platform implementation for Unity Game Engine (Unity3D).  
This project contains source code of Xamarin.Forms 2.5.0-sr1.

## Requirements

* Unity 2017.1.1f1 (or later)
* [UniRx 5.5.0 (or later)](https://www.assetstore.unity3d.com/#!/content/17276)

## Try the sample

1. Import "UniRx".
2. Open "Assets/main.unity" or "Assets/XamlPad.unity" scene file.
3. Edit sample source code.
  * main.unity
    * Assets/Scripts/Page1.xaml.cs
    * Assets/Scripts/SampleBindingContext.cs
  * XamlPad.unity
    * Assets/Scripts/XamlPad/XamlPadBindingContext.cs
    * Assets/Scripts/XamlPad/XamlPadPage.cs
    * Assets/StreamingAssets/XamlPadPage.xaml
4. Run.

## Getting Started

1. Create New Unity Project.
2. Changes "Script Runtime Version" at Player Settings to ".NET 4.6".
3. Copy 'Assets/Xamarin.Forms.Unity' to Unity Project.
4. Import "UniRx".
5. Create App class that inherits Xamarin.Forms.Application.
6. Create FormsApplicationActivity class that inherits Xamarin.Forms.Platform.Unity.UnityFormsApplicationActivity<T>. T is Application class that implemented earlier.
7. Create Prefabs of UI Comoponent. (Canvas, Button, Text, InputField etc.)
8. Create "UI - Canvas" on the Hierarchy of Unity scene. This Canvas is UI Root of Xamarin.Forms.
9. Create "Xamarin.Forms Application Activity(User defined)" on the Hierarchy of Unity scene. This Activity must be singleton.  
10. Set Prefabs of UI Component and Root Canvas to Activity.

## Known Issues

* XAML Loader does not work with WebGL Build. (maybe due to Unity (2017.1.1f1))
* NavigationPage is not implemented in the current version.
* ~~Custom Renderers is not supported in the current version.~~  
  (Please apply the design with the component of Prefab.)
* Standard Renderers are under development.
