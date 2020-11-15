
using Hananoki.Extensions;
using UnityEditor;
using UnityEngine;

using SS = Hananoki.SharedModule.S;

namespace Hananoki.ManifestJsonUtility {


	public class ManifestJsonUtilityWindow : HEditorWindow {

		TreeViewL m_treeViewL;
		TreeViewR m_treeViewR;


		[MenuItem( "Window/Hananoki/ManifestJsonUtility" )]
		static void Open() {
			GetWindow<ManifestJsonUtilityWindow>( "ManifestJsonUtility" );
		}


		void Refresh() {
			BuiltinPackage.dic = null;
			PackageCache.dic = null;
			PackageUser.dic = null;

			m_treeViewL = new TreeViewL();
			m_treeViewL.RegisterFiles();
			m_treeViewL.ExpandAll();

			m_treeViewR = new TreeViewR();
			m_treeViewR.RegisterFiles();
			m_treeViewR.ExpandAll();

			Utils.ClearItems();
		}


		void OnEnable() {
			Refresh();

		}


		void DrawLeftPane() {
			var styp = new GUIStyle( "Toolbar" );
			//styp.fixedHeight = EditorGUIUtility.singleLineHeight;

			HGUIScope.Horizontal( styp, _ );
			void _() {
				EditorGUILayout.LabelField( "manifest.json" );
				//GUILayout.FlexibleSpace();
				//GUILayout.Button( "aaa" );
			}
			//var a = GUILayoutUtility.GetLastRect();
			//var rc = a;
			//rc.x = ( a.x + a.width ) - 40;
			//rc.width = 40;
			//rc = rc.AlignCenterH( EditorGUIUtility.singleLineHeight );
			//EditorGUI.DrawRect( rc, new Color( 0, 0, 1, 0.1f ) );
			//GUI.Button( rc, "aaa", "Badge" );
			GUILayout.Box( "", HEditorStyles.treeViweArea, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

			var dropRc = GUILayoutUtility.GetLastRect();

			m_treeViewL.OnGUI( dropRc );
		}



		void DrawRightPane() {
			var styp = new GUIStyle( "Toolbar" );
			//styp.fixedHeight = EditorGUIUtility.singleLineHeight;
			HGUIScope.Horizontal( styp, _ );
			void _() {
				EditorGUILayout.LabelField( S._Installablepackages );
			}

			GUILayout.Box( "", HEditorStyles.treeViweArea, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

			var dropRc = GUILayoutUtility.GetLastRect();
			;
			m_treeViewR.OnGUI( dropRc.AlignR( dropRc.width - 1 ) );
		}



		void DrawToolBar() {
			HGUIScope.Horizontal( EditorStyles.toolbar, _ );
			void _() {
				if( GUILayout.Button( EditorHelper.TempContent( EditorIcon.settings ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) ) ) {
#if ENABLE_HANANOKI_SETTINGS
					SharedModule.SettingsWindow.OpenEditor( Package.name );
#else
						SettingsEditorWindow.Open();
#endif
				}
				if( GUILayout.Button( EditorHelper.TempContent( EditorIcon.refresh ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) ) ) {
					Refresh();
				}
				if( GUILayout.Button( EditorHelper.TempContent( SS._Apply ), EditorStyles.toolbarButton ) ) {
					Utils.ApplyModifyList();
					ManifestJsonUtils.Save();
				}
				GUILayout.FlexibleSpace();
			}
		}


		void OnGUI() {
			using( new IsCompilingDisableScope() ) {
				DrawToolBar();

				GUILayout.BeginHorizontal();
				using( new GUILayout.VerticalScope() ) {
					DrawLeftPane();
				}
				using( new GUILayout.VerticalScope( HEditorStyles.dopesheetBackground, GUILayout.Width( 40 ) ) ) {
					GUILayout.FlexibleSpace();
					using( new EditorGUI.DisabledGroupScope( !m_treeViewL.HasSelection() ) ) {
						if( GUILayout.Button( ">>" ) ) {
							m_treeViewL.UninstallSelectionPackage();
						}
					}
					GUILayout.Space( 16 );
					using( new EditorGUI.DisabledGroupScope( !m_treeViewR.HasSelection() ) ) {
						if( GUILayout.Button( "<<" ) ) {
							m_treeViewR.InstallSelectionPackage();
						}
					}
					GUILayout.FlexibleSpace();
				}
				using( new GUILayout.VerticalScope( HEditorStyles.dopesheetBackground ) ) {
					DrawRightPane();
				}
				GUILayout.EndHorizontal();
			}
		}
	}
}

