using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Unity
{
	public class VisualElementBehaviour : MonoBehaviour
	{
		/*-------------------------------------------------------------*/
		#region Field

		RectTransform _rectTransform;
		CanvasGroup _canvasGroup;

		#endregion

		/*-------------------------------------------------------------*/
		#region MonoBehavior

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			if (_rectTransform == null)
			{
				_rectTransform = gameObject.AddComponent<RectTransform>();
			}

			_canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}

		#endregion

		/*-------------------------------------------------------------*/
		#region Property

		public RectTransform RectTransform => _rectTransform;

		public CanvasGroup CanvasGroup
		{
			get
			{
				//	必要になった段階で CanvasGroup を追加する。
				if (_canvasGroup == null)
				{
					_canvasGroup = gameObject.AddComponent<CanvasGroup>();
				}
				return _canvasGroup;
			}
		}

		/// <summary>
		/// 不透明度
		/// </summary>
		/// <remarks>
		/// CanvasGroup で表現するが不透明度の設定が必要になるまで CanvasGroup は生成されないようにする。
		/// </remarks>
		public double Opacity
		{
			get
			{
				return _canvasGroup != null ? _canvasGroup.alpha : 1.0;
			}

			set
			{
				if (_canvasGroup == null && value >= 1.0)
				{
					return;
				}

				CanvasGroup.alpha = (float)value;
			}
		}

		#endregion
	}
}
