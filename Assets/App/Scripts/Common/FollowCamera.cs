using UnityEngine;

public class FollowCamera : MonoBehaviour {
	#region Constants
	public const float MaxDropDistance = 1.0f;
	#endregion

	#region Fields
	[SerializeField]
	private Transform _target;

	private Vector3 _initialPosition;
	private Vector3 _cachedPosition;
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_initialPosition = _cachedPosition = transform.position;
	}

	private void LateUpdate () {
		if (_target == null) { return; }

		// check if the target dropped too far
		if (transform.position.y - _target.transform.position.y > MaxDropDistance) {
			_cachedPosition.y = _target.transform.position.y + MaxDropDistance;
			transform.position = _cachedPosition;
		}
	}
	#endregion

	#region Actions
	public void Reset () {
		transform.position = _initialPosition;
	}
	#endregion
}
