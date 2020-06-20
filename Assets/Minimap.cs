using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    public float previousShadowDistance;

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
     
    void OnPreRender()
    {
        previousShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }
    void OnPostRender()
    {
        QualitySettings.shadowDistance = previousShadowDistance;
    }
}
