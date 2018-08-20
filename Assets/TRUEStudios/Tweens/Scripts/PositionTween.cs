/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using UnityEngine;

namespace TRUEStudios.Tweens {
	public class PositionTween : Tween<Vector3> {
		public enum CoordinateSpace {
			Local,
			Global,
		}
		
		#region Fields
		[SerializeField]
		private CoordinateSpace _transformSpace = CoordinateSpace.Local;
		#endregion

		#region Properties
		public CoordinateSpace TransformSpace {
			set { _transformSpace = value; }
			get { return _transformSpace; }
		}
		#endregion

		#region Methods
		public override void ApplyResult () {
			// handle based on transform space
			_result = ((_end - _begin) * DistributedValue) + _begin;
			switch (_transformSpace) {
				case CoordinateSpace.Local:
					TargetTransform.localPosition = _result;
					break;

				case CoordinateSpace.Global:
					TargetTransform.position = _result;
					break;
			}
		}
		#endregion
	}
}
