using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
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
	public class UnityFormsApplicationActivity<T> : UnityFormsApplicationActivity
		where T : Application, new()
	{
		/*-----------------------------------------------------------------*/
		#region Private Field

		UnityPlatform _platform;


		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			Forms.Init(this);
			_platform = new UnityPlatform(GetComponent<Canvas>());
		}

		private void Start()
		{
			LoadApplication(new App());
		}

		private void OnDestroy()
		{
			_platform = null;
			Forms.Uninit();
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Protected Method

		public void LoadApplication(Application app)
		{

			Application.SetCurrentApplication(app);
			_platform.SetPage(Application.Current.MainPage);
			app.PropertyChanged += OnApplicationPropertyChanged;

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
