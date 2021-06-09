using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using P = HananokiEditor.ManifestJsonUtility.SettingsProject;


namespace HananokiEditor.ManifestJsonUtility {
	[Serializable]
	public class SettingsProject {

		static string projectSettingsPath => $"{fs.currentDirectory}/ProjectSettings/ManifestJsonUtility.json";

		[System.Serializable]
		public class Data {
			public string name;
			public string version;
			public string displayName;
			public Data() {
			}
			public Data( Data a ) {
				name = a.name;
				version = a.version;
				displayName = a.displayName;
			}
		}

		public static P i;

		public List<Data> m_data;



		/////////////////////////////////////////
		SettingsProject() {
			m_data = new List<Data>();
		}



		/////////////////////////////////////////
		public static P CreateNew() {
			return new P();
		}



		/////////////////////////////////////////
		public static void Add( PackageTreeItem item ) {
			Load();

			var it = i.m_data.Find( x => x.name == item.name );
			if( it != null ) {
				Debug.LogWarning( S._Thereisapackagewiththesamename_ );
				return;
			}

			i.m_data.Add( new Data {
				name = item.name,
				version = item.value,
				displayName = item.displayName,
			} );
			Save();
		}



		/////////////////////////////////////////
		public static void Remove( PackageTreeItem item ) {
			Load();
			var it = i.m_data.Find( x => x.name == item.name );
			if( it != null ) {
				i.m_data.Remove( it );
				Save();
			}
		}


		/////////////////////////////////////////
		public static void Load() {
			if( i != null ) return;

			i = JsonUtility.FromJson<P>( fs.ReadAllText( projectSettingsPath ) );
			if( i == null ) {
				i = new P();
				//Debug.Log( "new EditorToolbarSettings" );
				Save();
			}
		}


		/////////////////////////////////////////
		public static void Save() {
			File.WriteAllText( projectSettingsPath, JsonUtility.ToJson( i, true ) );
		}
	}
}
