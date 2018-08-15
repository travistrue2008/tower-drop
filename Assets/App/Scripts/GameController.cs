using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	#region Constants
	public const float MaxRotationSpeed = 180.0f;
	#endregion

	#region Fields
	[SerializeField]
	private Ball _ball;
	[SerializeField]
	private Transform _ringsContainer;
	[SerializeField]
	private Transform _discardContainer;

	private int _ringsBroken = 0;
	private InputController _inputController;
	private Ring[] _rings;
	#endregion

	#region Properties
	public float Progress {
		get {
			return (float)_ringsBroken / (float)_rings.Length;
		}
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_inputController = GetComponent<InputController>();
		_rings = _ringsContainer.GetComponentsInChildren<Ring>();
	}

	private void Update () {
		float delta = -_inputController.LeftAxis.x * MaxRotationSpeed * Time.deltaTime;
		_ringsContainer.Rotate(0.0f, delta, 0.0f);
	}
	#endregion

	#region Public Methods
	public void BreakRing (Ring ring) {
		transform.SetParent(_discardContainer);
		ring.Break();
	}
	#endregion
}
