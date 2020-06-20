using System;
using System.Collections;
using System.Collections.Generic;
using Assets._Main.Scripts;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

namespace Com.HHN.FPSGame.Character
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The movement speed of the player character")]
        public float Speed;

        [Tooltip("The sprint speed modifier of the player character")]
        public float SprintModifier;

        public float JumpForce;

        public LayerMask Ground;
        public Transform GroundDetector;

        public int maxHealth;
        
        public GameObject Compass;

        public Camera NormalCamera;
        public GameObject cameraParent;

        public Transform weaponParent;

        public float slideTime;
        public RingMenu MainMenuPrefab;
        protected RingMenu MainMenuInstance;

        public AnswerRingMenu AnswerMenuPrefab;
        protected AnswerRingMenu AnswerMenuInstance;
        public LayerMask isPing;

        public AudioClip YesAudioClip;
        public AudioClip NoAudioClip;
        public AudioSource audio;

        #endregion


        #region Private Fields

        private int currentHealth;
        private Rigidbody Rig;

        private Vector3 weaponPartenOrigin;
        private Vector3 targetWeaponBobPosition;
        private Vector3 weaponParentCurrentPosition;

        private float movementCounter;
        private float idleCounter;

        private float aimAngle;

        public float baseFOV;
        public float sprintFOVModifier = 1.25f;

        private Vector3 origin;

        private GameManager gameManager;
        private Transform uiHealthbar;
        private Text uiAmmo;
        private Weapon weapon;

        private bool sliding;
        private float slide_time;
        private Vector3 slideDirection;
        public float slideModifier;
        #endregion


        public float crouchModifier;
        public float crouchAmount;
        public GameObject standingCollider;
        public GameObject crouchingCollider;
        private bool crouched;


        // Start is called before the first frame update
        void Start()
        {
            audio = GetComponent<AudioSource>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            weapon = GetComponent<Weapon>();
            currentHealth = maxHealth;

            cameraParent.SetActive(photonView.IsMine);
            Compass.SetActive(photonView.IsMine);

            if (!photonView.IsMine)
            {
                gameObject.layer = 11;
                standingCollider.layer = 11;
                crouchingCollider.layer = 11;
            }

            baseFOV = NormalCamera.fieldOfView;
            origin = NormalCamera.transform.localPosition;

            if(Camera.main) Camera.main.enabled = false;
            Rig = GetComponent<Rigidbody>();
            weaponPartenOrigin = weaponParent.localPosition;
            weaponParentCurrentPosition = weaponPartenOrigin;

            if (photonView.IsMine)
            {
                uiHealthbar = GameObject.Find("HUD/Health/Bar").transform;
                uiAmmo = GameObject.Find("HUD/Ammo/Text").GetComponent<Text>();
                refreshHealthbar();
            }
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                RefreshMultiplayerState();
                return;
            }

            float tHmove = Input.GetAxisRaw("Horizontal");
            float tVmove = Input.GetAxisRaw("Vertical");

            bool sprint = Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool crouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);

            bool isGrounded = Physics.Raycast(GroundDetector.position, Vector3.down, 0.15f, Ground);
            bool isSprinting = sprint && tVmove > 0;
            bool isJumping = jump && isGrounded;
            bool isCrouching = crouch && !isSprinting && !isJumping && isGrounded;

            if (isCrouching)
            {
                photonView.RPC("SetCrouch", RpcTarget.All, !crouched);
            }

            if (isJumping)
            {
                if (crouched) photonView.RPC("SetCrouch", RpcTarget.All, false);
                Rig.AddForce(Vector3.up * JumpForce);
                Debug.Log("Is Jumping");
            }

            if (Input.GetKeyDown(KeyCode.U)) takeDamage(100);


            if (sliding)
            {
                HeadBob(movementCounter, 0.09f, 0.05f);
                weaponParent.localPosition =
                    Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            }
            else if (tHmove == 0 && tVmove == 0)
            {
                HeadBob(idleCounter, 0.025f, 0.025f);
                idleCounter += Time.deltaTime;
                weaponParent.localPosition =
                    Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
            }
            else if (!isSprinting && !crouched)
            {
                HeadBob(movementCounter, 0.035f, 0.035f);
                movementCounter += Time.deltaTime * 3f;
                weaponParent.localPosition =
                    Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else if (crouched)
            {
                HeadBob(movementCounter, 0.02f, 0.02f);
                movementCounter += Time.deltaTime * 1.75f;
                weaponParent.localPosition =
                    Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else
            {
                HeadBob(movementCounter, 0.09f, 0.05f);
                movementCounter += Time.deltaTime * 3f;
                weaponParent.localPosition =
                    Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            }


            refreshHealthbar();
            weapon.RefreshAmmo(uiAmmo);

            if (Input.GetKey(KeyCode.F) && !activeRingMenu)
            {
                Debug.Log("Key F pressed");
                time += Time.deltaTime;
                Debug.Log(time);
                if (time > timeSpan)
                {
                    Debug.Log("Pressed for 0.1 s");
                    Transform tSpawn = transform.Find("Cameras/Player Camera");
                    RaycastHit tHit = new RaycastHit();
                    if (Physics.Raycast(tSpawn.position, tSpawn.forward, out tHit, 1000f, isPing))
                    {
                        activeRingMenu = true;
                        AnswerMenuInstance = Instantiate(AnswerMenuPrefab, FindObjectOfType<Canvas>().transform);
                        AnswerMenuInstance.callback = AnswerClick;
                        Debug.Log("Created answerRingMenu");
                    }
                    else
                    {
                        activeRingMenu = true;
                        MainMenuInstance = Instantiate(MainMenuPrefab, FindObjectOfType<Canvas>().transform);
                        MainMenuInstance.callback = MenuClick;
                        Debug.Log("Created mainRingMenu");

                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                if (time < timeSpan)
                {
                    photonView.RPC("PingWaypoint", RpcTarget.All);
                }

                time = 0f;
            }

        }
        public float timeSpan = 0.05f;
        private bool activeRingMenu = false;
        private float time;


        private void MenuClick(string path)
        {
            activeRingMenu = false;
            Debug.Log("Path: " + path);
            var paths = path.Split('/');

            Debug.Log(int.Parse(paths[1]));
            switch (int.Parse(paths[1]))
          {
              case 0:
                  photonView.RPC("PingWaypoint", RpcTarget.All);
                    break;
              case 1:
                  photonView.RPC("PingDefending", RpcTarget.All);
                    break;
              case 2:
                  photonView.RPC("PingEnemy", RpcTarget.All);
                    break;
              case 3:
                  photonView.RPC("PingLooting", RpcTarget.All);
                    break;
          }
        }


        private void AnswerClick(string path)
        {
            activeRingMenu = false;
            Debug.Log("Path: " + path);
            var paths = path.Split('/');

            Debug.Log(int.Parse(paths[1]));
        
            switch (int.Parse(paths[1]))
            {
                case 0:
                    Debug.Log("Yes");
                    audio.PlayOneShot(YesAudioClip, 0.75f);
                    break;
                case 1:
                    Debug.Log("No");
                    audio.PlayOneShot(NoAudioClip, 0.75f);
                    break;
            }

        }
        

        private void RefreshMultiplayerState()
        {
            float cacheEulY = weaponParent.localEulerAngles.y;

            Quaternion targetRotation = Quaternion.identity * Quaternion.AngleAxis(aimAngle, Vector3.right);
            weaponParent.rotation = Quaternion.Slerp(weaponParent.rotation, targetRotation, Time.deltaTime * 8f);

            Vector3 finalRotation = weaponParent.localEulerAngles;
            finalRotation.y = cacheEulY;

            weaponParent.localEulerAngles = finalRotation;

            NormalCamera.transform.rotation = cameraRotation;
            NormalCamera.transform.position = cameraPosition;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!photonView.IsMine) return;

            float tHmove = Input.GetAxisRaw("Horizontal");
            float tVmove = Input.GetAxisRaw("Vertical");

            bool sprint = Input.GetKey(KeyCode.LeftShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool slide = Input.GetKeyDown(KeyCode.LeftControl);


            bool isGrounded = Physics.Raycast(GroundDetector.position, Vector3.down, 0.1f, Ground);
            bool isSprinting = sprint && tVmove > 0;
            bool isJumping = jump && isGrounded;
            bool isSliding = isSprinting && slide && !sliding;

            Vector3 tDirection = Vector3.zero;
            float tAdjustedSpeed = Speed;

            if (!sliding){
                tDirection = new Vector3(tHmove, 0, tVmove); 
                tDirection.Normalize();
                tDirection = transform.TransformDirection(tDirection);

                if (isSprinting)
                {
                    if(crouched) photonView.RPC("SetCrouch", RpcTarget.All, false);
                    tAdjustedSpeed *= SprintModifier;
                }else if (crouched)
                {
                    tAdjustedSpeed *= crouchModifier;
                }
            }
            else
            {
                tDirection = slideDirection;
                tAdjustedSpeed *= slideModifier;
                slide_time -= Time.deltaTime;
                if (slide_time <= 0)
                {
                    sliding = false;
                    weaponParentCurrentPosition += Vector3.up * (0.5f - crouchAmount);
                }
            }

            Vector3 tTargetVelocity = tDirection * tAdjustedSpeed * Time.deltaTime;
            tTargetVelocity.y = Rig.velocity.y;
            Rig.velocity = tTargetVelocity;

            if (isSliding)
            {
                sliding = true;
                slideDirection = tDirection;
                slide_time = slideTime;
                weaponParentCurrentPosition += Vector3.down * (0.5f - crouchAmount);
                if(!crouched) photonView.RPC("SetCrouch", RpcTarget.All, true);
            }


            if (sliding)
            {
                NormalCamera.fieldOfView = Mathf.Lerp(NormalCamera.fieldOfView, baseFOV * sprintFOVModifier * 1.25f, Time.deltaTime * 8f);
                NormalCamera.transform.localPosition = Vector3.Lerp(NormalCamera.transform.localPosition,
                    origin + Vector3.down * 0.25f, Time.deltaTime * 6f);
            }
            else
            {
                if (isSprinting)
                {
                    Debug.Log("Is Sprinting");
                    NormalCamera.fieldOfView = Mathf.Lerp(NormalCamera.fieldOfView, baseFOV * sprintFOVModifier,
                        Time.deltaTime * 8f);
                }
                else
                {
                    NormalCamera.fieldOfView = Mathf.Lerp(NormalCamera.fieldOfView, baseFOV, Time.deltaTime * 8f);
                }

                if (crouched)
                    NormalCamera.transform.localPosition = Vector3.Lerp(NormalCamera.transform.localPosition,
                        origin + Vector3.down * crouchAmount, Time.deltaTime * 6f);
                else NormalCamera.transform.localPosition = Vector3.Lerp(NormalCamera.transform.localPosition,
                    origin, Time.deltaTime * 6f);
            }
        }

        [PunRPC]
        void SetCrouch(bool pState)
        {
            if(crouched == pState) return;

            crouched = pState;
            if (crouched)
            {
                standingCollider.SetActive(false);
                crouchingCollider.SetActive(true);
                weaponParentCurrentPosition += Vector3.down * crouchAmount;
            }
            else
            {
                standingCollider.SetActive(true);
                crouchingCollider.SetActive(false);
                weaponParentCurrentPosition -= Vector3.down * crouchAmount;
            }
        }

        void HeadBob(float pZ, float pXIntensity, float pYIntensity)
        {

            float tAimAdjust = 1f;
            if (weapon.isAiming) tAimAdjust = 0.1f;
            
            targetWeaponBobPosition = weaponParentCurrentPosition + new Vector3(Mathf.Cos(pZ ) * pXIntensity * tAimAdjust, Mathf.Sin(pZ *2) * pYIntensity * tAimAdjust, 0);
        }

        
        public void takeDamage(int pDamage)
        {
            if (photonView.IsMine)
            {
                currentHealth -= pDamage;
                refreshHealthbar();
                Debug.Log(currentHealth);
                if (currentHealth <= 0)
                {
                    gameManager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        void refreshHealthbar()
        {
            float tHealthRatio = (float) currentHealth / (float) maxHealth;
            uiHealthbar.localScale = new Vector3(tHealthRatio,1,1);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext((int)(weaponParent.transform.localEulerAngles.x * 100f));
                stream.SendNext((NormalCamera.transform.position));
                stream.SendNext((NormalCamera.transform.rotation));
            }
            else
            {
                aimAngle = (int) stream.ReceiveNext() / 100f;
                cameraPosition = (Vector3) stream.ReceiveNext();
                cameraRotation = (Quaternion) stream.ReceiveNext();
            }
        }

        private Vector3 cameraPosition;
        private Quaternion cameraRotation;
    }

    
}