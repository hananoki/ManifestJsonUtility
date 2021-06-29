
using HananokiEditor.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

using SS = HananokiEditor.SharedModule.S;

namespace HananokiEditor.ManifestJsonUtility {

	public class TreeViewL : HTreeView<PackageTreeItem> {

		public static TreeViewL instance;


		/////////////////////////////////////////
		public TreeViewL() : base( new TreeViewState() ) {
			showAlternatingRowBackgrounds = true;
			instance = this;
		}



		/////////////////////////////////////////
		public void AddInstallItem( PackageTreeItem item, bool undoFlag = false ) {
			var info = Utils.GetPackageInfo( item.name );
			var it = new PackageTreeItem {
				name = item.name,
				value = item.value,
				displayName = info.displayName,
				id = GetID(),
				icon = info.icon,
				install = !undoFlag,
			};
			m_root.children.Add( it );
			ReloadAndSorting();
		}


		/////////////////////////////////////////
		public void RegisterFiles() {

			InitID();
			MakeRoot();

			ManifestJsonUtils.Load();

			var dic = ManifestJsonUtils.GetDependencies();
			foreach( DictionaryEntry e in dic ) {
				var info = Utils.GetPackageInfo( (string) e.Key );

				var it = new PackageTreeItem {
					name = (string) e.Key,
					value = (string) e.Value,
					displayName = info.displayName,
					id = GetID(),
					icon = info.icon,
					version = info.version,
				};
				m_root.AddChild( it );
			}

			m_root.children = m_root.children.OrderBy( x => ( (PackageTreeItem) x ).name ).ToList();

			ReloadAndSorting();
		}


		/////////////////////////////////////////
		public void ReloadAndSorting() {
			ReloadRoot();
		}



		/////////////////////////////////////////
		protected override void OnContextClickedItem( PackageTreeItem item ) {
			var ev = Event.current;
			var pos = ev.mousePosition;

			var m = new GenericMenu();

			m.AddItem( SS._Uninstall, () => {
				選択パッケージをアンインストール指定する();
			} );

			m.DropDownAtMousePosition();
		}



		/////////////////////////////////////////
		public void 選択パッケージをアンインストール指定する() {
			選択パッケージをアンインストール指定する( GetSelection() );
		}



		/////////////////////////////////////////
		void 選択パッケージをアンインストール指定する( object context ) {
			var ids = (IList<int>) context;
			foreach( var id in ids ) {
				var item = ToItem( id );

				m_root.children.Remove( item );
				ManifestJsonUtils.RemovePackage( item.name );

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



		/////////////////////////////////////////
		protected override void OnRowGUI( PackageTreeItem item, RowGUIArgs args ) {

			Label( args, args.rowRect, $"{item.displayName}", item.icon );

			if( item.install ) {
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

