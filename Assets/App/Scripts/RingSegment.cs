using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class RingSegment : MonoBehaviour {
	#region Constants
	public const int Resolution = 90;
	public const float Height = 1.0f;
	public const float Radius = 1.0f;
	public const float Thickness = 2.0f;

	private readonly Vector3 HeightOffset = new Vector3(0.0f, -Height, 0.0f);
	private readonly int[] SliceIndices = new int[] {
		0, 4, 1, 4, 5, 1, // top indices
		3, 2, 6, 3, 6, 7, // bottom indices
		1, 5, 6, 1, 6, 2, // top indices
		4, 0, 3, 4, 3, 7, // top indices
	};
	#endregion

	#region Fields
	[SerializeField]
	private bool _hazard = false;
	[SerializeField]
	private int _span = 360;
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
		_meshCollider = GetComponent<MeshCollider>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_meshFilter = gameObject.GetComponent<MeshFilter>();
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
	private void BuildMesh () {
		if (_meshFilter == null) { return; }

		var indices = new int[_span * 24 + 12];
		var positions = new Vector3[(_span + 1) * 4];

		// iterate through each segment
		SetPositionsForSlice(0, positions);
		SetCapIndices(_span, indices);
		for (int i = 0; i < _span; ++i) {
			SetPositionsForSlice(i + 1, positions);
			SetIndicesForSlice(i, indices);
		}

		// update the mesh
		var mesh = _meshFilter.sharedMesh;
		mesh.triangles = null; // set to NULL, so that updating the vertices doesn't cause index buffer issues
		mesh.vertices = positions;
		mesh.normals = CalculateNormals(positions, indices);
		mesh.triangles = indices;
		// mesh.RecalculateNormals();
		_meshCollider.sharedMesh = mesh;
	}

	private Vector3[] CalculateNormals (Vector3[] positions, int[] indices) {
		// iterate through all triangles
		var normals = new Vector3[positions.Length];
		for (int i = 0; i < indices.Length / 3; ++i) {
			int[] localIndices = {
				indices[(i * 3) + 0],
				indices[(i * 3) + 1],
				indices[(i * 3) + 2],
			};

			Vector3[] triangle = {
				positions[localIndices[0]],
				positions[localIndices[1]],
				positions[localIndices[2]],
			};

			// find the triangle's normal, and add it to the affected vertex normals
			var left = triangle[1] - triangle[0];
			var right = triangle[2] - triangle[0];
			var normal = Vector3.Cross(left, right);
			normals[localIndices[0]] += normal;
			normals[localIndices[1]] += normal;
			normals[localIndices[2]] += normal;
		}

		for (int i = 0; i < normals.Length; ++i) {
			normals[i] = Vector3.Normalize(normals[i]);
		}

		return normals;
	}

	private void SetPositionsForSlice (int index, Vector3[] positions) {
		int startIndex = index * 4;
		float slice = (Mathf.PI * 2.0f) / (float)Resolution;
		float angle = (float)index * slice;
		var unitPos = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));

		positions[startIndex + 0] = unitPos * Radius;
		positions[startIndex + 1] = unitPos * (Radius + Thickness);
		positions[startIndex + 2] = (unitPos * (Radius + Thickness)) + HeightOffset;
		positions[startIndex + 3] = (unitPos * Radius) + HeightOffset;
	}

	private void SetIndicesForSlice (int index, int[] indices) {
		int startIndex = index * 24;
		int offset = index * 4;
		for (int i = 0; i < 24; ++i) {
			indices[i + startIndex] = SliceIndices[i] + offset;
		}
	}

	private void SetCapIndices (int span, int[] indices) {
		int startOffset = indices.Length - 12;
		int endOffset = span * 4;
		var capIndices = new int[] {
			// start cap
			0, 1, 2, 0, 2, 3,
			// end cap
			1 + endOffset,
			0 + endOffset,
			3 + endOffset,
			1 + endOffset,
			3 + endOffset,
			2 + endOffset,
		};

		for (int i = 0; i < 12; ++i) {
			indices[startOffset + i] = capIndices[i];
		}
	}
	#endregion
}
