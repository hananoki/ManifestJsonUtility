using HananokiEditor.SharedModule;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using E = HananokiEditor.ManifestJsonUtility.SettingsEditor;


namespace HananokiEditor.ManifestJsonUtility {

	public class SettingsEditorWindow {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = Package.nameNicify,
				version = Package.version,
				gui = DrawGUI,
			};
		}

		static ReorderableList s_reorderableList;
		static bool s_changed;


		/////////////////////////////////////////
		static ReorderableList MakeRLFromHierarchy() {
			if( E.i.m_dirList == null ) {
				E.i.m_dirList = new List<string>();
			}
			var r = new ReorderableList( E.i.m_dirList, typeof( string ) );

			r.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, S._LocalPackagePaths );
			};

			r.onAddCallback = ( rect ) => {
				if( E.i.m_dirList.Count == 0 ) {
					E.i.m_dirList.Add( string.Empty );
				}
				else {
					E.i.m_dirList.Add( E.i.m_dirList[ r.count - 1 ] );
				}
			};

			r.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				EditorGUI.BeginChangeCheck();
				var p = E.i.m_dirList[ index ];
				var w = rect.width;
				var x = rect.x;
				rect.y += 1;
				rect.height = EditorGUIUtility.singleLineHeight;

				EditorGUI.BeginChangeCheck();
				E.i.m_dirList[ index ] = HEditorGUI.FolderFiled( rect, p );
				if( EditorGUI.EndChangeCheck() ) {
					s_changed = true;
				}
			};

			return r;
		}


		/////////////////////////////////////////
		static void DrawCommandTable( ReorderableList r ) {
			EditorGUI.BeginChangeCheck();
			r.DoLayoutList();
			if( EditorGUI.EndChangeCheck() ) {
				s_changed = true;
			}
		}


		/////////////////////////////////////////
		public static void DrawGUI() {
			E.Load();
			s_changed = false;
			if( s_reorderableList == null ) {
				s_reorderableList = MakeRLFromHierarchy();
			}

			EditorGUI.BeginChangeCheck();

			DrawCommandTable( s_reorderableList );

			if( EditorGUI.EndChangeCheck() ) {
				s_changed = true;
			}

			if( s_changed ) {
				E.Save();
				EditorApplication.RepaintHierarchyWindow();
			}
		}
	}
}
