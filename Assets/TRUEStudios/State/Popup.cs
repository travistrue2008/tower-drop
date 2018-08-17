/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.Events;

using TRUEStudios.Tweens;

namespace TRUEStudios.State {
	public class Popup : MonoBehaviour {
		#region Fields
		[SerializeField]
		private Tween _transitionTween;
		[SerializeField]
		private UnityEvent _onClose = new UnityEvent();
		#endregion

		#region Properties
		public Tween TransitionTween { get { return _transitionTween; } }
		public UnityEvent OnClose { get { return _onClose; } }
		#endregion

		#region Setup
		protected virtual void Awake () {
			if (_transitionTween == null) {
				_transitionTween = GetComponent<Tween>();
			}
		}

		protected virtual void OnDestroy () {
			// signal the closed event
			if (_onClose != null) {
				_onClose.Invoke();
			}
		}

		public void Dismiss () {
			// only pop the last popup off the stack if it's this particular popup
			if (Services.Get<PopupService>().currentPopup == this) {
				Services.Get<PopupService>().PopPopup();
			} else {
				Debug.LogWarning("This popup isn't the active popup.");
			}
		}
		#endregion
	}
}
