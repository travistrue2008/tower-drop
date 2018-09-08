using System;
using UnityEngine;
using UnityEditor;

namespace TRUEStudios.TowerDropEditor {
	[CustomEditor(typeof(SegmentMeshProfile))]
	public class SegmentMeshProfileEditor : Editor {
		#region Fields
		private bool _changed = false;
		#endregion

		#region Properties
		public SegmentMeshProfile Reference { get { return (SegmentMeshProfile)target; } }
		#endregion

		#region Override Methods
		private void OnDisable () {
			if (_changed) {
				Reference.Apply();
			}
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();
			EditorGUILayout.BeginVertical();

			DrawProperties();
			DrawInfoLabels();
			DrawApplyButton();

			EditorGUILayout.EndVertical();
			serializedObject.ApplyModifiedProperties();
		}
		#endregion

		#region Draw Methods
		private void DrawProperties () {
			Reference.Resolution = EditorGUILayout.IntField("Resolution", Reference.Resolution);
			Reference.Radius = EditorGUILayout.FloatField("Radius", Reference.Radius);
			Reference.FallPower = EditorGUILayout.FloatField("FallPower", Reference.FallPower);
			Reference.DestructDuration = EditorGUILayout.FloatField("Destruct Duration", Reference.DestructDuration);
			Reference.Height = EditorGUILayout.FloatField("Height", Reference.Height);
			Reference.PlatformMaterial = (Material)EditorGUILayout.ObjectField("Resolution", Reference.PlatformMaterial, typeof(Material), false);
			Reference.HazardMaterial = (Material)EditorGUILayout.ObjectField("Hazard Material", Reference.HazardMaterial, typeof(Material), false);
			Reference.SlamMaterial = (Material)EditorGUILayout.ObjectField("Slam Material", Reference.SlamMaterial, typeof(Material), false);
			EditorGUILayout.Space();
			_changed |= GUI.changed; // set flag if changes occurred
		}

		private void DrawInfoLabels () {
			int numVertices = 0;

			foreach (var mesh in Reference.Meshes) {
				numVertices += mesh.vertexCount;
			}

			EditorGUILayout.LabelField($"Total Vertices: {numVertices:N0}");
			EditorGUILayout.Space();
		}

		private void DrawApplyButton () {
			// apply changes if necessary
			GUI.enabled = _changed;
			if (GUILayout.Button("Apply")) {
				Reference.Apply();
				_changed = false;
			}
			GUI.enabled = true;
		}
		#endregion
	}
}
