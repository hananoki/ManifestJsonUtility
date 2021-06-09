﻿
using UnityEditor;

namespace HananokiEditor.ManifestJsonUtility {
  public static class Package {
    public const string reverseDomainName = "com.hananoki.manifest-json-utility";
    public const string name = "ManifestJsonUtility";
    public const string nameNicify = "Manifest Json Utility";
    public const string editorPrefName = "Hananoki.ManifestJsonUtility";
    public const string version = "0.6.0";
		[HananokiEditorMDViewerRegister]
		public static string MDViewerRegister() {
			return "91a50302e95aae445aa204c762274bfb";
		}
  }
}
