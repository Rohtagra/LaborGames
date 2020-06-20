using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Com.HHN.FPSGame.Character
{
    public class PlayerLooking : MonoBehaviourPunCallbacks
    {

        public static bool CursorLocked = true;

        #region Public Fields

        public Transform Player;
        public Transform Cams;
        public Transform weapon;

        public float XSensitivity;
        public float YSensitivity;
        public float MaxAngle;

        #endregion

        #region Private Fields

        private Quaternion camCenter;

        #endregion


        #region Monobehaviour Calllbacks

        // Start is called before the first frame update
        void Start()
        {
            camCenter = Cams.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine) return;

            if (!Input.GetKey(KeyCode.F))
            {
                SetY();
                SetX();
            }

            UpdateCursorLock();
        }

        #endregion

        #region private Methods

        void SetY()
        {
            float tInput = Input.GetAxis("Mouse Y") * YSensitivity * Time.deltaTime;
            Quaternion tAdj = Quaternion.AngleAxis(tInput, -Vector3.right);
            Quaternion tDelta = Cams.localRotation * tAdj;

            if (Quaternion.Angle(camCenter, tDelta) < MaxAngle)
            {
                Cams.localRotation = tDelta;
            }

            weapon.rotation = Cams.rotation;
        }

        void SetX()
        {
            float tInput = Input.GetAxis("Mouse X") * XSensitivity * Time.deltaTime;
            Quaternion tAdj = Quaternion.AngleAxis(tInput, Vector3.up);
            Quaternion tDelta = Player.localRotation * tAdj;
            Player.localRotation = tDelta;
        }

        void UpdateCursorLock()
        {
            if (Input.GetKey(KeyCode.F))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = false;
            }
            else
            {
                if (CursorLocked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        CursorLocked = false;
                    }
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        CursorLocked = true;
                    }
                }
            }
        }

        #endregion

    }
}