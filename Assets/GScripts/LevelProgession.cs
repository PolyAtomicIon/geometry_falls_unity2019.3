using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgession : MonoBehaviour
{

    public List<Image> points = new List<Image>();

    public Color active = new Color(255, 148, 22);
    public Color inactive = new Color(155, 155, 155);

    public void increment(int Index){
        if( Index < points.Count )
            points[Index].color = active;
    }

    public void refresh(){
        foreach(Image point in points){
            point.color = inactive;
        }
    }

}
