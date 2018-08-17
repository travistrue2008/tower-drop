/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using UnityEngine;

namespace TRUEStudios.State {
	public abstract class Service : MonoBehaviour {
		#region Methods
		private bool _initialized = false;

		// Services use OnInitialize() instead of Awake() to prevent multiple instances of
		// Service components from initializing during the development cycle when an instance
		// of the Services prefab may exist in multiple scenes for direct-scene testing.
		protected virtual void OnInitialize () { }

		public void Initialize() {
			// only process sub-classed initialization if it hasn't been initialized already
			if (!_initialized) {
				OnInitialize();
				_initialized = true;
			}
		}


		// prevent from overriding...
		private void Awake () { }
		private void Start () { }
		#endregion
	}
}
