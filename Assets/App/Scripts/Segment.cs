using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class Segment : MonoBehaviour {
	public enum Type {
		None,
		Hazard,
		Platform,
	}

	#region Fields
	[SerializeField]
	private Type _type = Type.Platform;
	[SerializeField]
	private int _resolution = 360;
	[SerializeField]
	private int _span = 360;
	[SerializeField]
	private float _radius = 32.0f;
	[SerializeField]
	private float _thickness = 16.0f;
	[SerializeField]
	private float _height = 8.0f;
	[SerializeField]
	private Material _platformMaterial;
	[SerializeField]
	private Material _hazardMaterial;

	private MeshFilter _meshFilter = null;
	private MeshRenderer _meshRenderer = null;
	#endregion

	#region Methods
	private void Awake () {
		_meshFilter = gameObject.GetComponent<MeshFilter>();
		_meshFilter.sharedMesh = GenerateMesh();

		// setup the renderer
		_meshRenderer = GetComponent<MeshRenderer>();
		switch (_type) {
			case Type.None:
				break;

			case Type.Hazard:
				_meshRenderer.material = _hazardMaterial;
				break;

			case Type.Platform:
				_meshRenderer.material = _platformMaterial;
				break;
		}
	}

	private void Update () {
		_span = Math.Min(_span, _resolution);

		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			_meshFilter.sharedMesh = GenerateMesh();
		}
		#endif
	}

	private Mesh GenerateMesh () {
		var indices = new List<int>();
		var positions = new List<Vector3>();

		// iterate through each segment
		positions.AddRange(GeneratePositionsForSlice(0));
		indices.AddRange(GenerateCapIndices(_span));
		for (int i = 0; i < _span; ++i) {
			positions.AddRange(GeneratePositionsForSlice(i + 1));
			indices.AddRange(GenerateIndicesForSlice(i, _span));
		}

		// generate a new mesh
		var mesh = new Mesh();
		mesh.name = "Ring";
		mesh.vertices = positions.ToArray();
		mesh.triangles = indices.ToArray();
		return mesh;
	}

	private Vector3[] GeneratePositionsForSlice (int index) {
		float slice = (Mathf.PI * 2.0f) / _resolution;
		float angle = (float)index * slice;
		var unitPos = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
		var heightOffset = new Vector3(0.0f, -_height, 0.0f);

		return new Vector3[] {
			unitPos * _radius,
			unitPos * (_radius + _thickness),
			(unitPos * (_radius + _thickness)) + heightOffset,
			(unitPos * _radius) + heightOffset,
		};
	}

	private int[] GenerateIndicesForSlice (int index, int span) {
		int offset = index * 4;
		return new int[] {
			// top indices
			offset + 0,
			offset + 4,
			offset + 1,
			offset + 4,
			offset + 5,
			offset + 1,
			// bottom indices
			offset + 3,
			offset + 2,
			offset + 6,
			offset + 3,
			offset + 6,
			offset + 7,
			// top indices
			offset + 1,
			offset + 5,
			offset + 6,
			offset + 1,
			offset + 6,
			offset + 2,
			// top indices
			offset + 4,
			offset + 0,
			offset + 3,
			offset + 4,
			offset + 3,
			offset + 7,
		};
	}

	private int[] GenerateCapIndices(int span) {
		int endOffset = span * 4;
		return new int[] {
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
	}
	#endregion
}
