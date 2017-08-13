using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;


namespace Xamarin.Forms.Platform.Unity
{
	public class UnityPlatform : IPlatform, IDisposable
	{
		/*-----------------------------------------------------------------*/
		#region Private Field
		#endregion

		/*-----------------------------------------------------------------*/
		#region Constructor / Dispose

		public void Dispose()
		{
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property



		#endregion

		/*-----------------------------------------------------------------*/
		#region IPlatform

		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}
