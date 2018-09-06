using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRUEStudios.Foundation.Variables;

public class AudioListenerWrapper : MonoBehaviour {
	#region Fields
	[SerializeField]
	private FloatReference _volumeReference;
	#endregion

	#region Properties
	public float Volume {
		set { AudioListener.volume = Mathf.Clamp01(value); }
		get { return AudioListener.volume; }
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		Volume = _volumeReference.Value;
	}
	#endregion
}
