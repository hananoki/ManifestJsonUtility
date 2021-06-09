
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HananokiEditor.ManifestJsonUtility {

	public class PackageInfo {
		public string displayName;
		public Texture2D icon;
		public PackageMode mode;
		public string version;
	}

	public class PackageTreeItem : TreeViewItem {
		public string name;
		public string value;
		public bool install;
		public bool uninstall;
		public string version;
		public InstallType installType;
	}


	public enum PackageMode {
		Builtin,
		InProject,
		User,
	}

	public enum InstallType {
		通常,
		データベースに直インストール,
	}

}
