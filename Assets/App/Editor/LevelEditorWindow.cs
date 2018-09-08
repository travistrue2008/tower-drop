using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TRUEStudios.TowerDrop;

namespace TRUEStudios.TowerDropEditor {
	public class LevelEditorWindow : EditorWindow {
		#region Constants
		public const float InspectorMinWidth = 280.0f; // 256px
		public const float InspectorWidthRatio = 0.25f; // 25%
		public const string MenuName = "Level Editor";
		public const string CameraName = "Camera_Editor";
		public const string CameraTag = "EditorCamera";

		public readonly Color[] SegmentColors = new Color[2] {
			new Color(1.0f, 0.7f, 0.7f),
			new Color(0.7f, 1.0f, 0.7f),
		};
		#endregion

		#region Fields
		private int _levelNum = 1;
		private Vector2 _scrollPosition = Vector2.zero;
		private Camera _camera;
		private Rect _sceneRect = new Rect();
		private SegmentMeshProfile _profile;
		private LevelData _level;
		private GUIStyle _segmentStyle = new GUIStyle();
		private bool[] _foldouts;
		#endregion

		#region Properties
		public float SceneWidth {
			get {
				return Mathf.Round(position.size.x) - InspectorWidth;
			}
		}

		public float InspectorWidth {
			get {
				return Mathf.Max(InspectorMinWidth, Mathf.Round(position.size.x * InspectorWidthRatio));
			}
		}

		public Camera EditorCamera {
			get {
				// check if the camera isn't set yet
				if (_camera == null) {
					// attempt to get the GameObject by tag
					var obj = GameObject.FindGameObjectWithTag(CameraTag);
					if (obj == null) {
						// otherwise, create the GameObject
						obj = new GameObject(CameraName);
						obj.hideFlags = HideFlags.HideAndDontSave;
						obj.tag = CameraTag;

						// add the camera component, and configure it
						_camera = obj.AddComponent<Camera>();
						_camera.enabled = false;
						_camera.clearFlags = CameraClearFlags.SolidColor;
						_camera.backgroundColor = new Color(0.0f, 0.6f, 1.0f);
					} else {
						_camera = obj.GetComponent<Camera>();
					}
				}

				
				return _camera;
			}
		}

		public Texture2D DropdownTexture {
			get {
				return EditorGUIUtility.FindTexture("SceneAsset Icon");
			}
		}
		#endregion

		#region MenuItems
		[MenuItem("Window/Game/Level Editor")]
		public static void Init() {
			EditorWindow.GetWindow<LevelEditorWindow>(false, MenuName);
		}
		#endregion

		#region EditorWindow Hooks
		private void OnEnable () {
			autoRepaintOnSceneChange = true;
			_segmentStyle.normal.background = new Texture2D(1, 1);

			_foldouts = new bool[100];
			_level = CreateInstance<LevelData>();
			for (int i = 0; i < 100; ++i) {
				var ring = new RingData();
				_level.Rings.Add(ring);

				for (int e = 0; e < 3; ++e) {
					ring.Segments.Add(new SegmentData());
				}
			}

			Debug.Log($"DropdownTexture: {DropdownTexture != null}");
		}

		private void OnDisable () {
			DestroyImmediate(EditorCamera.gameObject);
		}

		private void OnGUI() {
			// only render during the repaint event
			if (Event.current.type == EventType.Repaint) {
				DrawSceneRect();
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(SceneWidth);
			DrawInspectorRect();
			EditorGUILayout.EndHorizontal();
		}
		#endregion

		#region Draw Methods
		private void DrawSceneRect () {
			_sceneRect.size = position.size;
			_sceneRect.width = SceneWidth;
			EditorCamera.pixelRect = _sceneRect;
			EditorCamera.Render();
		}

		private void DrawInspectorRect () {
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 64.0f;
			EditorGUILayout.BeginVertical();
			DrawHeader();
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			DrawRings();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			EditorGUIUtility.labelWidth = labelWidth;
		}

		private void DrawHeader () {
			_levelNum = EditorGUILayout.IntField("Level", _levelNum);
			_profile = (SegmentMeshProfile)EditorGUILayout.ObjectField("Profile", _profile, typeof(SegmentMeshProfile), false);

			EditorGUILayout.BeginHorizontal();

			// draw the save button
			if (GUILayout.Button("Save")) {
				Debug.Log("Saving...");
			}

			// draw the save button
			if (GUILayout.Button("Export")) {
				Debug.Log("Exporting...");
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			DrawHorizontalLine();
			EditorGUILayout.Space();
		}

		private void DrawRings () {
			GUI.enabled = (_profile != null);

			// draw each item in the list
			for (int i = 0; i < _level.Rings.Count; ++i) {
				// draw the foldout
				_foldouts[i] = EditorGUILayout.Foldout(_foldouts[i], $"Ring {i + 1}");
				if (_foldouts[i]) {
					DrawRing(_level.Rings[i]);
				}

				EditorGUILayout.Space();
			}
			GUI.enabled = true;
		}

		private void DrawRing (RingData ring) {
			++EditorGUI.indentLevel;

			for (int i = 0; i < ring.Segments.Count; ++i) {
				var segment = ring.Segments[i];
				GUI.backgroundColor = SegmentColors[i % 2];
				DrawSegment(segment);
			}

			--EditorGUI.indentLevel;

			ring.Constrain(_profile);
		}

		private void DrawSegment (SegmentData segment) {
			EditorGUILayout.BeginHorizontal(_segmentStyle);
			segment.Span = EditorGUILayout.IntField("Span", segment.Span, GUILayout.Width(96.0f));
			segment.Offset = EditorGUILayout.IntField("Offset", segment.Offset, GUILayout.Width(96.0f));

			// dropdown button
			if (EditorGUILayout.DropdownButton(new GUIContent(DropdownTexture), FocusType.Keyboard, GUILayout.Width(20.0f))) {
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Constant"), false, () => {});
			// 	menu.AddItem(new GUIContent("Variable"), false, () => SetProperty(property, false));
				menu.ShowAsContext();
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawHorizontalLine(float height = 1.0f) {
			var rect = EditorGUILayout.GetControlRect(false, height);
			rect.height = height;
			EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f));
		}
		#endregion

		#region Private Methods
		#endregion
	}
}
