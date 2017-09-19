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
		public UnityEngine.UI.Button _prefabButton;
		public Text _prefabText;
		public Slider _prefbSlider;
		public InputField _prefabInputField;
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
		public IVisualElementRenderer CreateVisualElementRenderer(System.Type type, GameObject target = null)
		{
			if (IsCompatibleType(type, typeof(Label)))
			{
				return GetGameObject(target, _prefabText).AddComponent<LabelRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Button)))
			{
				return GetGameObject(target, _prefabButton).gameObject.AddComponent<ButtonRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Entry)))
			{
				return GetGameObject(target, _prefabInputField).gameObject.AddComponent<EntryRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Editor)))
			{
				return GetGameObject(target, _prefabInputField).gameObject.AddComponent<EditorRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Page)))
			{
				return GetGameObject(target, _prefabCanvas).AddComponent<PageRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Layout)))
			{
				return GetGameObject(target, _prefabCanvas).AddComponent<LayoutRenderer>();
			}
			else
			{
				//	Default Renderer は Canvas コンポーネントに適用して返す
				return GetGameObject(target, _prefabCanvas).AddComponent<DefaultRenderer>();
			}
		}

		static GameObject GetGameObject<T>(GameObject target, T original) where T : UnityEngine.Component
		{
			if (target == null)
			{
				target = UnityEngine.Object.Instantiate<T>(original).gameObject;
			}
			return target;
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

		Platform _platform;

		//	Platform / PlatformRenderer が使用する Root Canvas
		public Canvas _xamarinFormsPlatformCanvas;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			Forms.Init(this);
			_platform = new Platform(this, _xamarinFormsPlatformCanvas);
		}

		private void Start()
		{
			LoadApplication(new T());
		}

		private void OnDestroy()
		{
			_platform?.Dispose();
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
