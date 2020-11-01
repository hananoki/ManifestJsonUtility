//#define ENABLE_LEGACY_PREFERENCE

using UnityEditor;
using UnityEngine;
using Hananoki.Extensions;
using Hananoki.SharedModule;
using UnityEditorInternal;
using System.Collections.Generic;

using E = Hananoki.ManifestJsonUtility.SettingsEditor;
using SS = Hananoki.SharedModule.S;

namespace Hananoki.ManifestJsonUtility {
	[System.Serializable]
	public class SettingsEditor {

		public static E i;

		public List<string> m_dirList;

		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<E>.Get( Package.editorPrefName );
		}



		public static void Save() {
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}
	}



	public class SettingsEditorWindow : HSettingsEditorWindow {

		static ReorderableList s_rl_searchFilterHierarchy;
		static bool s_changed;

		public static void Open() {
			var w = GetWindow<SettingsEditorWindow>();
			w.SetTitle( new GUIContent( "Project Settings", EditorIcon.settings ) );
			w.headerMame = Package.name;
			w.headerVersion = Package.version;
			w.gui = DrawGUI;
		}


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



		static void DrawCommandTable( ReorderableList r ) {
			EditorGUI.BeginChangeCheck();
			r.DoLayoutList();
			if( EditorGUI.EndChangeCheck() ) {
				s_changed = true;
			}
		}



		public static void DrawGUI() {
			E.Load();
			s_changed = false;
			if( s_rl_searchFilterHierarchy == null ) {
				s_rl_searchFilterHierarchy = MakeRLFromHierarchy();
			}

			EditorGUI.BeginChangeCheck();

			DrawCommandTable( s_rl_searchFilterHierarchy );

			if( EditorGUI.EndChangeCheck() ) {
				s_changed = true;
			}

			if( s_changed ) {
				E.Save();
				EditorApplication.RepaintHierarchyWindow();
			}
		}



#if !ENABLE_HANANOKI_SETTINGS
#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE
		
		[SettingsProvider]
		public static SettingsProvider PreferenceView() {
			var provider = new SettingsProvider( $"Preferences/Hananoki/{Package.name}", SettingsScope.User ) {
				label = $"{Package.name}",
				guiHandler = PreferencesGUI,
				titleBarGuiHandler = () => GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel ),
			};
			return provider;
		}
		public static void PreferencesGUI( string searchText ) {
#else
		[PreferenceItem( Package.name )]
		public static void PreferencesGUI() {
#endif
			using( new LayoutScope() ) DrawGUI();
		}
#endif
	}



#if ENABLE_HANANOKI_SETTINGS
	[SettingsClass]
	public class SettingsEvent {
		[SettingsMethod]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				//mode = 1,
				displayName = Package.name,
				version = Package.version,
				gui = SettingsEditorWindow.DrawGUI,
			};
		}
	}
#endif
}
