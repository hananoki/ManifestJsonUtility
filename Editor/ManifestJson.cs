
using Hananoki;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public static class ManifestJson {
	public static Dictionary<string, object> s_manifest = null;

	public static bool modify;

	public static void Load() {
		s_manifest = UnityEditorJson.Deserialize( File.ReadAllText( "Packages/manifest.json" ) ) as Dictionary<string, object>;
		modify = false;
	}


	public static void Save() {
		modify = false;
		File.WriteAllText( "Packages/manifest.json", UnityEditorJson.Serialize( s_manifest, true ) );
		AssetDatabase.Refresh();
	}


	public static IDictionary GetDependencies() {
		return (IDictionary) s_manifest[ "dependencies" ];
	}


	public static void InstallPackage( string packageName, string version ) {

		var dic = GetDependencies();

		if( !dic.Contains( packageName ) ) {
			dic.Add( packageName, version );
		}

		modify = true;
	}


	public static void UninstallPackage( string packageName ) {
		var d1 = (IDictionary) s_manifest[ "dependencies" ];
		d1.Remove( packageName );
		//Save();
		modify = true;
	}
}
