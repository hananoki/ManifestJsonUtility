
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Hananoki.ManifestJsonUtility {

	public class PackageInfo {
		public string displayName;
		public Texture2D icon;
		public PackageMode mode;
	}

	public class PackageItem : TreeViewItem {
		public string name;
		public string version;
		public bool install;
		public bool uninstall;
	}

	public enum PackageMode {
		Builtin,
		InProject,
		User,
	}


}
