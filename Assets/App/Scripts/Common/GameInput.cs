using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TRUEStudios.State;

public class GameInput : MonoBehaviour {
	#region Constants
	public const float MaxRotationSpeed = 180.0f;
	#endregion

	#region Fields
	private float _xLastMouse;
	private GamepadService _gamepadService;
	#endregion

	#region Properties
	public bool IsPlaying { set; get; }
	#endregion

	#region MonoBehaviour Hooks
	private void Start () {
		_gamepadService = Services.Get<GamepadService>();
	}

	private void Update () {
		if (!IsPlaying) { return; }
		float frameIncrementAngle = MaxRotationSpeed * Time.deltaTime;
		ProcessMouse(frameIncrementAngle);
		ProcessKeyboard(frameIncrementAngle);
		ProcessGamepad(frameIncrementAngle);
	}
	#endregion

	#region Actions
	public void Reset () {
		IsPlaying = true;
		transform.Rotate(0.0f, 0.0f, 0.0f);
	}
	#endregion

	#region Private Methods
	private void ProcessMouse (float incrementAngle) {
		if (Input.GetMouseButton(0)) {
			float delta = (Input.mousePosition.x - _xLastMouse);
			transform.Rotate(0.0f, -delta, 0.0f);
			_xLastMouse = Input.mousePosition.x;
		}
	}

	private void ProcessKeyboard (float incrementAngle) {
		float delta = 0.0f;

		// handle keyboard input
		delta -= Input.GetKey(KeyCode.LeftArrow) ? incrementAngle : 0.0f;
		delta += Input.GetKey(KeyCode.RightArrow) ? incrementAngle : 0.0f;
		transform.Rotate(0.0f, delta, 0.0f);
	}

	private void ProcessGamepad (float incrementAngle) {
		// handle Gamepad input
		float delta = _gamepadService.LeftAxis.x * incrementAngle;
		if (delta != 0.0f) {
			transform.Rotate(0.0f, delta, 0.0f);
		}
	}
	#endregion
}
