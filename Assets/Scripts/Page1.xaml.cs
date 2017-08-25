using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

public partial class Page1 : ContentPage
{
	public Page1()
	{
		InitializeComponent();
	}

	void InitializeComponent()
	{
		Xamarin.Forms.Xaml.Extensions.LoadFromXaml(this, typeof(Page1));
	}
}

