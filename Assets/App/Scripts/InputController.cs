using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class InputController : MonoBehaviour {
	#region Fields
	[SerializeField]
	private TextMeshProUGUI _debugText;

	private Vector2 _dPadAxis = Vector2.zero;
	private Vector2 _leftAxis = Vector2.zero;
	private Vector2 _rightAxis = Vector2.zero;
	#endregion

	#region Properties
	public float LeftTrigger { private set; get; }
	public float RightTrigger { private set; get; }
	public bool A { private set; get; }
	public bool B { private set; get; }
	public bool X { private set; get; }
	public bool Y { private set; get; }
	public bool Up { private set; get; }
	public bool Down { private set; get; }
	public bool Left { private set; get; }
	public bool Right { private set; get; }
	public bool Back { private set; get; }
	public bool Start { private set; get; }
	public bool LeftBumper { private set; get; }
	public bool RightBumper { private set; get; }
	public bool LeftThumbstick { private set; get; }
	public bool RightThumbstick { private set; get; }

	public Vector2 LeftAxis { get { return _leftAxis; } }
	public Vector2 RightAxis { get { return _rightAxis; } }
	#endregion

	#region Methods
	private void Update () {
		PollInput();

		if (_debugText != null) {
			PrintDebug();
		}
	}

	private void PollInput () {
		_dPadAxis.Set(Input.GetAxis("dpad_horizontal"), Input.GetAxis("dpad_vertical"));
		_leftAxis.Set(Input.GetAxis("left_x"), Input.GetAxis("left_y"));
		_rightAxis.Set(Input.GetAxis("right_x"), Input.GetAxis("right_y"));

		LeftTrigger = Input.GetAxis("trigger_left");
		RightTrigger = Input.GetAxis("trigger_right");
		Up = _dPadAxis.y > 0.5f;
		Down = _dPadAxis.y < -0.5f;
		Left = _dPadAxis.x < -0.5f;
		Right = _dPadAxis.x > 0.5f;

		A = Input.GetButton("button_a");
		B = Input.GetButton("button_b");
		X = Input.GetButton("button_x");
		Y = Input.GetButton("button_y");
		Back = Input.GetButton("button_back");
		Start = Input.GetButton("button_start");
		LeftBumper = Input.GetButton("bumper_left");
		RightBumper = Input.GetButton("bumper_right");
		LeftThumbstick = Input.GetButton("thumbstick_left");
		RightThumbstick = Input.GetButton("thumbstick_right");
	}

	private void PrintDebug () {
		_debugText.text = $@"LEGEND:
Triggers: ({LeftTrigger}, {RightTrigger})
Left Axis: {_leftAxis}
Right Axis: {_rightAxis}
{Up ? "Up" : ""}
{Down ? "Down" : ""}
{Left ? "Left" : ""}
{Right ? "Right" : ""}
{A ? "A" : ""}
{B ? "B" : ""}
{X ? "X" : ""}
{Y ? "Y" : ""}
{Back ? "Back" : ""}
{Start ? "Start" : ""}
{LeftBumper ? "LeftBumper" : ""}
{RightBumper ? "RightBumper" : ""}
{LeftThumbstick ? "LeftThumbstick" : ""}
{RightThumbstick ? "RightThumbstick" : ""}
		";
	}
	#endregion
}
