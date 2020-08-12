using UnityEngine;
 using UnityEngine.UI;
 using System.Collections;
 
 namespace Menu {
     public class ScrollbarManager : MonoBehaviour {
         Scrollbar scrollH;
         Scrollbar scrollV;
         
         void Awake () {
             ScrollRect rect = GetComponent<ScrollRect>();
            //  scrollH = rect.horizontalScrollbar;
             scrollV = rect.verticalScrollbar;
         }
         
         void Start () {
             ForceScroll(1f);
         }
 
         public void ForceScroll (float pos) {
            //  if (scrollH) scrollH.value = pos;
             if (scrollV) scrollV.value = pos;
         }
     }
 }