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
		Xamarin.Forms.Platform.Unity.XamlLoader.LoadXaml(
			this,
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ContentPage
  xmlns=""http://xamarin.com/schemas/2014/forms""
  xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml""
  x:Class=""Page1"">
  <Grid Padding=""16"">
    <Grid.RowDefinitions>
      <RowDefinition />
      <RowDefinition Height=""32""/>
    </Grid.RowDefinitions>

    <Grid Grid.Row=""1"">
      <Grid.ColumnDefinitions>
        <ColumnDefinition />
        <ColumnDefinition />
      </Grid.ColumnDefinitions>
      <Label Grid.Column=""0"" Text=""{Binding Counter.Value}"" HorizontalTextAlignment=""Center"" FontSize=""20""/>
      <Button Grid.Column=""1"" Command=""{Binding InstantiateCommand}"" />
    </Grid>
  </Grid>
</ContentPage>
");
	}
}

