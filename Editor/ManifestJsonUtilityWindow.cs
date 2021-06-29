using HananokiEditor.Extensions;
using HananokiRuntime;
using UnityEditor;
using UnityEngine;
using SS = HananokiEditor.SharedModule.S;


namespace HananokiEditor.ManifestJsonUtility {

	public class ManifestJsonUtilityWindow : HEditorWindow {

		TreeViewL m_treeViewL;
		TreeViewR m_treeViewR;

		InstallType m_installType;

		[MenuItem( "Window/Hananoki/" + Package.nameNicify, false, 'M' * 10 )]
		static void Open() {
			GetWindow<ManifestJsonUtilityWindow>();
		}


		/////////////////////////////////////////
		void Refresh() {
			BuiltinPackage.dic = null;
			PackageCache.dic = null;
			PackageUser.dic = null;

			Helper.New( ref m_treeViewL );
			m_treeViewL.RegisterFiles();
			m_treeViewL.ExpandAll();

			Helper.New( ref m_treeViewR );
			m_treeViewR.RegisterFiles( m_installType );
			m_treeViewR.ExpandAll();

			Utils.ClearItems();
		}



		/////////////////////////////////////////
		void OnEnable() {
			SetTitle( Package.nameNicify );
			Refresh();
		}



		/////////////////////////////////////////
		void DrawToolBar() {
			HGUIToolbar.Begin();
			if( HGUIToolbar.Button( EditorHelper.TempContent( EditorIcon.settings ) ) ) {
				SharedModule.SettingsWindow.OpenEditor( Package.nameNicify );
			}

			if( HGUIToolbar.Button( EditorHelper.TempContent( EditorIcon.refresh ) ) ) {
				Refresh();
			}

			GUILayout.FlexibleSpace();
			HGUIToolbar.End();
		}



		/////////////////////////////////////////
		void DrawLeftPane() {
			ScopeHorizontal.Begin( EditorStyles.toolbar );
			EditorGUILayout.LabelField( "manifest.json" );
			GUILayout.FlexibleSpace();
			if( HGUIToolbar.Button( EditorHelper.TempContent( SS._Apply ) ) ) {
				Utils.ApplyModifyList();
				ManifestJsonUtils.Save();
			}
			ScopeHorizontal.End();
			m_treeViewL.DrawLayoutGUI();
		}



		/////////////////////////////////////////
		void DrawRightPane() {
			HGUIToolbar.Begin();
			if( HGUIToolbar.Toggle( m_installType == InstallType.通常, S._Installablepackages ) ) {
				m_installType = InstallType.通常;
				m_treeViewR.RegisterFiles( m_installType );
			}
			if( HGUIToolbar.Toggle( m_installType == InstallType.データベースに直インストール, S._Unitypackage ) ) {
				m_installType = InstallType.データベースに直インストール;
				m_treeViewR.RegisterFiles( m_installType );
			}
			HGUIToolbar.End();

			GUILayout.Box( "", HEditorStyles.treeViweArea, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

			var dropRc = GUILayoutUtility.GetLastRect();

			m_treeViewR.OnGUI( dropRc.AlignR( dropRc.width - 1 ) );
		}





		/////////////////////////////////////////
		public override void OnDefaultGUI() {
			ScopeIsCompile.Begin();

			DrawToolBar();

			var ww = ( position.width - 40 ) * 0.5f;
			ScopeHorizontal.Begin();

			/////////////////////////////////////////
			ScopeVertical.Begin( GUILayout.Width( ww ) );
			DrawLeftPane();
			ScopeVertical.End();

			/////////////////////////////////////////
			ScopeVertical.Begin( HEditorStyles.dopesheetBackground, GUILayout.Width( 40 ) );
			GUILayout.FlexibleSpace();

			ScopeDisable.Begin( !m_treeViewL.HasSelection() );
			if( GUILayout.Button( ">>" ) ) {
				m_treeViewL.選択パッケージをアンインストール指定する();
			}
			ScopeDisable.End();

			GUILayout.Space( 16 );
			ScopeDisable.Begin( !m_treeViewR.HasSelection() );
			if( GUILayout.Button( "<<" ) ) {
				m_treeViewR.選択パッケージをインストール指定する();
			}
			ScopeDisable.End();

			GUILayout.FlexibleSpace();
			ScopeVertical.End();

			/////////////////////////////////////////
			ScopeVertical.Begin( HEditorStyles.dopesheetBackground, GUILayout.Width( ww ) );
			DrawRightPane();
			ScopeVertical.End();

			ScopeHorizontal.End();
			ScopeIsCompile.End();
		}
	}
}

