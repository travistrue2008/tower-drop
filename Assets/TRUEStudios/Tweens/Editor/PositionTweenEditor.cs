/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TRUEStudios.Tweens {
	[CustomEditor(typeof(PositionTween)), CanEditMultipleObjects]
	public class PositionTweenEditor : TweenEditor<PositionTween> {
		#region Fields
		private SerializedProperty _coordinateSpaceProperty;
		#endregion

		#region Methods
		protected override void OnEnable () {
			base.OnEnable();
			_coordinateSpaceProperty = serializedObject.FindProperty("_transformSpace");
		}

		protected override void DrawAdditionalFields () {
			EditorGUILayout.PropertyField(_coordinateSpaceProperty);
		}
		#endregion
	}
}
