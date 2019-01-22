using UnityEngine;

public class DroidOrientation : MonoBehaviour
{
    public Transform director;
    public Transform rotator;
    public float sizeScale =  0.8f;
    public float angularScale = 120f;

    /* Get pitch and roll from direction and tile data */
    void LateUpdate()
    {
        float hx0, hx1, hy0, hy1, ax, ay;

        Vector3 pos = director.position;

        // Find the height of 4 points around the droid.
        //    hy0
        // hx0 * hx1      (* = droid)
        //    hy1
        hx1 = TheMap.Height(pos + director.forward * sizeScale);
        hx0 = TheMap.Height(pos - director.forward * sizeScale);
        hy1 = TheMap.Height(pos + director.right * sizeScale);
        hy0 = TheMap.Height(pos - director.right * sizeScale);

        if (hx0 == 0)
            hx0 = hx1;
        else if (hx1 == 0)
            hx1 = hx0;

        if (hy0 == 0)
            hy0 = hy1;
        else if (hy1 == 0)
            hy1 = hy0;

        rotator.localPosition = new Vector3(0, (hx0 + hx1 + hy0 + hy1) / 4 - director.position.y, 0);

        // Calculate pitch of ground.
        ax = Mathf.Atan2(hx0 - hx1, sizeScale + sizeScale);
        ay = Mathf.Atan2(hy1 - hy0, sizeScale + sizeScale);

        rotator.localRotation = Quaternion.Euler(ax * angularScale, 0, ay * angularScale);
    }


}