using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.HHN.FPSGame.Character
{
    public class GameManager : MonoBehaviour
    {
        public string playerPrefab;
        public Transform[] spawnPoints;

        private void Start()
        {
            Spawn();
        }


        public void Spawn()
        {
            Transform tSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            PhotonNetwork.Instantiate(playerPrefab, tSpawn.position, tSpawn.rotation);
        }
    }
}