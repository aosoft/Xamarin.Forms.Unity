using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using UnityEngine;

namespace Xamarin.Forms.Platform.Unity
{
	public class UnityPlatform : IPlatform, IDisposable
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		Canvas _container;

		#endregion

		/*-----------------------------------------------------------------*/
		#region Constructor / Dispose

		internal UnityPlatform(Canvas container)
		{
			_container = container;
		}

		public void Dispose()
		{
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property



		#endregion

		/*-----------------------------------------------------------------*/
		#region Public Method

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			IVisualElementRenderer renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(element.GetType()) ??

		  new DefaultRenderer();
			renderer.SetElement(element);
			return renderer;
		}


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
