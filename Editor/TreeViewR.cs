
using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using E = HananokiEditor.ManifestJsonUtility.SettingsEditor;
using P = HananokiEditor.ManifestJsonUtility.SettingsProject;
using SS = HananokiEditor.SharedModule.S;

namespace HananokiEditor.ManifestJsonUtility {

	public class TreeViewR : HTreeView<PackageTreeItem> {

		public static TreeViewR instance;


		/////////////////////////////////////////
		public TreeViewR() : base( new TreeViewState() ) {
			showAlternatingRowBackgrounds = true;
			instance = this;
		}


		/////////////////////////////////////////
		public void AddUninstallItem( PackageTreeItem item, bool undoFlag = false ) {
			var info = Utils.GetPackageInfo( item.name );
			var it = new PackageTreeItem {
				name = item.name,
				value = item.value,
				displayName = info.displayName,
				id = GetID(),
				icon = info.icon,
				uninstall = !undoFlag,
			};

			m_root.AddChild( it );
			ReloadAndSorting();
		}


		/////////////////////////////////////////
		public void LoadDefault() {
			foreach( var p in P.i.m_data ) {
				var info = Utils.GetPackageInfo( p.name );
				var it = new PackageTreeItem {
					name = p.name,
					value = p.version,
					displayName = p.displayName,
					id = GetID(),
					icon = info.icon,
					version = p.version.StartsWith( "http" ) ? "URL" : p.version,
				};
				m_root.AddChild( it );
				//Debug.Log( it.displayName );
			}

			ManifestJsonUtils.Load();

			var mfdic = ManifestJsonUtils.GetDependencies();
			E.Load();
			foreach( var path in E.i.m_dirList ) {
				if( path.IsEmpty() ) continue;
				var files = DirectoryUtils.GetFiles( path, "package.json", System.IO.SearchOption.AllDirectories );
				foreach( var fname in files ) {
					//Debug.Log( sss );
					var packageJson = PackageJson.Load( fname );

					if( mfdic.Contains( packageJson.name ) ) continue;

					var info = Utils.GetPackageInfo( packageJson.name );
					var it = new PackageTreeItem {
						name = packageJson.name,
						value = "file:" + fname.DirectoryName().Replace( '\\', '/' ),
						displayName = info.displayName,
						id = GetID(),
						icon = info.icon,
						version = packageJson.version,
						//localPackage = true,
					};
					m_root.AddChild( it );
				}
			}
		}



		/////////////////////////////////////////
		public void AddUnityPackages() {
			ManifestJsonUtils.Load();
			var mfdic = ManifestJsonUtils.GetDependencies();

			var path = "6fbeb6e9c013add41b6670a9d288955b".ToAssetPath();
			foreach( var s in fs.ReadAllText( path ).Split( '\n' ) ) {
				if( s.IsEmpty() ) continue;

				var ss = s.Split( '\t' );
				var info = Utils.GetPackageInfo( ss[ 0 ] );

				m_root.AddChild( new PackageTreeItem {
					name = ss[ 0 ],
					value = ss[ 1 ],
					displayName = ss[ 0 ],
					id = GetID(),
					icon = info.icon,
					version = mfdic.Contains( ss[ 1 ] ) ? "インストール済" : ss[ 1 ],
					installType = InstallType.データベースに直インストール,
				} );
			}
			m_root.children = m_root.children.OrderBy( x => x ).ToList();
		}



		/////////////////////////////////////////
		public void RegisterFiles( InstallType mode ) {
			P.Load();

			InitID();
			MakeRoot();
			if( mode == InstallType.通常 ) LoadDefault();
			else AddUnityPackages();
			//m_registerItems = m_registerItems.Distinct( x => x.name ).ToList();
			ReloadAndSorting();
		}


		/////////////////////////////////////////
		public void ReloadAndSorting() {
			ReloadRoot();
		}


		/////////////////////////////////////////
		new public bool HasSelection() {
			if( !base.HasSelection() ) return false;
			if( currentItem.install ) return false;
			return true;
		}


		/////////////////////////////////////////
		protected override void OnContextClickedItem( int id ) {
			var ev = Event.current;
			var pos = ev.mousePosition;

			var m = new GenericMenu();

			m.AddItem( SS._Install, () => {
				//選択パッケージをインストール指定する();
				PackageDatabaseUtils.InstallFromUrl( ToItem( id ).value );
			} );

			m.DropDownAtMousePosition();
		}



		/////////////////////////////////////////
		public void 選択パッケージをインストール指定する() {
			選択パッケージをインストール指定する( GetSelection() );
		}



		/////////////////////////////////////////
		void 選択パッケージをインストール指定する( IList<int> ids ) {

			foreach( var id in ids ) {
				var item = FindItem( id );

				m_root.children.Remove( item );
				if( item.installType == InstallType.通常 ) {
					ManifestJsonUtils.AddPackage( item.name, item.value );
				}
				var undo = Utils.PopUninstallList( item );
				if( undo != null ) {
					TreeViewL.instance.AddInstallItem( undo, true );
					continue;
				}

				Utils.PushInstallItem( item );
				TreeViewL.instance.AddInstallItem( item );
			}
			ReloadAndSorting();
			SetSelectionNone();
		}



		/////////////////////////////////////////
		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (PackageTreeItem) args.item;

			ScopeDisable.Begin( item.install );
			Label( args, args.rowRect, $"{item.displayName}", item.icon );
			ScopeDisable.End();

			if( item.uninstall ) {
				GUI.DrawTexture( args.rowRect.AlignR( 16 ), EditorIcon.warning );
			}

			var lrc = args.rowRect;
			var cont = EditorHelper.TempContent( $"{item.version}" );
			var size = HEditorStyles.versionLabel.CalcSize( cont );
			lrc = lrc.AlignR( size.x );
			lrc.x -= 4;
			lrc = lrc.AlignCenterH( 12 );
			EditorGUI.DrawRect( lrc, SharedModule.SettingsEditor.i.versionBackColor );
			HEditorStyles.versionLabel.normal.textColor = SharedModule.SettingsEditor.i.versionTextColor;
			GUI.Label( lrc, $"{item.version}", HEditorStyles.versionLabel );
		}
	}
}

