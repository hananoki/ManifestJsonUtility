
using Hananoki.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using E = Hananoki.ManifestJsonUtility.SettingsEditor;
using P = Hananoki.ManifestJsonUtility.SettingsProject;
using SS = Hananoki.SharedModule.S;

namespace Hananoki.ManifestJsonUtility {

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
				version = item.version,
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

			var dic = ManifestJson.GetDependencies();
			foreach( var p in P.i.m_data ) {
				var info = Utils.GetPackageInfo( p.name );
				var it = new PackageItem {
					name = p.name,
					version = p.version,
					displayName = p.displayName,
					id = GetID(),
					icon = info.icon,
				};
				m_registerItems.Add( it );
			}

			ManifestJson.Load();

			var mfdic = ManifestJson.GetDependencies();
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
						version = "file:" + fname.DirectoryName().Replace( '\\', '/' ),
						displayName = info.displayName,
						id = GetID(),
						icon = info.icon,
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



		protected override void ContextClickedItem( int id ) {
			base.ContextClickedItem( id );
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
				ManifestJson.InstallPackage( item.name, item.version );

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
		}
	}
}

