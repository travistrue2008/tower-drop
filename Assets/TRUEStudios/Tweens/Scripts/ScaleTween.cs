/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using UnityEngine;

namespace TRUEStudios.Tweens {
	public class ScaleTween : Tween<Vector3> {
		#region Methods
		#if UNITY_EDITOR
		private ScaleTween () {
			begin = end = Vector3.one;
		}
		#endif

		public override void ApplyResult () {
			_result = ((_end - _begin) * DistributedValue) + _begin;
			TargetTransform.localScale = _result;
		}
		#endregion
	}
}
