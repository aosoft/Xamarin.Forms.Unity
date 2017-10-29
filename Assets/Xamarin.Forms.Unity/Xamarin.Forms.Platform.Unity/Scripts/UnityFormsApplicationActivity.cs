using System;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xamarin.Forms.Internals;

using Xamarin.Forms.Platform.Unity;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Label), typeof(LabelRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(ButtonRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Entry), typeof(EntryRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Editor), typeof(EditorRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Switch), typeof(SwitchRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Slider), typeof(SliderRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Picker), typeof(PickerRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.ScrollView), typeof(ScrollViewRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Page), typeof(PageRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Layout), typeof(LayoutRenderer))]

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
		public UnityEngine.UI.Slider _prefabSlider;
		public UnityEngine.UI.InputField _prefabInputField;
		public UnityEngine.CanvasRenderer _prefabPanel;
		public UnityEngine.Canvas _prefabCanvas;
		public UnityEngine.UI.Toggle _prefabToggle;
		public UnityEngine.UI.Dropdown _prefabDropdown;
		public UnityEngine.UI.ScrollRect _prefabScrollView;

		Dictionary<Type, UnityEngine.Component> _prefabs;

		protected virtual void Awake()
		{
			_prefabs = new Dictionary<Type, UnityEngine.Component>();
			InternalAddPrefab(_prefabButton);
			InternalAddPrefab(_prefabText);
			InternalAddPrefab(_prefabSlider);
			InternalAddPrefab(_prefabInputField);
			InternalAddPrefab(_prefabPanel);
			InternalAddPrefab(_prefabCanvas);
			InternalAddPrefab(_prefabToggle);
			InternalAddPrefab(_prefabDropdown);
			InternalAddPrefab(_prefabScrollView);
		}

		void InternalAddPrefab(UnityEngine.Component o)
		{
			if (o != null)
			{
				_prefabs.Add(o.GetType(), o);
			}
		}

		/// <summary>
		/// 指定の VisualElement に対応する VisualElementRenderer のインスタンスを取得する。
		/// </summary>
		/// <returns></returns>
		public IVisualElementRenderer CreateVisualElementRenderer(System.Type type)
		{
			var t = Internals.Registrar.Registered.GetHandlerType(type);
			if (t == null)
			{
				t = typeof(DefaultRenderer);
			}

			//	TODO:
			//	t2 は type から対応する prefab のものを類推する必要がある。
			//	t は VisualElementRenderer なので t2 は別の方法が必要。
			Type t2 = null;

			return UnityEngine.Object.Instantiate(_prefabs[t2]).gameObject.AddComponent(t) as IVisualElementRenderer;
			/*
			if (IsCompatibleType(type, typeof(Label)))
			{
				return GetGameObject(_prefabText).AddComponent<LabelRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Button)))
			{
				return GetGameObject(_prefabButton).gameObject.AddComponent<ButtonRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Entry)))
			{
				return GetGameObject(_prefabInputField).gameObject.AddComponent<EntryRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Editor)))
			{
				return GetGameObject(_prefabInputField).gameObject.AddComponent<EditorRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Switch)))
			{
				return GetGameObject(_prefabToggle).gameObject.AddComponent<SwitchRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Slider)))
			{
				return GetGameObject(_prefabSlider).gameObject.AddComponent<SliderRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Picker)))
			{
				return GetGameObject(_prefabDropdown).gameObject.AddComponent<PickerRenderer>();
			}
			else if (IsCompatibleType(type, typeof(ScrollView)))
			{
				return GetGameObject(_prefabScrollView).AddComponent<ScrollViewRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Page)))
			{
				return GetGameObject(_prefabCanvas).AddComponent<PageRenderer>();
			}
			else if (IsCompatibleType(type, typeof(Layout)))
			{
				return GetGameObject(_prefabCanvas).AddComponent<LayoutRenderer>();
			}
			else
			{
				//	Default Renderer は Canvas コンポーネントに適用して返す
				return GetGameObject(_prefabCanvas).AddComponent<DefaultRenderer>();
			}
			*/
		}

		public GameObject CreateBaseComponent<T>() where T : UnityEngine.Component
		{
			var t = typeof(T);
			if (_prefabs.ContainsKey(t))
			{
				return UnityEngine.Object.Instantiate<T>(_prefabs[t] as T).gameObject;
			}
			return null;
		}

		static GameObject GetGameObject<T>(T original) where T : UnityEngine.Component
		{
			return UnityEngine.Object.Instantiate<T>(original).gameObject;
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

		protected override void Awake()
		{
			base.Awake();

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
			if (pause)
			{
				Application.Current?.SendSleepAsync();
			}
			else
			{
				Application.Current?.SendResume();
			}
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
