using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HananokiRuntime.Extensions;
using HananokiEditor.Extensions;
using P = HananokiEditor.ManifestJsonUtility.SettingsProject;


namespace HananokiEditor.ManifestJsonUtility {

	[InitializeOnLoad]
	public static class Utils {

		public static List<PackageTreeItem> s_addItems;
		public static List<PackageTreeItem> s_removeItems;


		static Utils() {
			ExternalPackages.AddPB( ( string assetPath, string guid, ref Rect selectionRect ) => {
				// Favoriteが必ず一番上に来る前提とするとrect.yは0
				// ここでは適当に１行の高さより大きいかぐらいの意味で判定している
				if( assetPath.IsEmpty() && selectionRect.y < 16 && ExternalPackages.ManifestJsonUtility ) {
					var r = selectionRect.AlignR( 16 );
#if UNITY_2020_1_OR_NEWER
					if( HEditorGUI.IconButton( r, EditorIcon.package_manager ) ) {
						EditorApplication.ExecuteMenuItem( "Window/Package Manager" );
					}
					r.x -= 16;
#endif
					if( HEditorGUI.IconButton( r, EditorIcon.icons_processed_unityengine_textasset_icon_asset ) ) {
						EditorApplication.ExecuteMenuItem( "Window/Hananoki/Manifest Json Utility" );
					}
					selectionRect.width -= 32;
				}
			} );
		}

		/////////////////////////////////////////
		static void CreateList() {
			if( s_addItems == null ) {
				s_addItems = new List<PackageTreeItem>();
			}
			if( s_removeItems == null ) {
				s_removeItems = new List<PackageTreeItem>();
			}
		}



		/////////////////////////////////////////
		public static void ClearItems() {
			CreateList();
			s_addItems.Clear();
			s_removeItems.Clear();
		}



		/////////////////////////////////////////
		public static void PushInstallItem( PackageTreeItem item ) {
			CreateList();
			s_addItems.Add( item );
		}



		/////////////////////////////////////////
		public static PackageTreeItem PopInstallItem( PackageTreeItem item ) {
			CreateList();

			var it = s_addItems.Find( x => x.name == item.name );
			if( it != null ) {
				//Debug.Log( $"PopInstallItem: {it.displayName}" );
				s_addItems.Remove( it );
				return it;
			}
			return null;
		}



		/////////////////////////////////////////
		public static void PushUninstall( PackageTreeItem item ) {
			CreateList();
			s_removeItems.Add( item );
		}



		/////////////////////////////////////////
		public static PackageTreeItem PopUninstallList( PackageTreeItem item ) {
			CreateList();

			var it = s_removeItems.Find( x => x.name == item.name );
			if( it != null ) {
				//Debug.Log( $"PopUninstallList: {it.displayName}" );
				s_removeItems.Remove( it );
				return it;
			}
			return null;
		}



		/////////////////////////////////////////
		public static void ApplyModifyList() {
			P.Load();
			foreach( var p in s_addItems ) {
				if( p.installType == InstallType.データベースに直インストール ) {
					//	P.Add( p );
					Debug.Log( p.value );
					データベースに直インストール( p.value );
				}
				P.Remove( p );
			}
			foreach( var p in s_removeItems ) {
				// ローカルパッケージはディレクトリ指定の方で。
				if( !p.value.StartsWith( "file:" ) ) {
					P.Add( p );
				}
			}
		}



		/////////////////////////////////////////
		public static PackageInfo GetPackageInfo( string packageName ) {
			var info = new PackageInfo();
			if( BuiltinPackage.GetDisplayName( ref info, packageName ) ) {
				info.icon = EditorIcon.icons_processed_prefab_icon_asset;
				info.mode = PackageMode.Builtin;
				return info;
			}
			if( PackageUser.GetDisplayName( ref info, packageName ) ) {
				info.icon = EditorIcon.icons_processed_unityengine_gameobject_icon_asset;
				return info;
			}
			if( PackageCache.GetDisplayName( ref info, packageName ) ) {
				info.icon = EditorIcon.prematcube;
				info.mode = PackageMode.InProject;
				return info;
			}

			info.displayName = packageName;
			info.icon = EditorIcon.icons_processed_unityengine_networkview_icon_asset;

			return info;
		}



		/// <summary>
		/// 内部の動作を直接呼ぶせいかUIの反応が無い、けど一応はインストールされるっぽい
		/// </summary>
		/// <param name="packageName">逆ドメイン名表記の名前</param>
		public static void データベースに直インストール( string packageName ) {
			var con = Resources.FindObjectsOfTypeAll( UnityTypes.UnityEditor_PackageManager_UI_ServicesContainer );
			//var obj = System.Activator.CreateInstance( UnityTypes.UnityEditor_PackageManager_UI_PackageDatabase );
			//var win = EditorWindowUtils.Find( UnityTypes.UnityEditor_PackageManager_UI_PackageManagerWindow );
			if( con == null ) {
				Debug.Log( "ServicesContainer が見つからない" );
				return;
			}
			var obj = con[ 0 ].GetField<object>( "m_PackageDatabase" );
			if( obj == null ) {
				Debug.Log( "m_PackageDatabase が見つからない" );
				return;
			}

			obj.MethodInvoke( "InstallFromUrl", new object[] { packageName } );
		}
	}
}
