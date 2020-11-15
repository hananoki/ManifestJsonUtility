
using UnityEditor;

namespace Hananoki.ManifestJsonUtility {
  public static class Package {
    public const string name = "ManifestJsonUtility";
    public const string editorPrefName = "Hananoki.ManifestJsonUtility";
    public const string version = "0.5.1-preview";
  }
  
#if UNITY_EDITOR
  [EditorLocalizeClass]
  public class LocalizeEvent {
    [EditorLocalizeMethod]
    public static void Changed() {
      foreach( var filename in DirectoryUtils.GetFiles( AssetDatabase.GUIDToAssetPath( "f26dd4966cc73ec4c98133eb9e4a2338" ), "*.csv" ) ) {
        if( filename.Contains( EditorLocalize.GetLocalizeName() ) ) {
          EditorLocalize.Load( Package.name, AssetDatabase.AssetPathToGUID( filename ), "d9ad31d211b766041af35b29d6ad3f2e" );
        }
      }
    }
  }
#endif
}
