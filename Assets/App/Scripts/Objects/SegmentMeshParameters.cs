using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Segment Mesh Parameters", fileName = "SegmentMeshParameters")]
public class SegmentMeshParameters : ScriptableObject {
	#region Constants
	public const string MeshName = "Surface";
	#endregion

	#region Fields
	public int Resolution = 90;
	public float Radius = 2.0f;
	public float FallPower = 10.0f;
	public float DestructDuration = 1.0f;
	public Material PlatformMaterial;
	public Material HazardMaterial;
	public Material SlamMaterial;

	[SerializeField]
	private float _height = 0.35f;
	[SerializeField, HideInInspector]
	private Vector3 _heightOffset = new Vector3(0.0f, -0.35f, 0.0f);
	#endregion

	#region Properties
	public Vector3 HeightOffset { get { return _heightOffset; } }

	public float Slice {
		get { return (Mathf.PI * 2.0f) / (float)Resolution; }
	}

	public float DegreesPerSlice {
		get { return 360.0f / (float)Resolution; }
	}

	public float Height {
		set {
			_height = Mathf.Max(0.0f, value);
			_heightOffset = new Vector3(0.0f, -_height, 0.0f);
		}

		get { return _height; }
	}
	#endregion
}
