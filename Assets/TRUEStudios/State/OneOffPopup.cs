/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using UnityEngine;

using TRUEStudios.State;

namespace TRUEStudios.Core {
	public class OneOffPopup : MonoBehaviour {
		#region Fields
		[SerializeField]
		private bool _attemptGC;
		[SerializeField]
		private Popup _popupPrefab;

		private PopupService _service;
		#endregion

		#region Properties
		public Popup PopupPrefab {
			set { _popupPrefab = value; }
			get { return _popupPrefab; }
		}
		#endregion

		#region Methods
		private void Start () {
			_service = Services.Get<PopupService>();
		}

		public void Show () {
			if (_popupPrefab == null) {
				throw new NullReferenceException("_popupPrefab not set in the Inspector.");
			}

			// load and push the popup specified by prefab
			Popup popup = _service.PushPopup<Popup>(_popupPrefab);
			if (popup != null) {
				popup.OnClose.AddListener(HandleOnClose);
			} else {
				throw new Exception("Unable to push popup with prefab: " + _popupPrefab);
			}
		}

		private void HandleOnClose () {
			// release the prefab, and attempt to clean up memory
			_service.ReleasePrefab(_popupPrefab.name);
			if (_attemptGC) {
				Services.Release();
			}
		}
		#endregion
	}
}
