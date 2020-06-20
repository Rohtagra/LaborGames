using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Com.HHN.FPSGame.Character
{
    public class Sway : MonoBehaviour
    {
        public float intensity;
        public float smooth;
        public bool isMine;

        private Quaternion originRotation;

        private void Start()
        {
            originRotation = transform.localRotation;
        }
        
        private void Update()
        {
            UpdateSway();
        }


        private void UpdateSway()
        {
            float tXMouse = Input.GetAxis("Mouse X");
            float tYMouse = Input.GetAxis("Mouse Y");

            if (!isMine)
            {
                tXMouse = 0;
                tYMouse = 0;
            }

            Quaternion tXAdj = Quaternion.AngleAxis(-intensity* tXMouse, Vector3.up);
            Quaternion tYAdj = Quaternion.AngleAxis(intensity* tYMouse, Vector3.up);
            Quaternion targetRotation = originRotation * tXAdj * tYAdj;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
        }
    }
}