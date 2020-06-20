using Photon.Pun;
using UnityEngine;

namespace Assets._Main.Prefabs.Grenade
{
    public class Grenade : MonoBehaviourPunCallbacks
    {

        public float delay;
        public float explosionRadius;
        public LayerMask localPlayerLayer;
        public LayerMask PlayerLayer;
        public GameObject explosionPrefab;
        private GameObject explosion;
        // Start is called before the first frame update
        void Start()
        {
            Invoke("explode", delay);    
        }


        void explode()
        {
            Debug.Log("explode");
                Debug.Log("Looking for Colliders");
                if (photonView.IsMine)
                {

                    Debug.Log("Is mine");
                Collider[] hitColliders =
                        Physics.OverlapSphere(transform.position, explosionRadius, PlayerLayer);
                    foreach (Collider c in hitColliders)
                    {

                        Debug.Log("Scanned");
                        Scanable scanable = c.GetComponent<Scanable>();
                        scanable.Scanned();

                    }
                }else
                {

                    Debug.Log("Is not mine");
                Collider[] hitColliders =
                        Physics.OverlapSphere(transform.position, explosionRadius, localPlayerLayer);
                    foreach (Collider c in hitColliders)
                    {

                        Debug.Log("Scanned");
                        Scanable scanable = c.GetComponent<Scanable>();
                        scanable.Scanned();

                    }
            }

                // gameObject.GetComponent<AudioSource>().Play();
            explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;

            Invoke("kill", 3f);
        }


        void kill()
        {
            Destroy(explosion);
            Destroy (gameObject);
        }
    }
}
