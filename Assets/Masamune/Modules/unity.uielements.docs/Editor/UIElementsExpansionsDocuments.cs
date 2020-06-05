using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

/// <summary>
/// Class UIElementsExpansionsDocuments.
/// </summary>
public class UIElementsExpansionsDocuments : EditorWindow {
   /// <summary>
   /// Show the EditorWindow window.
   /// </summary>
   [MenuItem( "Masamune/UIElements Expansions", false, 151 )]
   public new static void Show( ) {
      UIElementsExpansionsDocuments wnd = GetWindow<UIElementsExpansionsDocuments>( );
      wnd.titleContent = new GUIContent( "UIElements Expansions" );
   }
   private void OnEnable( ) {
      this.ApplyStyle( );
      Toggle toggle = this.rootVisualElement.Q<Toggle>( "not_show_at_startup" );
      if( toggle == null ) return;
      toggle.value = EditorPrefs.HasKey( "MasamuneIsHideUIElementsExpansionsDocuments" );
      toggle.RegisterValueChangedCallback( ( evt ) => {
         if( evt.newValue ) EditorPrefs.SetBool( "MasamuneIsHideUIElementsExpansionsDocuments", true );
         else EditorPrefs.DeleteKey( "MasamuneIsHideUIElementsExpansionsDocuments" );
      } );
   }

   [InitializeOnLoadMethod]
   static void ShowOnInitialize( ) {
      if( EditorPrefs.HasKey( "MasamuneIsHideUIElementsExpansionsDocuments" ) ) return;
      EditorApplication.delayCall += Show;
   }
}