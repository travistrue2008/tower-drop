using System;
using System.Collections.Generic;
using UnityEngine;

namespace TRUEStudios.TowerDrop {
	[Serializable]
	public class LevelData : ScriptableObject {
		#region Fields
		public SegmentMeshProfile MeshProfile;

		[SerializeField]
		private int _levelNum = 1;
		[SerializeField]
		private float _ringSpacing = 2.0f;
		[SerializeField]
		private List<RingData> _rings = new List<RingData>();

		private HashSet<int> _meshIndices = new HashSet<int>();
		#endregion

		#region Properties
		public List<RingData> Rings { get { return _rings; } }

		public int LevelNum {
			set { _levelNum = Mathf.Max(1, value); }
			get { return _levelNum; }
		}

		public float RingSpacing {
			set { _ringSpacing = Mathf.Max(0.0f, value); }
			get { return _ringSpacing; }
		}
		#endregion

		#region Action
		public void Refresh () {
			_meshIndices.Clear();

			foreach (var ring in Rings) {
				foreach (var segment in ring.Segments) {
					_meshIndices.Add(segment.Span - 1);
				}
			}
		}
		#endregion
	}

	[Serializable]
	public class RingData {
		#region Fields
		[SerializeField]
		private List<SegmentData> _segments = new List<SegmentData>();
		#endregion

		#region Properties
		public List<SegmentData> Segments { get { return _segments; } }

		public int Space {
			get {
				int space = 0;
				foreach (var segment in Segments) {
					space += segment.Space;
				}

				return space;
			}
		}
		#endregion
	}

	[Serializable]
	public class SegmentData {
		#region Fields
		[SerializeField]
		private int _offset = 0;
		[SerializeField]
		private int _span = 1;

		private SegmentMeshProfile _meshProfile;
		private GameObject _refObj;
		#endregion

		#region Properties
		public int Space { get { return Span + Offset; } }

		public int Offset {
			set {
				_offset = Mathf.Max(0, value);
				Refresh();
			}

			get { return _offset; }
		}

		public int Span {
			set {
				_span = Mathf.Max(1, value);
				Refresh();
			}

			get { return _span; }
		}
		#endregion

		#region Actions
		public void Set (GameObject obj, SegmentMeshProfile meshProfile) {
			_refObj = obj;
			_meshProfile = meshProfile;
			Refresh();
		}

		public void Refresh () {
		}
		#endregion
	}
}
