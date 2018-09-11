using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TRUEStudios.TowerDropEditor {
	public class BuiltInStylesWindow : EditorWindow {
		#region Fields
		private string _searchText = string.Empty;
		private Vector2 _scrollPosition = Vector2.zero;
		private GUIStyle[] _searchResults = new GUIStyle[0];
		#endregion

		#region Properties
		public GUIStyle ToolbarStyle {
			get { return GUI.skin.FindStyle("Toolbar"); }
		}

		public GUIStyle SearchBarStyle {
			get { return GUI.skin.FindStyle("ToolbarSeachTextField"); }
		}

		public GUIStyle SearchCancelStyle {
			get { return GUI.skin.FindStyle("ToolbarSeachCancelButton"); }
		}
		#endregion

		#region Methods
		[MenuItem( "Window/Game/Built-In Styles" )] 
		public static void Open() {
			EditorWindow.GetWindow<BuiltInStylesWindow>(false, "Built-In Styles");
		}

		private void OnGUI() {
			using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition)) {
				_scrollPosition = scrollView.scrollPosition;
				DrawSearchBar();
				DrawStyles();
			}
		}

		private void DrawStyles () {
			var styles = !string.IsNullOrWhiteSpace(_searchText) ? _searchResults : GUI.skin.customStyles;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField($"Styles: {styles.Length}", EditorStyles.boldLabel);
			foreach(var style in styles) {
				EditorGUILayout.LabelField(style.name);
			}
		}

		private void DrawSearchBar () {
			using (new EditorGUILayout.HorizontalScope(ToolbarStyle)) {
				string old = _searchText;

				// update the search text, and check for a change
				_searchText = EditorGUILayout.TextField(old, SearchBarStyle);
				if (_searchText != old) {
					_searchResults = Search(_searchText);
				}

				// clear search text if necessary
				if (GUILayout.Button("", SearchCancelStyle)) {
					_searchText = "";
					GUI.FocusControl(null);
				}
			}
		}

		private GUIStyle[] Search (string text) {
			string lowerText = text.ToLower();
			List<GUIStyle> results = new List<GUIStyle>();

			foreach(var style in GUI.skin.customStyles) {
				if (style.name.ToLower().Contains(lowerText)) {
					results.Add(style.name);
				}
			}

			return results.ToArray();
		}
		#endregion
	}
}
