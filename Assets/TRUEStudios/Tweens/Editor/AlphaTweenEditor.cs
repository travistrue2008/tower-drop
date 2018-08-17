/******************************************************************************
 * Foundation Framework
 * Created by: Travis J True, 2016
 * This framework is free to use with no limitations.
******************************************************************************/

using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TRUEStudios.Tweens {
	[CustomEditor(typeof(AlphaTween)), CanEditMultipleObjects]
	public class AlphaTweenEditor : TweenEditor<AlphaTween> {
		#region Fields
		private SerializedProperty _graphicProperty;
		private SerializedProperty _spriteRendererProperty;
		#endregion

		#region Methods
		protected override void OnEnable () {
			base.OnEnable();
			ProvideCustomFields = true;
			_graphicProperty = serializedObject.FindProperty("_graphic");
			_spriteRendererProperty = serializedObject.FindProperty("_spriteRenderer");
		}

		protected override void DrawAdditionalFields () {
			EditorGUILayout.PropertyField(_graphicProperty);
			EditorGUILayout.PropertyField(_spriteRendererProperty);
		}

		protected override void DrawCustomBeginField () {
			EditorGUILayout.Slider(BeginProperty, 0.0f, 1.0f);
		}

		protected override void DrawCustomEndField () {
			EditorGUILayout.Slider(EndProperty, 0.0f, 1.0f);
		}
		#endregion
	}
}
