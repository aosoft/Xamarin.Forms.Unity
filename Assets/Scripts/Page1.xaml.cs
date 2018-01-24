using UnityEngine;
using Xamarin.Forms;

public partial class Page1 : ContentPage
{
    public Page1()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
#if false
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
      <Button Grid.Column=""1"" Text=""Button"" Command=""{Binding InstantiateCommand}"" />
    </Grid>
  </Grid>
</ContentPage>
");
#else
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition { Height = 32 });
        grid.RowDefinitions.Add(new RowDefinition { Height = 32 });
        grid.RowDefinitions.Add(new RowDefinition { Height = 32 });

        var sv = new ScrollView();
        grid.Children.Add(sv, 0, 0);
        var gridsv = new Grid();
        for (int i = 0; i < 100; i++)
        {
            gridsv.RowDefinitions.Add(new RowDefinition { Height = 24 });
            gridsv.Children.Add(new Label { Text = string.Format("Label {0}", i), FontSize = 20 }, 0, i);
        }
        sv.Content = gridsv;

        {
            var grid2 = new Grid();
            grid2.ColumnDefinitions.Add(new ColumnDefinition());
            grid2.ColumnDefinitions.Add(new ColumnDefinition());

            grid.Children.Add(grid2, 0, 1);

            var label = new Label
            {
                HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                FontSize = 20,
                TextColor = Xamarin.Forms.Color.White,
                Text = "Default"
            };

            var entry = new Entry();

            label.SetBinding(Label.TextProperty, new Binding { Path = "Text", Source = entry });

            grid2.Children.Add(label, 0, 0);
            grid2.Children.Add(entry, 1, 0);
        }

        {
            var grid2 = new Grid();
            grid2.ColumnDefinitions.Add(new ColumnDefinition());
            grid2.ColumnDefinitions.Add(new ColumnDefinition());

            grid.Children.Add(grid2, 0, 2);

            var label = new Label
            {
                HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                FontSize = 20,
                TextColor = Xamarin.Forms.Color.White
            };
            label.SetBinding(Label.TextProperty, new Binding { Path = "Counter.Value" });
            var button = new Button
            {
                Text = "Button"
            };
            button.SetBinding(Button.CommandProperty, new Binding { Path = "InstantiateCommand" });
            button.Clicked += (s, e) =>
            {
                label.Scale = 5.0;
                label.ScaleTo(1.0);
            };

            grid2.Children.Add(label, 0, 0);
            grid2.Children.Add(button, 1, 0);
        }

        {
            var slider = new Slider();
            grid.Children.Add(slider, 0, 3);

            slider.Minimum = 0.0;
            slider.Maximum = 1.0;
            slider.Value = 1.0;

            slider.SetBinding(Page.OpacityProperty, new Binding { Path = "Value", Source = slider });
        }

        this.Content = grid;
#endif
    }
}