using System;
using Assets._Main.Scripts.ScriptableObjectsGenerator.UI;
using UnityEngine;

namespace Assets._Main.Scripts
{
    public class RingMenu : MonoBehaviour
    {

        public Ring Data;

        public RingCakePiece RingCakePiecePrefab;

        public float GapWidthDegree = 1f;

        public Action<string> callback;

        protected RingCakePiece[] Pieces;

        protected RingMenu Parent;
        [HideInInspector]
        public string Path;

        private Vector3 startPosition;
        // Start is called before the first frame update
        void Start()
        {
            var stepLength = 360f / Data.Elements.Length;
            var iconDist = Vector3.Distance(RingCakePiecePrefab.Icon.transform.position, RingCakePiecePrefab.CakePiece.transform.position);

            Pieces = new RingCakePiece[Data.Elements.Length];
            for (int i = 0; i < Data.Elements.Length; i++)
            {
                Debug.Log("Instantiating");
                Pieces[i] = Instantiate(RingCakePiecePrefab, transform);
                Pieces[i].transform.localPosition = Vector3.zero;
                Pieces[i].transform.localRotation = Quaternion.identity;

                //set cake piece
                Pieces[i].CakePiece.fillAmount = 1f / Data.Elements.Length - GapWidthDegree / 360f;
                Pieces[i].CakePiece.transform.localPosition = Vector3.zero;
                Pieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0, 0, -stepLength / 2f + GapWidthDegree / 2f + i * stepLength);
                Pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

                //set icon
                Pieces[i].Icon.transform.localPosition = Pieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDist;
                Pieces[i].Icon.sprite = Data.Elements[i].Icon;
            }
            startPosition = Input.mousePosition;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mouseDelta = Input.mousePosition - startPosition; 

            if (mouseDelta.sqrMagnitude < 0.1f)
            {
                return; // don't do tiny rotations.
            }

            float angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
            angle = (angle + 360f) % 360f;
        //    Debug.Log(angle);



            var stepLength = 360f / Data.Elements.Length;
      //      var mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - transform.position, Vector3.forward) + stepLength / 2f);
            var activeElement = (int)(angle / stepLength);
            Debug.Log("Active Element" + activeElement);
            for (int i = 0; i < Data.Elements.Length; i++)
            {
                if (i == activeElement)
                    Pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.75f);
                else
                    Pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);
            }


            if (Input.GetKeyUp(KeyCode.F))
            {
                var path = Path + "/" + Data.Elements[activeElement].Name;
                if (Data.Elements[activeElement].NextRing != null)
                {
                    var newSubRing = Instantiate(gameObject, transform.parent).GetComponent<RingMenu>();
                    newSubRing.Parent = this;
                    for (var j = 0; j < newSubRing.transform.childCount; j++)
                        Destroy(newSubRing.transform.GetChild(j).gameObject);
                    newSubRing.Data = Data.Elements[activeElement].NextRing;
                    newSubRing.Path = path;
                    newSubRing.callback = callback;
                }
                else
                {
                    Debug.Log("Invoking...");
                    callback?.Invoke(path);
                    Destroy(this);
                }

                gameObject.SetActive(false);
            }
        }

        private float NormalizeAngle(float a) => (a + 360f) % 360f;
    }
}
