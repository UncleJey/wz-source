using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
   public void Select(Vector2Int pPoint)
   {
        transform.localPosition = HexMap.HexToWorld(pPoint);
   }

}
