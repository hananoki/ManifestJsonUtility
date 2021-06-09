using System.Collections.Generic;
using E = HananokiEditor.ManifestJsonUtility.SettingsEditor;

namespace HananokiEditor.ManifestJsonUtility {
	[System.Serializable]
	public class SettingsEditor {

		public static E i;

		public List<string> m_dirList;

		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<E>.Get( Package.editorPrefName );
		}


		public static void Save() {
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}
	}
}
