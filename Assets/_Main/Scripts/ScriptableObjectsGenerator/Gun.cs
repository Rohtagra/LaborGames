using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.HHN.FPSGame.Character
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        public string name;
        public GameObject prefab;
        public float firerate;
        public float recoil;
        public float kickback;
        public float aimSpeed;
        public int damage;
        public int ammo;
        public int clipSize;
        public float reload;
        public int burst; // 0 semi | 1 auto | 2+ burst fire

        private int stash; //current ammo
        private int clip; //current clip

        public void init()
        {
            stash = ammo;
            clip = clipSize;
        }


        public bool FireBullet()
        {
            if (clip > 0)
            {
                clip -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reload()
        {
            stash += clip;
            clip = Mathf.Min(clipSize, stash);
            stash -= clip;
        }

        public int GetStash()
        {
            return stash;
        }

        public int GetClip()
        {
            return clip;
        }

    }
}