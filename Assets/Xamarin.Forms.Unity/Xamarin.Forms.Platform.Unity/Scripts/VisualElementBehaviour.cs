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
		}

		#endregion

		/*-------------------------------------------------------------*/
		#region Property

		public RectTransform RectTransform => _rectTransform;

		#endregion
	}
}
