
using HananokiRuntime.Extensions;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

using E = HananokiEditor.ManifestJsonUtility.SettingsEditor;


namespace HananokiEditor.ManifestJsonUtility {
	public static class BuiltinPackage {
		public static System.Collections.Hashtable dic;

		public static bool GetDisplayName( ref PackageInfo info, string packageName ) {
			if( dic == null ) {
				dic = new Hashtable();
				var files = DirectoryUtils.GetFiles( $"{EditorApplication.applicationContentsPath}/Resources/PackageManager/BuiltInPackages", "package.json", System.IO.SearchOption.AllDirectories );
				foreach( var f in files ) {
					var pj = PackageJson.Load( f );

					dic.Add( pj.name, pj );
				}
			}
			if( dic.Contains( packageName ) ) {
				info.displayName = ((PackageJson) dic[ packageName ]).displayName;
				info.version = ( (PackageJson) dic[ packageName ] ).version;
				return true;
			}
			info.displayName = string.Empty;
			info.version = string.Empty;
			return false;
		}
	}


	public static class PackageCache {
		public static System.Collections.Hashtable dic;

		public static bool GetDisplayName( ref PackageInfo info, string packageName ) {
			if( dic == null ) {
				dic = new Hashtable();
				var files = DirectoryUtils.GetFiles( $"{Environment.CurrentDirectory}/Library/PackageCache", "package.json", System.IO.SearchOption.AllDirectories );
				foreach( var f in files ) {
					var pj = PackageJson.Load( f );

					dic.Add( pj.name, pj );
				}
			}
			if( dic.Contains( packageName ) ) {
				info.displayName = ( (PackageJson) dic[ packageName ] ).displayName;
				info.version = ( (PackageJson) dic[ packageName ] ).version;
				if( info.version .StartsWith("http")) {
					info.version = "URL";
				}
				return true;
			}
			info.displayName = string.Empty;
			info.version = string.Empty;
			return false;
		}
	}


	public static class PackageUser {
		public static System.Collections.Hashtable dic;

		public static bool GetDisplayName( ref PackageInfo info, string packageName ) {
			if( dic == null ) {
				dic = new Hashtable();

				E.Load();
				foreach( var path in E.i.m_dirList ) {
					if( path.IsEmpty() ) continue;
					var files = DirectoryUtils.GetFiles( path, "package.json", System.IO.SearchOption.AllDirectories );
					foreach( var fname in files ) {
						var pj = PackageJson.Load( fname );
						if( dic.Contains( pj.name ) ) {
							Debug.LogWarning( $"Duplicate: {pj.name}: {fname}" );
							continue;
						}

						dic.Add( pj.name, pj );
					}
				}
			}
			if( dic.Contains( packageName ) ) {
				info.displayName = ( (PackageJson) dic[ packageName ] ).displayName;
				info.version = ( (PackageJson) dic[ packageName ] ).version;
				return true;
			}
			info.displayName = string.Empty;
			info.version = string.Empty;
			return false;
		}
	}

	

	

}
