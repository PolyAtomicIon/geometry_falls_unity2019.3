using System.Collections.Generic;

namespace UnityEngine.Style.Icon {
   /// <summary>
   /// Class FontAwesomeInfo.
   /// Implements the <see cref="UnityEngine.ScriptableObject" />
   /// </summary>
   /// <seealso cref="UnityEngine.ScriptableObject" />
   [CreateAssetMenu( menuName = "Plugins/FontAwesome", fileName = "FontAwesomeInfo", order = 0 )]
   public sealed class FontAwesomeInfo : ScriptableObject {
      [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )]
      private static void FactoryRegistry( ) => Initialize( );
      /// <summary>
      /// Gets the instance.
      /// </summary>
      /// <value>The instance.</value>
      public static FontAwesomeInfo Instance {
         get {
            if( _instance == null ) {
               FontAwesomeInfo[] resources = Resources.LoadAll<FontAwesomeInfo>( "FontAwesomeInfo" );
               if( resources != null && resources.Length > 0 ) _instance = resources[0];
            }
            return _instance;
         }
      }
      private static FontAwesomeInfo _instance;

      /// <summary>
      /// Factories the registry.
      /// </summary>
      public static void Initialize( ) {
         if( Instance == null ) return;
         foreach( TextAsset textAsset in Instance.iconJson ) {
            if( textAsset == null || textAsset.text.IsNullOrEmpty( ) ) continue;
            if( !Masamune.Utils.Json.TryDeserializeAsDictionary( textAsset.text, out Dictionary<string, dynamic> output ) ) continue;
            foreach( KeyValuePair<string, dynamic> tmp in output ) {
               if( tmp.Key.IsNullOrEmpty( ) || !( tmp.Value is Dictionary<string, dynamic> detail ) ) continue;
               string id = "fa-" + tmp.Key;
               if( UnityEngine.Icon.Contains( id ) ) continue;
               if( !( detail["styles"] is List<dynamic> styles ) ) continue;
               FontAwesome font = Instance?.iconFont?.Find( item => styles.Contains( item.ID ) );
               if( font == null ) continue;
               UnityEngine.Icon.Add( id, detail["unicode"], font.font );
            }
         }
      }

      /// <summary>
      /// Gets the icon json.
      /// </summary>
      /// <value>The icon json.</value>
      public List<TextAsset> iconJson { get => this._iconJson; }
      [SerializeField]
      private List<TextAsset> _iconJson = List.Create<TextAsset>( );

      /// <summary>
      /// Gets or sets the localize font.
      /// </summary>
      /// <value>The localize font.</value>
      public List<FontAwesome> iconFont { get => this._iconFont; }
      [SerializeField]
      private List<FontAwesome> _iconFont = List.Create<FontAwesome>( );
      
      /// <summary>
      /// Class FontAwesome.
      /// </summary>
      [System.Serializable]
      public sealed class FontAwesome : SerializableBehaviour {
         /// <summary>
         /// The identifier
         /// </summary>
         [SerializeField]
         public string ID;
         /// <summary>
         /// The font
         /// </summary>
         [SerializeField]
         public Font font;
      }
   }
}
