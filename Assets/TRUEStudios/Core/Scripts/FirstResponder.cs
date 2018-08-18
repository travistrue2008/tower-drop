/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRUEStudios.State;

namespace TRUEStudios.Core {
	public class FirstResponder : MonoBehaviour {
		private static FirstResponder _instance;

		private void Awake () {
			// check if an object already exists
			if (_instance != null) {
				throw new Exception("Only a single instance of 'FirstResponder` should exist at a time.");
			}

			_instance = this;
		}

		private void Start () {
			Services.Get<EventService>().System.SetSelectedGameObject(gameObject);
		}

		private void OnDestroy () {
			// unset the singleton reference if it is the current instance
			if (_instance == this) {
				_instance = null;
			}
		}
	}
}
