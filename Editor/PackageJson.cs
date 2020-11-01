using System.Collections.Generic;
using Hananoki;

#if UNITY_EDITOR
#endif


using System.IO;

public class PackageJson {

	Dictionary<string, object> m_data;

	public static PackageJson Load( string filepath ) {

		var a = UnityEditorJson.Deserialize( File.ReadAllText( filepath ) ) as Dictionary<string, object>;

		return new PackageJson { m_data = a, };
	}

	public string name => (string) m_data[ "name" ];
	public string displayName => (string) m_data[ "displayName" ];
	public string version => (string) m_data[ "version" ];
	public string unity => (string) m_data[ "unity" ];
	public string description => (string) m_data[ "description" ];
}
