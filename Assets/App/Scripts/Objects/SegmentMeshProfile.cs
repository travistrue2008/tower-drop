using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Segment Mesh Profile", fileName = "Segment Mesh Profile")]
public class SegmentMeshProfile : ScriptableObject {
	#region Constants
	public const string MeshNamePrefix = "Surface";

	private readonly int[] CapIndices = new int[] {
		0, 1, 2, 0, 2, 3, // start cap
		5, 4, 7, 5, 7, 6, // end cap
	};
	#endregion

	#region Fields
	public Material PlatformMaterial;
	public Material HazardMaterial;
	public Material SlamMaterial;

	[SerializeField]
	private int _resolution = 90;
	[SerializeField]
	private float _radius = 2.0f;
	[SerializeField]
	private float _fallPower = 10.0f;
	[SerializeField]
	private float _destructDuration = 1.0f;
	[SerializeField]
	private float _height = 0.35f;
	[SerializeField, HideInInspector]
	private Vector3 _heightOffset = new Vector3(0.0f, -0.35f, 0.0f);

	private Mesh[] _meshes = new Mesh[0];
	#endregion

	#region Properties
	public Vector3 HeightOffset { get { return _heightOffset; } }
	public Mesh[] Meshes { get { return _meshes; } }

	public float Slice {
		get { return (Mathf.PI * 2.0f) / (float)_resolution; }
	}

	public float DegreesPerSlice {
		get { return 360.0f / (float)_resolution; }
	}

	public int Resolution {
		set { _resolution = Mathf.Clamp(value, 3, 360); }
		get { return _resolution; }
	}

	public float Radius {
		set { _radius = Mathf.Max(Mathf.Epsilon, value); }
		get { return _radius; }
	}

	public float FallPower {
		set { _fallPower = Mathf.Max(Mathf.Epsilon, value); }
		get { return _fallPower; }
	}

	public float DestructDuration {
		set { _destructDuration = Mathf.Max(0.0f, value); }
		get { return _destructDuration; }
	}

	public float Height {
		set {
			_height = Mathf.Max(Mathf.Epsilon, value);
			_heightOffset = new Vector3(0.0f, -_height, 0.0f);
		}

		get { return _height; }
	}
	#endregion

	#region Actions
	public void Apply () {
		// rebuild the mesh cache
		_meshes = new Mesh[_resolution];
		for (int i = 0; i < _resolution; ++i) {
			_meshes[i] = BuildMesh(i+1);
		}
	}
	#endregion

	#region Private Methods
	private Mesh BuildMesh (int numSegments) {
		var indices = new int[(numSegments * 12) + CapIndices.Length];
		var positions = new Vector3[((numSegments + 1) * 4) + 10]; // each side needs its own set of vertices for decent lighting

		// setup vertex positions
		SetupCaps(numSegments, positions, indices);
		SetPositionsForRim(numSegments, 0, false, positions);
		SetPositionsForRim(numSegments, 1, true, positions);
		SetPositionsForRim(numSegments, 2, false, positions);
		SetPositionsForRim(numSegments, 3, true, positions);
		SetIndices(numSegments, indices);

		// update the mesh
		var mesh = new Mesh();
		mesh.name = $"{MeshNamePrefix}_{numSegments}";
		mesh.vertices = positions;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
		return mesh;
	}

	private void SetupCaps (int numSegments, Vector3[] positions, int[] indices) {
		positions[0] = GetPosition(0, false);           // start-upper-rim
		positions[1] = Vector3.zero;                    // start-upper-middle
		positions[2] = _heightOffset;                   // start-lower-middle
		positions[3] = GetPosition(0, true);            // start-lower-rim

		positions[4] = GetPosition(numSegments, false); // end-upper-rim
		positions[5] = Vector3.zero;                    // end-upper-middle
		positions[6] = _heightOffset;                   // end-lower-middle
		positions[7] = GetPosition(numSegments, true);  // end-lower-rim

		positions[8] = Vector3.zero;                    // upper-middle
		positions[9] = _heightOffset;                   // lower-middle
		Array.Copy(CapIndices, indices, CapIndices.Length);
	}

	private void SetPositionsForRim (int numSegments, int index, bool lower, Vector3[] positions) {
		int vertexOffset = ((numSegments + 1) * index) + 10;
		for (int i = 0; i < (numSegments + 1); ++i) {
			positions[i + vertexOffset] = GetPosition(i, lower);
		}
	}

	private void SetIndices (int numSegments, int[] indices) {
		int bottomVertexOffset = numSegments + 11;
		int bottomIndexOffset = (numSegments * 3) + CapIndices.Length;
		int rimVertexOffset = ((numSegments + 1) * 2) + 10;
		int rimIndexOffset = (numSegments * 6) + CapIndices.Length;

		// iterate through each slice
		for (int i = 0; i < numSegments; ++i) {
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
			indices[rimIndexOffset + (i * 6) + 1] = rimVertexOffset + i + (numSegments + 1);
			indices[rimIndexOffset + (i * 6) + 2] = rimVertexOffset + i + 1;
			indices[rimIndexOffset + (i * 6) + 3] = rimVertexOffset + i + 1;
			indices[rimIndexOffset + (i * 6) + 4] = rimVertexOffset + i + (numSegments + 1);
			indices[rimIndexOffset + (i * 6) + 5] = rimVertexOffset + i + (numSegments + 2);
		}
	}

	private Vector3 GetPosition (int index, bool lower) {
		float angle = (float)index * Slice;
		var unitPosition = new Vector3(Mathf.Cos(angle), 0.0f, -Mathf.Sin(angle));
		var position = unitPosition * _radius;
		return lower ? (position + _heightOffset) : position;
	}
	#endregion
}
