/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TRUEStudios.Core;
using TMPro;

namespace TRUEStudios.State {
	public class EventService : Service {
		#region Fields
		private GameObject _target;
		private EventSystem _system;
		#endregion

		#region Properties
		public EventSystem System {
			get {
				// cache the child EventSystem if not already
				if (_system == null) {
					_system = GetComponentInChildren<EventSystem>();
				}

				// make sure a child was found after caching
				if (_system == null) {
					throw new NullReferenceException("No EventSystem component was found for Services.");
				}

				return _system;
			}
		}
		#endregion

		#region MonoBehaviour Hooks
		protected override void OnInitialize() {
			_target = System.firstSelectedGameObject;
		}

		private void Update () {
			// check if the target GameObject is out-of-sync with the EventSystem's selected GameObject
			if (_target != System.currentSelectedGameObject) {
				// check if no GameObject is currently selected
				if (System.currentSelectedGameObject == null) {
					System.SetSelectedGameObject(_target); // re-select the target GameObject
				} else {
					_target = System.currentSelectedGameObject; // update the target GameObject
				}
			}
		}
		#endregion
	}
}
