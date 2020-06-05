using System.Collections.Generic;
using UnityEngine;

namespace UIElements.Expansion.Sample {
   /// <summary>
   /// Class MonoBehaviourScript.
   /// Implements the <see cref="UnityEngine.MonoBehaviour" />
   /// </summary>
   /// <seealso cref="UnityEngine.MonoBehaviour" />
   public class MonoBehaviourScript : MonoBehaviour {

      /// <summary>
      /// The serializable script list
      /// </summary>
      [SerializeField]
      public List<SerializableScript> serializableScriptList = new List<SerializableScript>( );      

   }
}
