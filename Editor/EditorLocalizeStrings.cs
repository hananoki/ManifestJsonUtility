﻿// Generated by Assets/Hananoki/ManifestJsonUtility/Localize/en-US.csv

namespace HananokiEditor.ManifestJsonUtility {
	public static class L {
		public static string Tr( int n ) {
			try {
				return EditorLocalize.GetPakage( Package.name ).m_Strings[ n ];
			}
			catch( System.Exception ) {
			}
			return string.Empty;
		}
	}
	public static class S {
		public static string _LocalPackagePaths => L.Tr( 0 );
		public static string _Installablepackages => L.Tr( 1 );
	}

#if UNITY_EDITOR
  public class LocalizeEvent {
    [HananokiEditorLocalizeRegister]
    public static void Changed() {
			EditorLocalize.Load( Package.name, "f26dd4966cc73ec4c98133eb9e4a2338" );
		}
	}
#endif
}
