using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Xamarin.Forms.Platform.Unity
{
	public class ButtonRenderer : ViewRenderer<Button, UnityEngine.UI.Button>
	{
		/*-----------------------------------------------------------------*/
		#region Field

		UnityEngine.UI.Text _componentText;

		#endregion

		/*-----------------------------------------------------------------*/
		#region MonoBehavior

		protected override void Awake()
		{
			base.Awake();

			var button = UnityComponent;
			if (button != null)
			{
				button.OnClickAsObservable()
					.Subscribe(_ => (Element as IButtonController)?.SendClicked())
					.AddTo(this);
			}

			_componentText = this.GetComponentInChildren<UnityEngine.UI.Text>();
		}

		#endregion
	}
}
