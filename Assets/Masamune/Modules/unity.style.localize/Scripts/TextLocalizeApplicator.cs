using UnityEngine;

namespace Masamune {
   /// <summary>
   /// Class TextLocalizeApplicator.
   /// Implements the <see cref="UnityEngine.MonoBehaviour" />
   /// </summary>
   /// <seealso cref="UnityEngine.MonoBehaviour" />
   [AddComponentMenu( "Masamune/Localize/Text Localize Applicator" ), DisallowMultipleComponent, ExecuteInEditMode, RequireComponent( typeof( UnityEngine.UI.Text ) )]
   public class TextLocalizeApplicator : MonoBehaviour {

      /// <summary>
      /// The text
      /// </summary>
      [SerializeField]
      public UnityEngine.UI.Text text;

      /// <summary>
      /// Gets or sets the icon.
      /// </summary>
      /// <value>The icon.</value>
      public string localizeText {
         get => this._localizeText;
         set {
            this._localizeText = value;
            this.Repaint( );
         }
      }
      [SerializeField]
      private string _localizeText;

      /// <summary>
      /// Gets or sets the language.
      /// </summary>
      /// <value>The language.</value>
      public SystemLanguage language {
         get => this._language;
         set {
            this._language = value;
            this.Repaint( );
         }
      }
      [SerializeField]
      private SystemLanguage _language = SystemLanguage.Unknown;

      /// <summary>
      /// Awakes this instance.
      /// </summary>
      private void Awake( ) {
         if( this.text == null ) this.text = this.GetComponent<UnityEngine.UI.Text>( );
         this.Repaint( );
      }
      /// <summary>
      /// Called when [validate].
      /// </summary>
      private void OnValidate( ) {
         this.Repaint( );
      }

      /// <summary>
      /// Repaints this instance.
      /// </summary>
      public void Repaint( ) {
         if( this.text == null || this.localizeText.IsNullOrEmpty( ) ) return;
         SystemLanguage language = this.language == SystemLanguage.Unknown ? Localize.SystemLanguage : this.language;
         Localize localize = Localize.Get( this.localizeText, language, this.localizeText );
         if( localize == null ) return;
         this.text.font = localize;
         this.text.text = localize;
      }
   }
}
