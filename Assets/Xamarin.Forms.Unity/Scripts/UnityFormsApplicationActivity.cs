using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	/// <summary>
	/// Xamarin.Forms の初期化をする MonoBehavior のベースクラス。
	/// UI の生成元となる Prefab をここで管理する。
	/// </summary>
	[DisallowMultipleComponent]
	public abstract class UnityFormsApplicationActivity : MonoBehaviour
	{
		public UnityEngine.UI.Button _prefabButton;
		public UnityEngine.UI.Text _prefabText;
		public UnityEngine.UI.Slider _prefbSlider;
		public UnityEngine.CanvasRenderer _prefabPanel;
	}

	/// <summary>
	/// Xamarin.Forms の初期化をする MonoBehavior。
	/// どこかの UI EventSystem にこれの継承クラスを AddComponent しておくことが Forms.Init 相当の処置になる。 
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventSystem))]
	public class UnityFormsApplicationActivity<T> : UnityFormsApplicationActivity
		where T : Application, new()
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		UnityPlatform _platform;
		Canvas _canvas;
		T _app;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			Forms.Init(this);
			_canvas = gameObject.AddComponent<Canvas>();
			_canvas.transform.parent.parent = null;
			_canvas.name = "Xamarin.Forms Platform";
			_platform = new UnityPlatform(_canvas);
		}

		private void Start()
		{
			LoadApplication(new T());
		}

		private void OnDestroy()
		{
			_platform = null;
			Forms.Uninit();
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Protected Method

		public void LoadApplication(T app)
		{

			Application.SetCurrentApplication(app);
			_platform.SetPage(Application.Current.MainPage);
			app.PropertyChanged += OnApplicationPropertyChanged;
			_app = app;
			Application.Current.SendStart();
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Event Handler

		void OnApplicationPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "MainPage")
				_platform.SetPage(Application.Current.MainPage);
		}

		#endregion
	}
}
