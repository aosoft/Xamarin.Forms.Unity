using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Xamarin.Forms;

public partial class Page1 : ContentPage
{
	public Page1()
	{
		InitializeComponent();
	}

	void InitializeComponent()
	{
		var path = System.IO.Path.Combine(UnityEngine.Application.streamingAssetsPath, "Page1.xaml");
		var xaml = System.IO.File.ReadAllText(path);
		Xamarin.Forms.Platform.Unity.XamlLoader.LoadXaml(this, xaml);
	}
}

