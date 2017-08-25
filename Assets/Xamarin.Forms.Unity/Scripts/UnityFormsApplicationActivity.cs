using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
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
		public Button _prefabButton;
		public Text _prefabText;
		public Slider _prefbSlider;
		public CanvasRenderer _prefabPanel;
		public Canvas _prefabCanvas;

		/// <summary>
		/// 指定の VisualElement に対応する VisualElementRenderer のインスタンスを取得する。
		/// </summary>
		/// <remarks>
		/// Unity の構造上、Registrar.GetHandler 経由でのインスタンス取得ができないので。
		/// </remarks>
		/// <param name="type"></param>
		/// <returns></returns>
		public IVisualElementRenderer GetVisualElementRenderer(System.Type type)
		{
			//	最終的にはもうちょっと頭のいい実装にする
			if (IsCompatibleType(type, typeof(Label)))
			{
				var newInstance = UnityEngine.Object.Instantiate(_prefabText);
				return newInstance.gameObject.AddComponent<LabelRenderer>();
			}
			if (IsCompatibleType(type, typeof(Page)))
			{
				var newInstance = UnityEngine.Object.Instantiate(_prefabCanvas);
				return newInstance.gameObject.AddComponent<PageRenderer>();
			}
			else
			{
				//	Default Renderer は Canvas コンポーネントに適用して返す
				var newInstance = UnityEngine.Object.Instantiate(_prefabCanvas);
				return newInstance.gameObject.AddComponent<DefaultRenderer>();
			}

			return null;
		}

		static bool IsCompatibleType(System.Type target, System.Type baseType)
		{
			return target == baseType || target.IsSubclassOf(baseType);
		}
	}

	/// <summary>
	/// Xamarin.Forms の初期化をする MonoBehavior。
	/// </summary>
	[DisallowMultipleComponent]
	public class UnityFormsApplicationActivity<T> : UnityFormsApplicationActivity
		where T : Application, new()
	{
		/*-----------------------------------------------------------------*/
		#region Field

		UnityPlatform _platform;

		//	Platform / PlatformRenderer が使用する Root Canvas
		public Canvas _xamarinFormsPlatformCanvas;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			Forms.Init(this);
			_platform = new UnityPlatform(this, _xamarinFormsPlatformCanvas);
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

		private void OnApplicationFocus(bool focus)
		{
			
		}

		private void OnApplicationPause(bool pause)
		{
			
		}

		#endregion

		/*-----------------------------------------------------------------*/
		#region Protected Method

		protected void LoadApplication(T app)
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
