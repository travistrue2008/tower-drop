using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class Segment : MonoBehaviour {
	#region Fields
	[SerializeField]
	private bool _isHazard = false;

	private Rigidbody _rigidBody = null;
	// private MeshFilter _meshFilter = null;
	// private MeshRenderer _meshRenderer = null;
	// private MeshCollider _meshCollider = null;
	#endregion

	#region Properties
	public bool IsFalling { get { return _rigidBody != null; } }
	public Ring ParentRing { get { return transform.parent.GetComponent<Ring>(); } }

	public bool IsHazard {
		set {
			_isHazard = value;
			// if (_meshRenderer != null) {
			// 	_meshRenderer.material = _isHazard ? _profile.HazardMaterial : _profile.PlatformMaterial;
			// }
		}

		get { return _isHazard; }
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		// _meshRenderer = GetComponent<MeshRenderer>();
		// _meshCollider = GetComponent<MeshCollider>();
		// _meshFilter = GetComponent<MeshFilter>();
	}
	#endregion

	#region Public Methods
	public void Fall (bool slammed = false) {
		// if (IsFalling) { return; }

		// // change the material if slammed
		// if (slammed) {
		// 	_meshRenderer.material = _profile.SlamMaterial;
		// }

		// // attach the rigid body
		// _rigidBody = gameObject.AddComponent<Rigidbody>();
		// float angle = ((((float)_span * _profile.DegreesPerSlice) / 2.0f) + transform.eulerAngles.y) * Mathf.Deg2Rad;
		// var direction = new Vector3(Mathf.Cos(angle), 0.0f, -Mathf.Sin(angle));
		// var right = Vector3.Cross(direction, Vector3.up);

		// // set velocities, and destroy GameObjects
		// _rigidBody.velocity = direction * _profile.FallPower;
		// _rigidBody.angularVelocity = right;
		// _meshCollider.enabled = false;
		// Destroy(gameObject, _profile.DestructDuration);
	}
	#endregion
}
