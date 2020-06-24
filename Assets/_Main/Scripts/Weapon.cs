using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.HHN.FPSGame.Character
{
    public class Weapon : MonoBehaviourPunCallbacks
    {

        public Gun[] loadout;
        public Transform weaponParent;
        public GameObject bulletholePrefab;
        public LayerMask canBeShot;

        public GameObject pingWaypointPrefab;
        public GameObject pingDefendingPrefab;
        public GameObject pingEnemyPrefab;
        public GameObject pingLootingPrefab;
        public LayerMask canBePinged;
        public bool isAiming = false;
        public Image crosshair;

        private float currentCooldown;
        private int currentInd;
        private GameObject currentWeapon;

        private bool isReloading = false;
        public GameObject grenadePrefab;
        public float throwForce;

        private void Start()
        {
            if(!photonView.IsMine) crosshair.enabled = false;
            foreach (Gun a in loadout)
            {
                a.init();
            }
            Equip(0);
        }

        // Update is called once per frame
        void Update()
        {

            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.All, 0); }
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha2)) { photonView.RPC("Equip", RpcTarget.All, 1); }

            if (currentWeapon != null)
            {
                if (photonView.IsMine)
                {
                    Aim(Input.GetMouseButton(1));

                    if (loadout[currentInd].burst != 1)
                    {
                        if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
                        {
                            if (!isReloading)
                            {
                                if (loadout[currentInd].FireBullet())
                                {
                                    photonView.RPC("Shoot", RpcTarget.All);
                                }
                                else
                                {
                                    StartCoroutine(Reload(loadout[currentInd].reload));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButton(0) && currentCooldown <= 0)
                        {
                            if (!isReloading)
                            {
                                if (loadout[currentInd].FireBullet())
                                {
                                    photonView.RPC("Shoot", RpcTarget.All);
                                }
                                else
                                {
                                    StartCoroutine(Reload(loadout[currentInd].reload));
                                }
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        StartCoroutine(Reload(loadout[currentInd].reload));
                    }

                    if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
                }

                currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

            } 
             

            if (Input.GetKeyDown(KeyCode.G) && photonView.IsMine)
            {
                GameObject tNewGrenade = PhotonNetwork.Instantiate("Grenade", currentWeapon.transform.position, Quaternion.identity) as GameObject;
                Rigidbody r = tNewGrenade.GetComponent<Rigidbody>();
                r.AddForce(transform.forward * throwForce);
                // photonView.RPC("Grenade", RpcTarget.All);
                Debug.Log("Grenaaaaaaaaade");
            }


        }

        IEnumerator Reload(float p_wait)
        {
            isReloading = true;
            currentWeapon.SetActive(false);
            yield return new WaitForSeconds(p_wait);
            loadout[currentInd].Reload();
            currentWeapon.SetActive(true);
            isReloading = false;

        }

        [PunRPC]
        private void PingWaypoint()
        {
            Debug.Log("Pinging Waypoint");
            Transform tSpawn = transform.Find("Cameras/Player Camera");
            
            RaycastHit tHit = new RaycastHit();
            if (Physics.Raycast(tSpawn.position, tSpawn.forward, out tHit, 1000f, canBePinged))
            {
                GameObject tNewPing = Instantiate(pingWaypointPrefab, tHit.point + tHit.normal * 0.001f, Quaternion.identity) as GameObject;
                tNewPing.transform.LookAt(tHit.point + tHit.normal);
                Destroy(tNewPing, 5f);
            }
        }

        [PunRPC]
        private void PingEnemy()
        {
            Debug.Log("Pinging Enemy");
            Transform tSpawn = transform.Find("Cameras/Player Camera");

            RaycastHit tHit = new RaycastHit();
            if (Physics.Raycast(tSpawn.position, tSpawn.forward, out tHit, 1000f, canBePinged))
            {
                GameObject tNewPing = Instantiate(pingEnemyPrefab, tHit.point + tHit.normal * 0.001f, Quaternion.identity) as GameObject;
                tNewPing.transform.LookAt(tHit.point + tHit.normal);
                Destroy(tNewPing, 5f);
            }
        }

        [PunRPC]
        private void PingLooting()
        {
            Debug.Log("Pinging Looting");
            Transform tSpawn = transform.Find("Cameras/Player Camera");

            RaycastHit tHit = new RaycastHit();
            if (Physics.Raycast(tSpawn.position, tSpawn.forward, out tHit, 1000f, canBePinged))
            {
                GameObject tNewPing = Instantiate(pingLootingPrefab,tHit.point + tHit.normal * 0.001f, Quaternion.identity) as GameObject;
                tNewPing.transform.LookAt(tHit.point + tHit.normal);
                Destroy(tNewPing, 5f);
            }
        }

        [PunRPC]
        private void PingDefending()
        {
            Debug.Log("Pinging Defending");
            Transform tSpawn = transform.Find("Cameras/Player Camera");

            RaycastHit tHit = new RaycastHit();
            if (Physics.Raycast(tSpawn.position, tSpawn.forward, out tHit, 1000f, canBePinged))
            {
                GameObject tNewPing = Instantiate(pingDefendingPrefab, tHit.point + tHit.normal * 0.001f, Quaternion.identity) as GameObject;
                tNewPing.transform.LookAt(tHit.point + tHit.normal);
                Destroy(tNewPing, 5f);
            }
        }

     /**   [PunRPC]
        private void Grenade()
        {

            Debug.Log("'Murica");

            GameObject tNewGrenade = Instantiate(grenadePrefab, currentWeapon.transform.position, Quaternion.identity) as GameObject;
            Rigidbody r = tNewGrenade.GetComponent<Rigidbody>();
            r.AddForce(transform.forward * throwForce);
            Destroy(tNewGrenade, 5f);
        } */
        

        [PunRPC]
        private void Shoot()
        {
            Debug.Log("Shooting");
            Transform tSpawn = transform.Find("Cameras/Player Camera");
            
            currentCooldown = loadout[currentInd].firerate;

            RaycastHit tHit = new RaycastHit();
            if (Physics.Raycast(tSpawn.position, tSpawn.forward, out tHit, 1000f, canBeShot))
            {
                GameObject tNewHole =
                    Instantiate(bulletholePrefab, tHit.point + tHit.normal * 0.001f,
                        Quaternion.identity) as GameObject;
                tNewHole.transform.LookAt(tHit.point + tHit.normal);
                Destroy(tNewHole, 5f);

                if (photonView.IsMine)
                {
                    //shooting a player
                    if (tHit.collider.gameObject.layer == 11)
                    {
                        tHit.collider.transform.root.gameObject.GetPhotonView().RPC("takeDamage", RpcTarget.All, loadout[currentInd].damage);
                        Debug.Log("Shot another Player");
                    }
                }
            }

            //gun kickback
            currentWeapon.transform.Rotate(-loadout[currentInd].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentInd].kickback;
        }

        [PunRPC]
        private void takeDamage(int pDamage)
        {
            GetComponent<PlayerController>().takeDamage(pDamage);
        }


        private void Aim(bool pIsAiming)
        {
            isAiming = pIsAiming;
            Transform tAnchor = currentWeapon.transform.Find("Anchor"); 
            Transform tAnchorStateADS = currentWeapon.transform.Find("State/ADS");
            Transform tAnchorStateHip = currentWeapon.transform.Find("State/Hip");
            
            if (pIsAiming)
            {
                tAnchor.position = Vector3.Lerp(tAnchor.position, tAnchorStateADS.position, Time.deltaTime * loadout[currentInd].aimSpeed); 
                crosshair.enabled = false;
            }
            else
            {
                tAnchor.position = Vector3.Lerp(tAnchor.position, tAnchorStateHip.position, Time.deltaTime * loadout[currentInd].aimSpeed);
                crosshair.enabled = true;
            }
        }

        [PunRPC]
        void Equip(int pInd)
        {
            if (currentWeapon != null)
            {
               if(isReloading) StopCoroutine("Reload");
                Destroy(currentWeapon);
            }

            currentInd = pInd;
            GameObject tNewWeapon = Instantiate(loadout[pInd].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            tNewWeapon.transform.localPosition = Vector3.zero;
            tNewWeapon.transform.localEulerAngles = Vector3.zero;
            tNewWeapon.GetComponent<Sway>().isMine = photonView.IsMine;

            currentWeapon = tNewWeapon;
        }

        public void RefreshAmmo(Text pText)
        {
            int tClip = loadout[currentInd].GetClip();
            int tStash = loadout[currentInd].GetStash();

            pText.text = tClip.ToString("D2") + " / " + tStash.ToString("D2");
        }

    }
}