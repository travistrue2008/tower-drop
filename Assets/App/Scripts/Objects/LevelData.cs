using System;
using System.Collections.Generic;
using UnityEngine;

namespace TRUEStudios.TowerDrop {
	[Serializable]
	public class LevelData : ScriptableObject {
		#region Fields
		[SerializeField]
		private List<RingData> _rings = new List<RingData>();
		#endregion

		#region Properties
		public List<RingData> Rings { get { return _rings; } }
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
		#endregion

		#region Actions
		public void Constrain (SegmentMeshProfile profile) {
			int totalOffset = 0;

			foreach (var segment in Segments) {
				// clamp the offset if not enough space
				if (totalOffset + segment.Space > profile.Resolution) {
					segment.Offset = Mathf.Clamp(profile.Resolution - totalOffset, 0, segment.Offset);
				}

				// clamp the span if not enough space
				if (totalOffset + segment.Space > profile.Resolution) {
					segment.Span = Mathf.Clamp(profile.Resolution - totalOffset - segment.Offset, 0, segment.Span);
				}

				// clamp the span if not enough space
				if (totalOffset + segment.Space > profile.Resolution) {
					throw new Exception($"Segment has taken up too much space: {segment.Offset} + {segment.Span} = {segment.Space}");
				}

				totalOffset += segment.Space;
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
		#endregion

		#region Properties
		public int Space { get { return Span + Offset; } }

		public int Offset {
			set { _offset = Mathf.Max(0, value); }
			get { return _offset; }
		}

		public int Span {
			set { _span = Mathf.Max(0, value); }
			get { return _span; }
		}
		#endregion
	}
}
