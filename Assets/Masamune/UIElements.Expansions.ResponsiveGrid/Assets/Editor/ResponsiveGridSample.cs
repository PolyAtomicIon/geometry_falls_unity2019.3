using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;

/// <summary>
/// Class ResponsiveGridSample.
/// </summary>
public class ResponsiveGridSample : EditorWindow {
   /// <summary>
   /// Show the EditorWindow window.
   /// </summary>
   [MenuItem( "Masamune/Demo/Responsive Grid Sample" )]
   public new static void Show( ) {
      ResponsiveGridSample wnd = GetWindow<ResponsiveGridSample>( );
      wnd.titleContent = new GUIContent( "Responsive Grid Sample" );
   }
   private void OnEnable( ) => this.ApplyStyle( );
}