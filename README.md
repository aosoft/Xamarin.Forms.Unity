Xamarin.Forms for Unity
=====

Copyright (c) 2017 Yasuhiro Taniuchi  
Copyright (c) .NET Foundation Contributors

[LICENSE (MIT)](LICENSE)  
[Demo (WebGL Build / 2017/9/8 Version)](https://aosoft.github.io/XFUnitySample/170908/)

## About

Xamarin.Forms Platform implementation for Unity Game Engine.  
This project contains source code of Xamarin.Forms 2.4.0-pre3.

## Requirements

* Unity 2017.1.1f1 (or later)
* [UniRx 5.5.0 (or later)](https://www.assetstore.unity3d.com/#!/content/17276)

## Try the sample

1. Import "UniRx".
2. Open "Assets/main.unity" scene file.
3. Edit sample source code.
  * Assets/Scripts/Page1.xaml.cs
  * Assets/Scripts/SampleBindingContext.cs
4. Run.

## 新しいプロジェクトからの始め方

1. 新規に Unity Project を作成する。
2. PlayerSettings の Scripting Runtime Version を ".NET 4.6" に変更する。
3. 'Assets/Xamarin.Forms.Unity' を作成したプロジェクトにコピーする。
4. "UniRx" を import する。
5. Xamarin.Forms.Application を継承した App クラスを作成する。
6. Xamarin.Forms.Platform.Unity.UnityFormsApplicationActivity<T> を継承した FormsApplicationActivity クラスを作成する。 T は先に作成した Application 継承クラスを指定する。
7. UI Comoponent の Prefab を作成する。 (Canvas, Button, Text, InputField etc.)
8. Unity の Scene 上に Xamarin.Forms の UI ルートとなる Canvas (UI - Canvas) を作成する。
9. Unity の Scene 上で "Xamarin.Forms.Activity" を作成する (GameObject メニュー) 。
10. 9.で作成したActivityに UI Comoponent の Prefab と Root Canvas を指定する。
