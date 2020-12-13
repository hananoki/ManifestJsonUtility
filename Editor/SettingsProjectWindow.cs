#pragma warning disable 649
#define ENABLE_HANANOKI_SETTINGS

using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using P = HananokiEditor.ManifestJsonUtility.SettingsProject;

namespace HananokiEditor.ManifestJsonUtility {
	public class SettingsProjectWindow : HSettingsEditorWindow {

		static bool s_changed;

		static ReorderableList s_rl_searchFilterHierarchy;

		public static void Open() {
			var w = GetWindow<SettingsProjectWindow>();
			w.SetTitle( new GUIContent( "Project Settings", EditorIcon.settings ) );
			w.headerMame = Package.name;
			w.headerVersion = Package.version;
		}

		void OnEnable() {
			var w = GetWindow<SettingsProjectWindow>();
			w.gui = DrawGUI;
		}

		static void DrawCommandTable( ReorderableList r ) {
			EditorGUI.BeginChangeCheck();
			r.DoLayoutList();
			if( EditorGUI.EndChangeCheck() ) {
				s_changed = true;
			}
		}

		static ReorderableList MakeRLFromHierarchy() {
			var r = new ReorderableList( P.i.m_data, typeof( SettingsProject.Data ) );

			r.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, "Backup Packages" );
			};

			r.onAddCallback = ( rect ) => {
				if( P.i.m_data.Count == 0 ) {
					P.i.m_data.Add( new P.Data() );
				}
				else {
					P.i.m_data.Add( new P.Data( P.i.m_data[ r.count - 1 ] ) );
				}
			};

			r.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				EditorGUI.BeginChangeCheck();
				var p = P.i.m_data[ index ];
				var w = rect.width;
				var x = rect.x;
				rect.y += 1;
				rect.height = EditorGUIUtility.singleLineHeight;
				//rect.width = w * 0.20f;
				//p.searchMode = (USceneHierarchyWindow.SearchMode) EditorGUI.EnumPopup( rect, p.searchMode, "MiniPopup" );

				//rect.x = x + w * 0.20f;
				//rect.width = w * 0.80f;
				 EditorGUI.LabelField( rect, $"{p.name}:    {p.version}" );

			};

			return r;
		}


		public static void DrawGUI() {

			P.Load();
			s_changed = false;

			if( s_rl_searchFilterHierarchy == null ) {
				s_rl_searchFilterHierarchy = MakeRLFromHierarchy();
			}


			DrawCommandTable( s_rl_searchFilterHierarchy );

			

			if( s_changed ) {
				P.Save();
			}
		}




#if !ENABLE_HANANOKI_SETTINGS
#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE
		[SettingsProvider]
		public static SettingsProvider PreferenceView() {
			var provider = new SettingsProvider( $"Hananoki/{Package.name}", SettingsScope.Project ) {
				label = $"{Package.name}",
				guiHandler = PreferencesGUI,
				titleBarGuiHandler = () => GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel ),
			};
			return provider;
		}
		public static void PreferencesGUI( string searchText ) {
			using( new LayoutScope() ) DrawGUI();
		}
#endif
#endif
	}



#if ENABLE_HANANOKI_SETTINGS
	public class SettingsProjectEvent {
		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				mode = 1,
				displayName = Package.nameNicify,
				version = Package.version,
				gui = SettingsProjectWindow.DrawGUI,
			};
		}
	}
#endif
}
