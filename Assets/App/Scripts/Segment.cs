using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Segment : MonoBehaviour {
	#region Constants
	public const int Resolution = 90;
	public const float Height = 0.75f;
	public const float Radius = 2.0f;
	public const float Thickness = 2.0f;
	public const float Slice = (Mathf.PI * 2.0f) / (float)Resolution;
	public const string CollisionMeshName = "Collider_Surface";

	private readonly Vector3 HeightOffset = new Vector3(0.0f, -Height, 0.0f);
	private readonly int[] CapIndices = new int[] {
		0, 1, 2, 0, 2, 3, // start cap
		5, 4, 7, 5, 7, 6, // end cap
	};
	#endregion

	#region Fields
	[SerializeField]
	private bool _hazard = false;
	[SerializeField]
	private int _span = 90;
	[SerializeField]
	private Material _platformMaterial;
	[SerializeField]
	private Material _hazardMaterial;

	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;
	private MeshCollider _meshCollider = null;
	#endregion

	#region Properties
	public bool Hazard {
		set {
			_hazard = value;
			if (_meshRenderer != null) {
				_meshRenderer.material = _hazard ? _hazardMaterial : _platformMaterial;
			}
		}

		get { return _hazard; }
	}

	public int Span {
		set {
			_span = Mathf.Clamp(value, 1, Resolution);
			BuildMesh();
		}

		get { return _span; }
	}
	#endregion

	#region Methods
	private void Awake () {
		_meshRenderer = GetComponent<MeshRenderer>();
		_meshFilter = GetComponent<MeshFilter>();
		if (_meshFilter != null) {
			_meshFilter.sharedMesh = new Mesh();
			_meshFilter.sharedMesh.name = "Ring";
		}

		BuildMesh();
	}

	private void OnValidate () {
		Hazard = _hazard;
		Span = _span;
	}
	#endregion

	#region Private Methods
	private void SetupMesh (Mesh mesh, string name, Vector3[] positions, int[] indices) {
		mesh.name = name;
		mesh.triangles = null;
		mesh.vertices = positions;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
	}

	private void BuildMesh () {
		if (_meshFilter == null) { return; }
		var indices = new int[((_span * 6) * 4) + 12];
		var positions = new Vector3[((_span + 1) * 8) + 8];

		// iterate through each segment
		SetSideIndices(0, indices);
		SetSideIndices(1, indices);
		SetSideIndices(2, indices);
		SetSideIndices(3, indices);
		SetPositionsForSide(0, false, false, true,  false, positions);
		SetPositionsForSide(1, true,  false, true,  true,  positions);
		SetPositionsForSide(2, true,  true,  false, true,  positions);
		SetPositionsForSide(3, false, true,  false, false, positions);
		BuildCaps(positions, indices);

		// update the mesh
		var mesh = _meshFilter.sharedMesh;
		mesh.triangles = null; // set to NULL, so that updating the vertices doesn't cause index buffer issues
		mesh.vertices = positions;
		mesh.triangles = indices;
		mesh.RecalculateNormals();

		if (Application.isPlaying) {
			BuildCollisionMesh();
		}
	}

	private void BuildCollisionMesh () {
		var positions = new Vector3[_span + 2 + 8];
		var indices = new int[_span * 3 + 12];
		BuildCaps(positions, indices);

		// setup positions
		positions[0] = Vector3.zero;
		for (int i = 1; i < positions.Length; ++i) {
			positions[i] = GetPosition(i, true, false);
		}

		// setup indices
		for (int i = 0; i < _span; ++i) {
			indices[(i * 3) + 0] = 0;
			indices[(i * 3) + 1] = i + 1;
			indices[(i * 3) + 2] = i;
		}

		// set the new sibling's transform
		var obj = new GameObject(CollisionMeshName);
		var sibling = obj.transform;
		sibling.SetParent(transform.parent);
		sibling.localScale = Vector3.one;
		sibling.localPosition = Vector3.zero;

		if (_meshCollider) {
			Destroy(_meshCollider.gameObject);
		}

		// setup the mesh collider
		var mesh = new Mesh();
		SetupMesh(mesh, "Surface", positions, indices);
		_meshCollider = sibling.gameObject.AddComponent<MeshCollider>();
		_meshCollider.sharedMesh = mesh;
	}

	private void BuildCaps (Vector3[] positions, int[] indices) {
		int vertexOffset = positions.Length - 8;
		int indexOffset = indices.Length - 12;

		// set inner cap's positions
		positions[vertexOffset + 0] = GetPosition(0, false, false);
		positions[vertexOffset + 1] = GetPosition(0, true,  false);
		positions[vertexOffset + 2] = GetPosition(0, true,  true);
		positions[vertexOffset + 3] = GetPosition(0, false, true);

		// set outer cap's positions
		positions[vertexOffset + 4] = GetPosition(_span, false, false);
		positions[vertexOffset + 5] = GetPosition(_span, true,  false);
		positions[vertexOffset + 6] = GetPosition(_span, true,  true);
		positions[vertexOffset + 7] = GetPosition(_span, false, true);

		// copy the indices
		for (int i = 0; i < 12; ++i) {
			indices[indexOffset + i] = CapIndices[i] + vertexOffset;
		}
	}

	private void SetSideIndices (int sideIndex, int[] indices) {
		// set the indices
		int vertexOffset = (_span + 1) * 2 * sideIndex;
		int indexOffset = _span * 6 * sideIndex;
		for (int i = 0; i < _span; ++i) {
			indices[indexOffset + (i * 6) + 0] = vertexOffset + (i * 2) + 0;
			indices[indexOffset + (i * 6) + 1] = vertexOffset + (i * 2) + 2;
			indices[indexOffset + (i * 6) + 2] = vertexOffset + (i * 2) + 3;
			indices[indexOffset + (i * 6) + 3] = vertexOffset + (i * 2) + 0;
			indices[indexOffset + (i * 6) + 4] = vertexOffset + (i * 2) + 3;
			indices[indexOffset + (i * 6) + 5] = vertexOffset + (i * 2) + 1;
		}
	}

	private void SetPositionsForSide (int index, bool startOuter, bool startLower, bool endOuter, bool endLower, Vector3[] positions) {
		int vertexOffset = (_span + 1) * 2 * index;
		for (int i = 0; i < (_span + 1); ++i) {
			positions[vertexOffset + (i * 2) + 0] = GetPosition(i, startOuter, startLower);
			positions[vertexOffset + (i * 2) + 1] = GetPosition(i, endOuter, endLower);
		}
	}

	private Vector3 GetPosition (int index, bool outer, bool lower) {
		float angle = (float)index * Slice;
		float multiplier = outer ? (Radius + Thickness) : Radius;
		Vector3 unitPosition = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
		Vector3 position = unitPosition * multiplier;
		return lower ? (position + HeightOffset) : position;
	}
	#endregion
}
