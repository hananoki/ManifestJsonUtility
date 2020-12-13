
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

	public class TreeViewR : HTreeView<PackageItem> {

		public static TreeViewR instance;

		public TreeViewR() : base( new TreeViewState() ) {
			showAlternatingRowBackgrounds = true;
			instance = this;
		}


		public void AddUninstallItem( PackageItem item, bool undoFlag = false ) {
			var info = Utils.GetPackageInfo( item.name );
			var it = new PackageItem {
				name = item.name,
				value = item.value,
				displayName = info.displayName,
				id = GetID(),
				icon = info.icon,
				uninstall = !undoFlag,
			};

			m_registerItems.Add( it );
			ReloadAndSorting();
		}


		public void RegisterFiles() {

			InitID();
			m_registerItems = new List<PackageItem>();

			P.Load();

			var dic = ManifestJsonUtils.GetDependencies();
			foreach( var p in P.i.m_data ) {
				var info = Utils.GetPackageInfo( p.name );
				var it = new PackageItem {
					name = p.name,
					value = p.version,
					displayName = p.displayName,
					id = GetID(),
					icon = info.icon,
					version = p.version.StartsWith( "http" ) ? "URL" : p.version,
				};
				m_registerItems.Add( it );
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
					var it = new PackageItem {
						name = packageJson.name,
						value = "file:" + fname.DirectoryName().Replace( '\\', '/' ),
						displayName = info.displayName,
						id = GetID(),
						icon = info.icon,
						version = packageJson.version,
						//localPackage = true,
					};
					m_registerItems.Add( it );
				}
			}

			m_registerItems = m_registerItems.Distinct( x => x.name ).ToList();
			ReloadAndSorting();
		}


		public void ReloadAndSorting() {
			Reload();
		}



		protected override void OnContextClickedItem( int id ) {
			var ev = Event.current;
			var pos = ev.mousePosition;

			var m = new GenericMenu();

			m.AddItem( SS._Install, () => {
				InstallSelectionPackage();
			} );

			m.DropDown( new Rect( pos.x, pos.y, 1, 1 ) );
			Event.current.Use();
		}


		public void InstallSelectionPackage() {
			InstallSelectionPackage( GetSelection() );
		}


		void InstallSelectionPackage( object context ) {

			var ids = (IList<int>) context;
			foreach( var id in ids ) {
				var item = FindItem( id );

				m_registerItems.Remove( item );
				ManifestJsonUtils.AddPackage( item.name, item.value );

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


		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (PackageItem) args.item;
			var labelStyle = /*args.selected ? EditorStyles.whiteLabel :*/ EditorStyles.label;
			EditorGUI.LabelField( args.rowRect, EditorHelper.TempContent( $"{item.displayName}", item.icon ), labelStyle );

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

