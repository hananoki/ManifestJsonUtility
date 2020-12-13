
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HananokiEditor.ManifestJsonUtility {

	public class PackageInfo {
		public string displayName;
		public Texture2D icon;
		public PackageMode mode;
		public string version;
	}

	public class PackageItem : TreeViewItem {
		public string name;
		public string value;
		public bool install;
		public bool uninstall;
		public string version;
	}

	public enum PackageMode {
		Builtin,
		InProject,
		User,
	}


}
