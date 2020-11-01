
using Hananoki.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using SS = Hananoki.SharedModule.S;

namespace Hananoki.ManifestJsonUtility {

	public class TreeViewL : HTreeView<PackageItem> {

		public static TreeViewL instance;

		public TreeViewL() : base( new TreeViewState() ) {
			showAlternatingRowBackgrounds = true;
			instance = this;
		}



		public void AddInstallItem( PackageItem item, bool undoFlag = false ) {
			var info = Utils.GetPackageInfo( item.name );
			var it = new PackageItem {
				name = item.name,
				version = item.version,
				displayName = info.displayName,
				id = GetID(),
				icon = info.icon,
				install = !undoFlag,
			};
			m_registerItems.Add( it );
			ReloadAndSorting();
		}



		public void RegisterFiles() {

			InitID();
			m_registerItems = new List<PackageItem>();

			ManifestJson.Load();

			var dic = ManifestJson.GetDependencies();
			foreach( DictionaryEntry e in dic ) {
				var info = Utils.GetPackageInfo( (string) e.Key );

				var it = new PackageItem {
					name = (string) e.Key,
					version = (string) e.Value,
					displayName = info.displayName,
					id = GetID(),
					icon = info.icon,
					//builtin = bbb,
				};
				m_registerItems.Add( it );
			}

			m_registerItems = m_registerItems.OrderBy( x => x.name ).ToList();

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

			m.AddItem( SS._Uninstall, () => {
				UninstallSelectionPackage();
			} );

			m.DropDown( new Rect( pos.x, pos.y, 1, 1 ) );
			Event.current.Use();
		}


		public void UninstallSelectionPackage() {
			UninstallSelectionPackage( GetSelection() );
		}

		void UninstallSelectionPackage( object context ) {
			var ids = (IList<int>) context;
			foreach( var id in ids ) {
				var item = FindItem( id );

				m_registerItems.Remove( item );
				ManifestJson.UninstallPackage( item.name );

				var undo = Utils.PopInstallItem( item );
				if( undo != null ) {
					TreeViewR.instance.AddUninstallItem( undo, true );
					continue;
				}

				Utils.PushUninstall( item );
				TreeViewR.instance.AddUninstallItem( item );
			}
			ReloadAndSorting();
			SetSelectionNone();
		}



		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (PackageItem) args.item;
			var labelStyle = /*args.selected ? EditorStyles.whiteLabel :*/ EditorStyles.label;

			EditorGUI.LabelField( args.rowRect, EditorHelper.TempContent( $"{item.displayName}", item.icon ), labelStyle );

			if( item.install ) {
				GUI.DrawTexture( args.rowRect.AlignR( 16 ), EditorIcon.warning );
			}
		}


	}

}

