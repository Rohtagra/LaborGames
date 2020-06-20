using System;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;
public class Compass : MonoBehaviourPunCallbacks
{
    public RawImage CompassImage;
    public Transform Player;
    public Text CompassDirectionText;

    public void Update()
    {
        CompassImage.uvRect = new Rect(Player.localEulerAngles.y / 360, 0, 1, 1);

        Vector3 forward = Player.transform.forward;

        forward.y = 0;

        float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
                headingAngle = 5 * (Mathf.RoundToInt(headingAngle / 5.0f));

                CompassDirectionText.text = headingAngle.ToString();

              }

}