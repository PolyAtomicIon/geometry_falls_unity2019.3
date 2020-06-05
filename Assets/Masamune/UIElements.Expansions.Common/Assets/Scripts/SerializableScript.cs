using UnityEngine;

namespace UIElements.Expansion.Sample {
   /// <summary>
   /// Class SerializableScript.
   /// Implements the <see cref="UnityEngine.SerializableBehaviour" />
   /// By inheriting SerializableBehaviour, PropertyDrawer and uxml file of Inspector are automatically assigned.
   /// </summary>
   /// <seealso cref="UnityEngine.SerializableBehaviour" />
   [System.Serializable]
   public class SerializableScript : SerializableBehaviour {

      /// <summary>
      /// The name
      /// </summary>
      [SerializeField]
      public string name;

      /// <summary>
      /// The address
      /// </summary>
      [SerializeField]
      public string address;

      /// <summary>
      /// The age
      /// </summary>
      [SerializeField]
      public int age;
   }
}
