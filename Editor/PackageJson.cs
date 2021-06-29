
using HananokiEditor.Extensions;
using System.Collections.Generic;

namespace HananokiEditor.ManifestJsonUtility {

	/// <summary>
	/// package.jsonの読み取り
	/// </summary>
	public class PackageJson {

		Dictionary<string, object> m_data;


		public static PackageJson Load( string filepath ) {

			var data = EditorJson.Deserialize( filepath.ReadAllText() ) as Dictionary<string, object>;

			return new PackageJson { m_data = data, };
		}


		public string name => (string) m_data[ "name" ];
		public string displayName {
			get {
				// com.unity.adsとかdisplayNameが無い奴がある
				object result ="";
				if( m_data.TryGetValue( "displayName", out result ) ) return (string) result;
				return name;
			}
		}
		public string version     => (string) m_data[ "version" ];
		public string unity       => (string) m_data[ "unity" ];
		public string description => (string) m_data[ "description" ];
	}
}
