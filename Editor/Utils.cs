
using System.Collections.Generic;

using P = Hananoki.ManifestJsonUtility.SettingsProject;

namespace Hananoki.ManifestJsonUtility {

	public static class Utils {

		public static List<PackageItem> s_addItems;
		public static List<PackageItem> s_removeItems;

		static void CreateList() {
			if( s_addItems == null ) {
				s_addItems = new List<PackageItem>();
			}
			if( s_removeItems == null ) {
				s_removeItems = new List<PackageItem>();
			}
		}


		public static void ClearItems() {
			CreateList();
			s_removeItems.Clear();
		}


		public static void PushInstallItem( PackageItem item ) {
			CreateList();
			s_addItems.Add( item );
		}

		public static PackageItem PopInstallItem( PackageItem item ) {
			CreateList();

			var it = s_addItems.Find( x => x.name == item.name );
			if( it != null ) {
				//Debug.Log( $"PopInstallItem: {it.displayName}" );
				s_addItems.Remove( it );
				return it;
			}
			return null;
		}


		public static void PushUninstall( PackageItem item ) {
			CreateList();
			s_removeItems.Add( item );
		}

		public static PackageItem PopUninstallList( PackageItem item ) {
			CreateList();

			var it = s_removeItems.Find( x => x.name == item.name );
			if( it != null ) {
				//Debug.Log( $"PopUninstallList: {it.displayName}" );
				s_removeItems.Remove( it );
				return it;
			}
			return null;
		}



		public static void ApplyModifyList() {
			P.Load();
			foreach( var p in s_addItems ) {
				//if( !p.version.StartsWith( "file:" ) ) {
				//	P.Add( p );
				//}
				P.Remove( p );
			}
			foreach( var p in s_removeItems ) {
				// ローカルパッケージはディレクトリ指定の方で。
				if( !p.version.StartsWith( "file:" ) ) {
					P.Add( p );
				}
			}
		}



		public static PackageInfo GetPackageInfo( string packageName ) {
			var info = new PackageInfo();
			if( BuiltinPackage.GetDisplayName( out info.displayName, packageName ) ) {
				info.icon = EditorIcon.icons_processed_prefab_icon_asset;
				info.mode = PackageMode.Builtin;
				return info;
			}
			if( PackageUser.GetDisplayName( out info.displayName, packageName ) ) {
				info.icon = EditorIcon.icons_processed_unityengine_gameobject_icon_asset;
				return info;
			}
			if( PackageCache.GetDisplayName( out info.displayName, packageName ) ) {
				info.icon = EditorIcon.prematcube;
				info.mode = PackageMode.InProject;
				return info;
			}

			info.displayName = packageName;
			info.icon = EditorIcon.icons_processed_unityengine_networkview_icon_asset;

			return info;
		}
	}
}
