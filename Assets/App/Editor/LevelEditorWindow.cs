using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TRUEStudios.Foundation.Tweens;
using TRUEStudios.TowerDrop;

namespace TRUEStudios.TowerDropEditor {
	public class LevelEditorWindow : EditorWindow {
		#region Constants
		public const int LayerIndex = 8;
		public const float InspectorWidth = 228.0f;
		public const float SegmentLabelWidth = 40.0f;
		public const float SegmentFieldWidth = 72.0f;
		public const float TowerOffset = 10.0f;
		public const string CylinderMeshAssetPath = "Assets/App/Prefabs/Meshes/Mesh_Cylinder.prefab";

		public static readonly Vector4 ScenePadding = new Vector4(0.0f, -2.0f, -4.0f, -2.0f); // top, right, bottom, left
		public static readonly Vector2 SceneRectSizeOffset = new Vector2(ScenePadding.y + ScenePadding.w, ScenePadding.x + ScenePadding.z);
		public static readonly Vector2 SceneRectPositionOffset = new Vector2(-ScenePadding.w, -ScenePadding.z);
		public static readonly Color[] SegmentColors = new Color[2] {
			new Color(0.7f, 1.0f, 0.7f), // green
			new Color(1.0f, 0.7f, 0.7f), // red
		};
		#endregion

		#region Fields
		private int _highlightedRingIndex = 0;
		private Vector2 _scrollPosition = Vector2.zero;
		private Camera _camera;
		private PositionTween _cameraTween;
		private Rect _sceneRect = new Rect();
		private LevelData _level;
		private Texture2D _modifyListButtonTexture;
		private GUIStyle _segmentStyle = new GUIStyle();
		private GameObject _rootObj;
		private GameObject _towerObj;
		private List<List<GameObject>> _ringObjs = new List<List<GameObject>>();
		#endregion

		#region Properties
		public float SceneWidth {
			get {
				return position.size.x - InspectorWidth;
			}
		}

		public Texture2D ModifyListButtonTexture {
			get {
				// generate the content if not already allocated
				if (_modifyListButtonTexture == null) {
					_modifyListButtonTexture = (Texture2D)EditorGUIUtility.Load("icons/_Popup.png");
				}

				return _modifyListButtonTexture;
			}
		}

		public Texture2D DropdownTexture {
			get {
				return EditorGUIUtility.FindTexture("UnityEditor.SceneHierarchyWindow");
			}
		}
		#endregion

		#region MenuItems
		[MenuItem("Window/Game/Level Editor")]
		public static void Open() {
			EditorWindow.GetWindow<LevelEditorWindow>(false, "Level Editor");
		}
		#endregion

		#region EditorWindow Hooks
		private void Awake () {
			// autoRepaintOnSceneChange = true;
			_segmentStyle.normal.background = new Texture2D(1, 1);
			_level = CreateInstance<LevelData>();

			// TEMPORARY...
			for (int i = 0; i < 5; ++i) {
				var ring = new RingData();
				_level.Rings.Add(ring);

				for (int e = 0; e < 3; ++e) {
					ring.Segments.Add(new SegmentData());
				}
			}

			// setup the scene
			CreateCamera();
			CreateSceneRoot();
		}

		private void OnDestroy () {
			DestroyImmediate(_camera.gameObject);
		}

		private void OnGUI() {
			// only render during the repaint event
			if (Event.current.type == EventType.Repaint) {
				DrawSceneRect();
			}

			using (new EditorGUILayout.HorizontalScope()) {
				GUILayout.Space(SceneWidth);
				DrawInspectorRect();
			}
		}

		private void OnInspectorUpdate() {
			Debug.Log("OnInspectorUpdate()");
			Repaint();
		}
		#endregion

		#region Draw Methods
		private void DrawSceneRect () {
			_sceneRect.width = SceneWidth + SceneRectSizeOffset.x;
			_sceneRect.height = position.height + SceneRectSizeOffset.y;
			_sceneRect.position = SceneRectPositionOffset;
			_camera.pixelRect = _sceneRect;
			_camera.Render();
		}

		private void DrawInspectorRect () {
			using (new EditorGUILayout.VerticalScope()) {
				DrawHeader();

				if (_level.MeshProfile != null) {
					using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition)) {
						_scrollPosition = scrollView.scrollPosition;
						DrawRingList();
					}
				} else {
					bool enabled = GUI.enabled;
					GUI.enabled = false;
					EditorGUILayout.LabelField("No SegmentMeshProfile set");
					GUI.enabled = enabled;
				}
			}
		}

		private void DrawHeader () {
			float oldRingSpacing = _level.RingSpacing;
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 80.0f;

			_level.LevelNum = EditorGUILayout.IntField("Level", _level.LevelNum);
			_level.RingSpacing = EditorGUILayout.FloatField("Spacing", oldRingSpacing);
			_level.MeshProfile = (SegmentMeshProfile)EditorGUILayout.ObjectField("Mesh Profile", _level.MeshProfile, typeof(SegmentMeshProfile), false);

			EditorGUIUtility.labelWidth = labelWidth;

			// rescale when spacing changes
			if (oldRingSpacing != _level.RingSpacing) {
				RescaleTower();
			}

			using (new EditorGUILayout.HorizontalScope()) {
				// draw the save button
				if (GUILayout.Button("Save")) {
					Debug.Log("Saving...");
				}

				// draw the save button
				if (GUILayout.Button("Export")) {
					Debug.Log("Exporting...");
				}
			}

			EditorGUILayout.Space();
		}

		private void DrawRingList () {
			EditorGUILayout.LabelField("Rings");
			GUI.enabled = (_level.MeshProfile != null);
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = SegmentLabelWidth;

			// draw each item in the list
			for (int i = 0; i < _level.Rings.Count; ++i) {
				DrawRing(i);
			}

			if (GUILayout.Button("Add Ring")) {
				_level.Rings.Add(new RingData());
			}

			EditorGUIUtility.labelWidth = labelWidth;
			GUI.enabled = true;
		}

		private void DrawRing (int index) {
			var ring = _level.Rings[index];

			using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
				using (new EditorGUILayout.HorizontalScope()) {
					EditorGUILayout.LabelField($"Ring {index + 1}");
					if (GUILayout.Button("Move To")) {
						MoveCamera(index);
					}

					GUILayout.FlexibleSpace();
					DrawModifyRingListMenu(index);
				}

				DrawSegmentList(ring);

				// draw a button
				if (GUILayout.Button("Add Segment")) {
					ring.Segments.Add(new SegmentData());
				}
			}
		}

		private void DrawSegmentList (RingData ring) {
			var backgroundColor = GUI.backgroundColor;

			using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
				for (int i = 0; i < ring.Segments.Count; ++i) {
					GUI.backgroundColor = SegmentColors[i % 2];
					DrawSegment(i, ring);
				}
			}

			GUI.backgroundColor = backgroundColor;
		}

		private void DrawSegment (int index, RingData ring) {
			var segment = ring.Segments[index];

			using (new EditorGUILayout.HorizontalScope(_segmentStyle)) {
				int oldSpace = ring.Space;
				int oldSpan = segment.Span;
				int oldOffset = segment.Offset;

				// update span and offset with UI fields
				segment.Span = EditorGUILayout.IntField("Span", oldSpan, GUILayout.MinWidth(SegmentFieldWidth));
				segment.Offset = EditorGUILayout.IntField("Offset", oldOffset, GUILayout.MinWidth(SegmentFieldWidth));

				// check if the total space is beyond capacity
				if (ring.Space > _level.MeshProfile.Resolution) {
					// cap the span if it was modified
					if (segment.Span != oldSpan) {
						segment.Span = _level.MeshProfile.Resolution - (oldSpace - oldSpan);
					}

					// cap the offset if it was modified
					if (segment.Offset != oldOffset) {
						segment.Offset = _level.MeshProfile.Resolution - (oldSpace - oldOffset);
					}
				}

				// check for changes, and handle them
				if (segment.Span != oldSpan || segment.Offset != oldOffset) {
					_level.Refresh();
				}

				DrawModifyRingListMenu(index);
			}
		}

		private void DrawModifyRingListMenu (int index) {
			var iconButtonStyle = GUI.skin.FindStyle("IconButton");
			var buttonStyle = new GUIContent(ModifyListButtonTexture);

			// draw the button, and generate the context menu if necessary
			if (EditorGUILayout.DropdownButton(buttonStyle, FocusType.Passive, iconButtonStyle)) {
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Remove"), false, () => _level.Rings.RemoveAt(index));

				if (index > 0) {
					menu.AddItem(new GUIContent("Move Up"), false, () => {
						_level.Rings.Insert(index - 1, _level.Rings[index]);
						_level.Rings.RemoveAt(index + 1);
					});
				} else {
					menu.AddDisabledItem(new GUIContent("Move Up"));
				}

				if (index < (_level.Rings.Count - 1)) {
					menu.AddItem(new GUIContent("Move Down"), false, () => {
						_level.Rings.Insert(index + 2, _level.Rings[index]);
						_level.Rings.RemoveAt(index);
					});
				} else {
					menu.AddDisabledItem(new GUIContent("Move Down"));
				}

				menu.AddItem(new GUIContent("Insert Above"), false, () => {
					_level.Rings.Insert(index, new RingData());
					RescaleTower();
				});

				menu.AddItem(new GUIContent("Insert Below"), false, () => {
					_level.Rings.Insert(index + 1, new RingData());
					RescaleTower();
				});

				menu.ShowAsContext();
				RescaleTower();
			}
		}

		private void DrawModifySegmentListMenu (int index, RingData ring) {
			var iconButtonStyle = GUI.skin.FindStyle("IconButton");
			var buttonStyle = new GUIContent(ModifyListButtonTexture);

			// draw the button, and generate the context menu if necessary
			if (EditorGUILayout.DropdownButton(buttonStyle, FocusType.Passive, iconButtonStyle)) {
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Remove"), false, () => ring.Segments.RemoveAt(index));

				if (index > 0) {
					menu.AddItem(new GUIContent("Move Up"), false, () => {
						ring.Segments.Insert(index - 1, ring.Segments[index]);
						ring.Segments.RemoveAt(index + 1);
					});
				} else {
					menu.AddDisabledItem(new GUIContent("Move Up"));
				}

				if (index < (ring.Segments.Count - 1)) {
					menu.AddItem(new GUIContent("Move Down"), false, () => {
						ring.Segments.Insert(index + 2, ring.Segments[index]);
						ring.Segments.RemoveAt(index);
					});
				} else {
					menu.AddDisabledItem(new GUIContent("Move Down"));
				}

				menu.AddItem(new GUIContent("Insert Above"), false, () => ring.Segments.Insert(index, new SegmentData()));
				menu.AddItem(new GUIContent("Insert Below"), false, () => ring.Segments.Insert(index + 1, new SegmentData()));
				menu.ShowAsContext();
				RescaleTower();
			}
		}

		private void DrawHorizontalLine(float height = 1.0f) {
			var rect = EditorGUILayout.GetControlRect(false, height);
			rect.height = height;
			EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f));
		}
		#endregion

		#region Scene Methods
		private GameObject CreateGameObject (string name) {
			var obj = new GameObject(name);
			obj.hideFlags = HideFlags.HideAndDontSave;
			obj.layer = LayerIndex;
			return obj;
		}

		private void CreateCamera () {
			// create the camera's root GameObject, and attach a PositionTween to it
			var rootObj = CreateGameObject("Container_Camera");
			_cameraTween = rootObj.AddComponent<PositionTween>();
			// _cameraTween.Begin = _cameraTween.End = _cameraTween.transform.localPosition;
			_cameraTween.Duration = 2.0f;
			_cameraTween.Begin = new Vector3(0.0f, 20.0f, 0.0f);
			_cameraTween.End = new Vector3(0.0f, -20.0f, 0.0f);
			_cameraTween.LoopMode = Tween.PlaybackMode.Pingpong;
			_cameraTween.Play();

			// create the camera GameObject, attach it to the root, and offset its position and orientation
			var obj = CreateGameObject("Camera_Editor");
			obj.transform.SetParent(rootObj.transform);
			obj.transform.localPosition = new Vector3(0.0f, 1.0f, -3.5f);
			obj.transform.localRotation = Quaternion.Euler(20.0f, 0.0f, 0.0f);

			// add the Camera component
			_camera = obj.AddComponent<Camera>();
			_camera.enabled = false;
			_camera.cullingMask = (1 << LayerIndex);
			_camera.clearFlags = CameraClearFlags.SolidColor;
			_camera.backgroundColor = new Color(0.0f, 0.6f, 1.0f);
		}

		private void CreateSceneRoot () {
			_rootObj = CreateGameObject("Root");

			// instantiate the prefab (we don't want to modify the prefab, so we don't need to link it)
			var towerPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(CylinderMeshAssetPath, typeof(GameObject));
			_towerObj = (GameObject)Instantiate(towerPrefab);
			_towerObj.hideFlags = HideFlags.HideAndDontSave;
			_towerObj.layer = LayerIndex;
			_towerObj.transform.SetParent(_rootObj.transform);
			_towerObj.transform.localPosition = new Vector3(0.0f, TowerOffset, 0.0f);
			RescaleTower();
		}

		private void RescaleTower () {
			int numLinks = Mathf.Max(0, _level.Rings.Count - 1);
			float yScale = (numLinks * _level.RingSpacing) + TowerOffset;
			_towerObj.transform.localScale = new Vector3(1.0f, yScale, 1.0f);
		}

		private void MoveCamera (int index) {
			var position = _cameraTween.Begin;
			position.y = _highlightedRingIndex * _level.RingSpacing;
			_cameraTween.Begin = position;
			Debug.Log($"Begin: {_cameraTween.Begin}    End: {_cameraTween.End}");

			_highlightedRingIndex = index;
			position.y = _highlightedRingIndex * _level.RingSpacing;
			_cameraTween.End = position;
			_cameraTween.PlayForward(true);
		}
		#endregion
	}
}
