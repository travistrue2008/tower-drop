using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class Segment : MonoBehaviour {
	#region Constants
	public const int Resolution = 90;
	public const float Height = 0.35f;
	public const float Radius = 2.0f;
	public const float FallPower = 10.0f;
	public const float DestructDuration = 1.0f;
	public const float Slice = (Mathf.PI * 2.0f) / (float)Resolution;
	public static float DegreesPerSlice = 360.0f / (float)Resolution;
	public const string MeshName = "Surface";

	private readonly Vector3 HeightOffset = new Vector3(0.0f, -Height, 0.0f);
	private readonly int[] CapIndices = new int[] {
		0, 1, 2, 0, 2, 3, // start cap
		5, 4, 7, 5, 7, 6, // end cap
	};
	#endregion

	#region Fields
	[SerializeField]
	private bool _isHazard = false;
	[SerializeField]
	private int _span = 90;
	[SerializeField]
	private Material _platformMaterial;
	[SerializeField]
	private Material _hazardMaterial;
	[SerializeField]
	private Material _slamMaterial;

	private Rigidbody _rigidBody = null;
	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;
	private MeshCollider _meshCollider = null;
	#endregion

	#region Properties
	public bool IsFalling { get { return _rigidBody != null; } }
	public Ring ParentRing { get { return transform.parent.GetComponent<Ring>(); } }

	public bool IsHazard {
		set {
			_isHazard = value;
			if (_meshRenderer != null) {
				_meshRenderer.material = _isHazard ? _hazardMaterial : _platformMaterial;
			}
		}

		get { return _isHazard; }
	}

	public int Span {
		set {
			_span = Mathf.Clamp(value, 1, Resolution);
			BuildMesh();
		}

		get { return _span; }
	}
	#endregion

	#region MonoBehaviour Hooks
	private void Awake () {
		_meshRenderer = GetComponent<MeshRenderer>();
		_meshCollider = GetComponent<MeshCollider>();
		_meshFilter = GetComponent<MeshFilter>();

		// build the mesh
		var mesh = new Mesh();
		mesh.name = MeshName;
		_meshFilter.sharedMesh = _meshCollider.sharedMesh = mesh;
		BuildMesh();
	}

	private void OnValidate () {
		IsHazard = _isHazard;
		Span = _span;
	}
	#endregion

	#region Public Methods
	public void Fall (bool slammed = false) {
		if (IsFalling) { return; }

		// change the material if slammed
		if (slammed) {
			_meshRenderer.material = _slamMaterial;
		}

		// attach the rigid body
		_rigidBody = gameObject.AddComponent<Rigidbody>();
		float angle = ((((float)_span * DegreesPerSlice) / 2.0f) + transform.eulerAngles.y) * Mathf.Deg2Rad;
		var direction = new Vector3(Mathf.Cos(angle), 0.0f, -Mathf.Sin(angle));
		var right = Vector3.Cross(direction, Vector3.up);

		// set velocities, and destroy GameObjects
		_rigidBody.velocity = direction * FallPower;
		_rigidBody.angularVelocity = right;
		_meshCollider.enabled = false;
		Destroy(gameObject, DestructDuration);
	}
	#endregion

	#region Private Methods
	private void BuildMesh () {
		if (_meshFilter == null) { return; }
		var mesh = _meshFilter.sharedMesh;
		var indices = new int[(_span * 12) + CapIndices.Length];
		var positions = new Vector3[((_span + 1) * 4) + 10]; // each side needs its own set of vertices for decent lighting

		// setup vertex positions
		SetupCaps(positions, indices);
		SetPositionsForRim(0, false, positions);
		SetPositionsForRim(1, true, positions);
		SetPositionsForRim(2, false, positions);
		SetPositionsForRim(3, true, positions);
		SetIndices(indices);

		// update the mesh
		mesh.Clear();
		mesh.vertices = positions;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
	}

	private void SetupCaps (Vector3[] positions, int[] indices) {
		positions[0] = GetPosition(0, false);     // start-upper-rim
		positions[1] = Vector3.zero;              // start-upper-middle
		positions[2] = HeightOffset;              // start-lower-middle
		positions[3] = GetPosition(0, true);      // start-lower-rim

		positions[4] = GetPosition(_span, false); // end-upper-rim
		positions[5] = Vector3.zero;              // end-upper-middle
		positions[6] = HeightOffset;              // end-lower-middle
		positions[7] = GetPosition(_span, true);  // end-lower-rim

		positions[8] = Vector3.zero;              // upper-middle
		positions[9] = HeightOffset;              // lower-middle
		Array.Copy(CapIndices, indices, CapIndices.Length);
	}

	private void SetPositionsForRim (int index, bool lower, Vector3[] positions) {
		int vertexOffset = ((_span + 1) * index) + 10;
		for (int i = 0; i < (_span + 1); ++i) {
			positions[i + vertexOffset] = GetPosition(i, lower);
		}
	}

	private void SetIndices (int[] indices) {
		int bottomVertexOffset = _span + 11;
		int bottomIndexOffset = (_span * 3) + CapIndices.Length;
		int rimVertexOffset = ((_span + 1) * 2) + 10;
		int rimIndexOffset = (_span * 6) + CapIndices.Length;

		// iterate through each slice
		for (int i = 0; i < _span; ++i) {
			// top
			indices[CapIndices.Length + (i * 3) + 0] = 8;
			indices[CapIndices.Length + (i * 3) + 1] = i + 10;
			indices[CapIndices.Length + (i * 3) + 2] = i + 11;

			// bottom
			indices[bottomIndexOffset + (i * 3) + 0] = 9;
			indices[bottomIndexOffset + (i * 3) + 1] = bottomVertexOffset + i + 1;
			indices[bottomIndexOffset + (i * 3) + 2] = bottomVertexOffset + i;

			// rim
			indices[rimIndexOffset + (i * 6) + 0] = rimVertexOffset + i;
			indices[rimIndexOffset + (i * 6) + 1] = rimVertexOffset + i + (_span + 1);
			indices[rimIndexOffset + (i * 6) + 2] = rimVertexOffset + i + 1;
			indices[rimIndexOffset + (i * 6) + 3] = rimVertexOffset + i + 1;
			indices[rimIndexOffset + (i * 6) + 4] = rimVertexOffset + i + (_span + 1);
			indices[rimIndexOffset + (i * 6) + 5] = rimVertexOffset + i + (_span + 2);
		}
	}

	private Vector3 GetPosition (int index, bool lower) {
		float angle = (float)index * Slice;
		var unitPosition = new Vector3(Mathf.Cos(angle), 0.0f, -Mathf.Sin(angle));
		var position = unitPosition * Radius;
		return lower ? (position + HeightOffset) : position;
	}
	#endregion
}
