using System.Collections.Generic;
using UnityEngine;

namespace UIElements.Expansion.Sample {
   /// <summary>
   /// Class ScriptableObjectScript.
   /// Implements the <see cref="UnityEngine.ScriptableObject" />
   /// </summary>
   /// <seealso cref="UnityEngine.ScriptableObject" />
   [CreateAssetMenu( menuName = "UIElements Expansions/Common/ScriptableObject Sample", fileName = "ScriptableObjectSample", order = -500 )]
   public class ScriptableObjectScript : ScriptableObject {

      /// <summary>
      /// The serializable script list
      /// </summary>
      [SerializeField]
      public List<SerializableScript> serializableScriptList = new List<SerializableScript>( );

   }
}
