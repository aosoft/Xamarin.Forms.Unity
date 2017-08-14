using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	/// <summary>
	/// Xamarin.Forms の Forms 実装。
	/// 通常の実装と異なり、どこかの UI EventSystem に AddComponent しておくことが Forms.Init 相当の処置になる。 
	/// </summary>
	public class Forms : MonoBehaviour
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		Thread _mainThread;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			_mainThread = Thread.CurrentThread;

			Device.PlatformServices = new UnityPlatformServices(this);
			Device.SetIdiom(TargetIdiom.Desktop);
			Device.Info = new UnityDeviceInfo();

			Registrar.RegisterAll(new[]
				{ typeof(ExportRendererAttribute), typeof(ExportCellAttribute), typeof(ExportImageSourceHandlerAttribute) });
			ExpressionSearch.Default = new UnityExpressionSearch();
		}

		private void OnDestroy()
		{
			Device.PlatformServices = null;
			Device.SetIdiom(TargetIdiom.Unsupported);
			Device.Info = null;
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Property

		public Thread MainThread => _mainThread;

		#endregion
	}
}
