
using System;
using System.Collections;
using UnityEditor;

using E = Hananoki.ManifestJsonUtility.SettingsEditor;


namespace Hananoki.ManifestJsonUtility {
	public static class BuiltinPackage {
		public static System.Collections.Hashtable dic;

		public static bool GetDisplayName( out string displayName, string packageName ) {
			if( dic == null ) {
				dic = new Hashtable();
				var files = DirectoryUtils.GetFiles( $"{EditorApplication.applicationContentsPath}/Resources/PackageManager/BuiltInPackages", "package.json", System.IO.SearchOption.AllDirectories );
				foreach( var f in files ) {
					var pj = PackageJson.Load( f );

					dic.Add( pj.name, pj.displayName );
				}
			}
			if( dic.Contains( packageName ) ) {
				displayName = (string) dic[ packageName ];
				return true;
			}
			displayName = string.Empty;
			return false;
		}
	}


	public static class PackageCache {
		public static System.Collections.Hashtable dic;

		public static bool GetDisplayName( out string displayName, string packageName ) {
			if( dic == null ) {
				dic = new Hashtable();
				var files = DirectoryUtils.GetFiles( $"{Environment.CurrentDirectory}/Library/PackageCache", "package.json", System.IO.SearchOption.AllDirectories );
				foreach( var f in files ) {
					var pj = PackageJson.Load( f );

					dic.Add( pj.name, pj.displayName );
				}
			}
			if( dic.Contains( packageName ) ) {
				displayName = (string) dic[ packageName ];
				return true;
			}
			displayName = string.Empty;
			return false;
		}
	}


	public static class PackageUser {
		public static System.Collections.Hashtable dic;

		public static bool GetDisplayName( out string displayName, string packageName ) {
			if( dic == null ) {
				dic = new Hashtable();

				E.Load();
				foreach( var path in E.i.m_dirList ) {
					var files = DirectoryUtils.GetFiles( path, "package.json", System.IO.SearchOption.AllDirectories );
					foreach( var fname in files ) {
						//Debug.Log( sss );
						var pj = PackageJson.Load( fname );

						dic.Add( pj.name, pj.displayName );
					}
				}
			}
			if( dic.Contains( packageName ) ) {
				displayName = (string) dic[ packageName ];
				return true;
			}
			displayName = string.Empty;
			return false;
		}
	}

	

	

}
