/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using UnityEngine;

namespace TRUEStudios.Tweens {
	public class RotationTween : Tween<Vector3> {
		private enum Axis { X, Y, Z }

		public enum Mode {
			Nearest,
			Farthest,
			Clockwise,
			CounterClockwise,
		}

		#region Fields
		[SerializeField]
		private Mode _xMode = Mode.Nearest;
		[SerializeField]
		private Mode _yMode = Mode.Nearest;
		[SerializeField]
		private Mode _zMode = Mode.Nearest;

		private Vector3 _diff = Vector3.zero;
		private Vector3 _fixedBegin = Vector3.zero;
		private Vector3 _fixedEnd = Vector3.zero;
		#endregion

		#region Properties
		public Vector3 fixedBegin { get { return _fixedBegin; } }
		public Vector3 fixedEnd { get { return _fixedEnd; } }

		public Mode xMode {
			set {
				_xMode = value;
				_diff.x = GetRotationAngle(_xMode, _fixedEnd.x - _fixedBegin.x);
			}

			get { return _xMode; }
		}

		public Mode yMode {
			set {
				_yMode = value;
				_diff.y = GetRotationAngle(_yMode, _fixedEnd.y - _fixedBegin.y);
			}

			get { return _yMode; }
		}

		public Mode zMode {
			set {
				_zMode = value;
				_diff.z = GetRotationAngle(_zMode, _fixedEnd.z - _fixedBegin.z);
			}

			get { return _zMode; }
		}
		#endregion

		#region Override Methods
		protected override void BeginWillBeSet (Vector3 to) {
			_fixedBegin = BoundAngle(to);
			FindDifference();
		}

		protected override void EndWillBeSet (Vector3 to) {
			_fixedEnd = BoundAngle(to);
			FindDifference();
		}
		#endregion

		#region Methods
		public override void ApplyResult () {
			_result = ((_end - _begin) * DistributedValue) + _begin;
			TargetTransform.localRotation = Quaternion.Euler(_result);
		}

		private Vector3 BoundAngle (Vector3 eulerAngles) {
			// wrap all angles to a domain of 0 - 360 degrees
			while (eulerAngles.x < 0.0f) { eulerAngles.x = (180.0f + eulerAngles.x) + 180.0f; }
			while (eulerAngles.y < 0.0f) { eulerAngles.y = (180.0f + eulerAngles.y) + 180.0f; }
			while (eulerAngles.z < 0.0f) { eulerAngles.z = (180.0f + eulerAngles.z) + 180.0f; }
			while (eulerAngles.x >= 360.0f) { eulerAngles.x -= 360.0f; }
			while (eulerAngles.y >= 360.0f) { eulerAngles.y -= 360.0f; }
			while (eulerAngles.z >= 360.0f) { eulerAngles.z -= 360.0f; }
			return eulerAngles;
		}

		private void FindDifference () {
			_diff = _fixedEnd - _fixedBegin;
			_diff.x = GetRotationAngle(xMode, _diff.x);
			_diff.y = GetRotationAngle(yMode, _diff.y);
			_diff.z = GetRotationAngle(zMode, _diff.z);
		}

		private float GetRotationAngle (Mode mode, float angle) {
			// set the component based on rotation mode
			switch (mode) {
				case Mode.Nearest:
					if (Mathf.Abs(angle) > 180.0f) {
						if (angle > 0.0f) {
							angle = (360.0f - angle) * -1.0f;
						} else {
							angle = 360.0f - Mathf.Abs(angle);
						}
					}
					break;

				case Mode.Farthest:
					if (Mathf.Abs(angle) < 180.0f) {
						if (angle > 0.0f) {
							angle = (360.0f - angle) * -1.0f;
						} else {
							angle = 360.0f - Mathf.Abs(angle);
						}
					}
					break;

				case Mode.Clockwise:
					if (angle > 0.0f) {
						angle = (360.0f - angle) * -1.0f;
					}
					break;

				case Mode.CounterClockwise:
					if (angle < 0.0f) {
						angle = 360.0f - Mathf.Abs(angle);
					}
					break;
			}

			return angle;
		}
		#endregion
	}
}
