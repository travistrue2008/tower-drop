/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TRUEStudios.Core;
using TMPro;

namespace TRUEStudios.State {
	public enum GamepadButton : int {
		A               = 1,
		B               = 2,
		X               = 4,
		Y               = 8,
		Up              = 16,
		Down            = 32,
		Left            = 64,
		Right           = 128,
		Back            = 256,
		Start           = 512,
		LeftBumper      = 1024,
		RightBumper     = 2048,
		LeftThumbstick  = 4096,
		RightThumbstick = 8192,
	}

	public class GamepadService : Service {
		#region Constants
		public const string AKey = "button_a";
		public const string BKey = "button_b";
		public const string XKey = "button_x";
		public const string YKey = "button_y";
		public const string BackKey = "button_back";
		public const string StartKey = "button_start";
		public const string LeftBumperKey = "bumper_left";
		public const string RightBumperKey = "bumper_right";
		public const string LeftThumbstickKey = "thumbstick_left";
		public const string RightThumbstickKey = "thumbstick_right";
		public const string LeftTriggerKey = "trigger_left";
		public const string RightTriggerKey = "trigger_right";
		public const string DpadHorizontalKey = "dpad_horizontal";
		public const string DpadVerticalKey = "dpad_vertical";
		public const string LeftXKey = "left_x";
		public const string LeftYKey = "left_y";
		public const string RightXKey = "right_x";
		public const string RightYKey = "right_y";
		#endregion

		#region Fields
		[SerializeField]
		private EventSystem _system;
		[SerializeField]
		private TextMeshProUGUI _debugText;
		[SerializeField]
		private IntEvent _onButtonPressed = new IntEvent();
		[SerializeField]
		private IntEvent _onButtonReleased = new IntEvent();

		private int _lastUpPad = 0;
		private int _lastDownPad = 0;
		private Vector2 _leftAxis = Vector2.zero;
		private Vector2 _rightAxis = Vector2.zero;
		private GameObject _target;
		private bool[] _lastDpad = new bool[] { false, false, false, false }; // UP, DOWN, LEFT, RIGHT
		#endregion

		#region Properties
		public EventSystem System { get { return _system; } }
		public IntEvent OnButtonPressed { get { return _onButtonPressed; } }
		public IntEvent OnButtonReleased { get { return _onButtonReleased; } }
		public bool A { get { return Input.GetButton(AKey); } }
		public bool B { get { return Input.GetButton(BKey); } }
		public bool X { get { return Input.GetButton(XKey); } }
		public bool Y { get { return Input.GetButton(YKey); } }
		public bool Up { get { return Input.GetAxis(DpadVerticalKey) > 0.5f; } }
		public bool Down { get { return Input.GetAxis(DpadVerticalKey) < -0.5f; } }
		public bool Left { get { return Input.GetAxis(DpadHorizontalKey) < -0.5f; } }
		public bool Right { get { return Input.GetAxis(DpadHorizontalKey) > 0.5f; } }
		public bool Back { get { return Input.GetButton(BackKey); } }
		public bool Start { get { return Input.GetButton(StartKey); } }
		public bool LeftBumper { get { return Input.GetButton(LeftBumperKey); } }
		public bool RightBumper { get { return Input.GetButton(RightBumperKey); } }
		public bool LeftThumbstick { get { return Input.GetButton(LeftThumbstickKey); } }
		public bool RightThumbstick { get { return Input.GetButton(RightThumbstickKey); } }
		public float LeftTrigger { get { return Input.GetAxis(LeftTriggerKey); } }
		public float RightTrigger { get { return Input.GetAxis(RightTriggerKey); } }

		public Vector2 LeftAxis {
			get {
				_leftAxis.Set(Input.GetAxis(LeftXKey), Input.GetAxis(LeftYKey));
				return _leftAxis;
			}
		}

		public Vector2 RightAxis {
			get {
				_rightAxis.Set(Input.GetAxis(RightXKey), Input.GetAxis(RightYKey));
				return _rightAxis;
			}
		}
		#endregion

		#region Private Methods
		protected override void OnInitialize() {
			_target = System.firstSelectedGameObject;
		}

		private void Update () {
			SyncEventSystem();
			ProcessButtonPresses();
			ProcessButtonReleases();
			UpdateDpad();

			// print debug info if provided
			if (_debugText != null) {
				PrintDebug();
			}
		}

		private void SyncEventSystem () {
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

		private void ProcessButtonPresses () {
			int pad = 0;
			pad |= A ? (int)GamepadButton.A : 0;
			pad |= B ? (int)GamepadButton.B : 0;
			pad |= X ? (int)GamepadButton.X : 0;
			pad |= Y ? (int)GamepadButton.Y : 0;
			pad |= (Up && !_lastDpad[0]) ? (int)GamepadButton.Up : 0;
			pad |= (Down && !_lastDpad[1]) ? (int)GamepadButton.Down : 0;
			pad |= (Left && !_lastDpad[2]) ? (int)GamepadButton.Left : 0;
			pad |= (Right && !_lastDpad[3]) ? (int)GamepadButton.Right : 0;
			pad |= Back ? (int)GamepadButton.Back : 0;
			pad |= Start ? (int)GamepadButton.Start : 0;
			pad |= LeftBumper ? (int)GamepadButton.LeftBumper : 0;
			pad |= RightBumper ? (int)GamepadButton.RightBumper : 0;
			pad |= LeftThumbstick ? (int)GamepadButton.LeftThumbstick : 0;
			pad |= RightThumbstick ? (int)GamepadButton.RightThumbstick : 0;
			
			// check if the gamepad's state has changed
			if (_lastDownPad != pad) {
				_onButtonPressed.Invoke(pad);
				_lastDownPad = pad;
			}
		}

		private void ProcessButtonReleases () {
			int pad = 0;
			pad |= A ? 0 : (int)GamepadButton.A;
			pad |= B ? 0 : (int)GamepadButton.B;
			pad |= X ? 0 : (int)GamepadButton.X;
			pad |= Y ? 0 : (int)GamepadButton.Y;
			pad |= (Up && !_lastDpad[0]) ? 0 : (int)GamepadButton.Up;
			pad |= (Down && !_lastDpad[1]) ? 0 : (int)GamepadButton.Down;
			pad |= (Left && !_lastDpad[2]) ? 0 : (int)GamepadButton.Left;
			pad |= (Right && !_lastDpad[3]) ? 0 : (int)GamepadButton.Right;
			pad |= Back ? 0 : (int)GamepadButton.Back;
			pad |= Start ? 0 : (int)GamepadButton.Start;
			pad |= LeftBumper ? 0 : (int)GamepadButton.LeftBumper;
			pad |= RightBumper ? 0 : (int)GamepadButton.RightBumper;
			pad |= LeftThumbstick ? 0 : (int)GamepadButton.LeftThumbstick;
			pad |= RightThumbstick ? 0 : (int)GamepadButton.RightThumbstick;

			// check if the gamepad's state has changed
			if (_lastUpPad != pad) {
				_onButtonReleased.Invoke(pad);
				_lastUpPad = pad;
			}
		}

		private void UpdateDpad () {
			// update last D-pad state
			_lastDpad[0] = Up;
			_lastDpad[1] = Down;
			_lastDpad[2] = Left;
			_lastDpad[3] = Right;
		}

		private void PrintDebug () {
			_debugText.text = $@"LEGEND:
Triggers: ({LeftTrigger}, {RightTrigger})
Left Axis: {LeftAxis}
Right Axis: {RightAxis}
Up: {Up}
Down: {Down}
Left: {Left}
Right: {Right}
A: {A}
B: {B}
X: {X}
Y: {Y}
Back: {Back}
Start: {Start}
LeftBumper: {LeftBumper}
RightBumper: {RightBumper}
LeftThumbstick: {LeftThumbstick}
RightThumbstick: {RightThumbstick}
			";
		}
		#endregion

		#region Actions
		public bool IsButtonPressed (int pad, GamepadButton button) {
			return (pad & (int)button) > 0;
		}

		public bool IsButtonReleased (int pad, GamepadButton button) {
			return (pad & (int)button) > 0;
		}
		#endregion
	}
}
