using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Xamarin.Forms.Platform.Unity
{
	public class EntryRenderer : ViewRenderer<Entry, UnityEngine.UI.InputField>
	{
		/*-----------------------------------------------------------------*/
		#region Field


		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior


		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{

			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			base.OnElementPropertyChanged(sender, e);
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Internals

		#endregion
	}
}
