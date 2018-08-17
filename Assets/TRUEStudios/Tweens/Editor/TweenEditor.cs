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
	[CustomEditor(typeof(Tween)), CanEditMultipleObjects]
	public class TweenEditor<T> : Editor where T : Tween {
		#region Fields
		private SerializedProperty _awakeTargetProperty;
		private SerializedProperty _targetProperty;

		private SerializedProperty _stateProperty;
		private SerializedProperty _playingForwardProperty;

		private SerializedProperty _loopModeProperty;
		private SerializedProperty _numIterationsProperty;

		private SerializedProperty _durationProperty;
		private SerializedProperty _delayProperty;

		private SerializedProperty _distributionCurveProperty;
		private SerializedProperty _beginProperty;
		private SerializedProperty _endProperty;

		private SerializedProperty _onPlayProperty;
		private SerializedProperty _onFinishProperty;
		private SerializedProperty _onIterateProperty;
		private SerializedProperty _onUpdateProperty;
		#endregion

		#region Properties
		public bool EnableTargetField { protected set; get; }
		public bool ProvideCustomFields { protected set; get; }
		public T Reference { get { return target as T; } }
		public SerializedProperty BeginProperty { get { return _beginProperty; } }
		public SerializedProperty EndProperty { get { return _endProperty; } }
		#endregion

		#region Virtual Methods
		protected virtual void DrawAdditionalFields () { }
		protected virtual void DrawCustomBeginField () { }
		protected virtual void DrawCustomEndField () { }
		#endregion

		#region Editor Hooks
		protected virtual void OnEnable () {
			_awakeTargetProperty = serializedObject.FindProperty("_awakeTarget");
			_targetProperty = serializedObject.FindProperty("_target");

			_stateProperty = serializedObject.FindProperty("_state");
			_playingForwardProperty = serializedObject.FindProperty("_playingForward");

			_loopModeProperty = serializedObject.FindProperty("_loopMode");
			_numIterationsProperty = serializedObject.FindProperty("_numIterations");

			_durationProperty = serializedObject.FindProperty("_duration");
			_delayProperty = serializedObject.FindProperty("_delay");

			_distributionCurveProperty = serializedObject.FindProperty("_distributionCurve");
			_beginProperty = serializedObject.FindProperty("_begin");
			_endProperty = serializedObject.FindProperty("_end");

			_onPlayProperty = serializedObject.FindProperty("_onPlay");
			_onFinishProperty = serializedObject.FindProperty("_onFinish");
			_onIterateProperty = serializedObject.FindProperty("_onIterate");
			_onUpdateProperty = serializedObject.FindProperty("_onUpdate");
		}

		protected virtual void OnDisable () {
			// return to the cached value if not currently playing
			if (!Application.isPlaying) {
				foreach (Object target in targets) {
					(target as T).Factor = 0.0f;
				}
			}
		}

		public override void OnInspectorGUI () {
			// update the serialized object
			serializedObject.Update();
			EditorGUILayout.BeginVertical();

			// draws additional fields defined by subclass
			DrawAdditionalFields();
			EditorGUILayout.Space();

			// draw properties
			DrawTargetProperties();
			DrawStateProperties();
			DrawLoopProperties();
			DrawDurationProperties();
			DrawAnimationProperties();
			DrawResetButtons();
			DrawEventProperties();

			EditorGUILayout.EndVertical();
			serializedObject.ApplyModifiedProperties();
		}
		#endregion

		#region Draw Methods
		private void DrawTargetProperties ()
		{
			// set the initial target (begin or end), and target GameObject if available
			EditorGUILayout.PropertyField(_awakeTargetProperty);
			if (EnableTargetField) {
				EditorGUILayout.PropertyField(_targetProperty);
			}
		}

		private void DrawStateProperties () {
			// display state
			EditorGUILayout.PropertyField(_stateProperty);
			EditorGUILayout.PropertyField(_playingForwardProperty);
			EditorGUILayout.Space();
		}

		private void DrawLoopProperties () {
			// display loop
			EditorGUILayout.PropertyField(_loopModeProperty);
			EditorGUILayout.PropertyField(_numIterationsProperty);
			EditorGUILayout.Space();
		}

		private void DrawDurationProperties () {
			// display duration and delay
			EditorGUILayout.PropertyField(_durationProperty);
			EditorGUILayout.PropertyField(_delayProperty);
			EditorGUILayout.Space();
		}

		private void DrawAnimationProperties () {
			// check if the subclass should draw the begin and end fields
			if (ProvideCustomFields) {
				DrawCustomBeginField();
				DrawCustomEndField();
			} else {
				// draw default fields
				EditorGUILayout.PropertyField(BeginProperty);
				EditorGUILayout.PropertyField(EndProperty);
			}

			// display the distribution curve, and interpolation
			EditorGUILayout.PropertyField(_distributionCurveProperty);
			float factor = EditorGUILayout.Slider("Interpolation", Reference.Factor, 0.0f, 1.0f);
			if (factor != Reference.Factor) {
				// only update Factor, if a change has occurred
				foreach (Object target in targets) {
					(target as T).Factor = factor;
				}
			}

			Reference.ApplyResult();
		}

		private void DrawResetButtons () {
			// convenience buttons
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("To Begin")) {
				foreach (Object target in targets) {
					(target as T).ResetToBegin();
				}
			}

			if (GUILayout.Button("To End")) {
				foreach (Object target in targets) {
					(target as T).ResetToEnd();
				}
			}

			// check if the swap button was pressed
			EditorGUI.BeginChangeCheck();
			GUILayout.Button("Swap");
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObjects(targets, "Swap Begin and End");
				foreach (Object target in targets) {
					(target as T).Swap();
				}
			}
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		private void DrawEventProperties () {
			// display UnityEvent properties
			EditorGUILayout.PropertyField(_onPlayProperty);
			EditorGUILayout.PropertyField(_onFinishProperty);
			EditorGUILayout.PropertyField(_onIterateProperty);
			EditorGUILayout.PropertyField(_onUpdateProperty);
		}
		#endregion
	}
}
