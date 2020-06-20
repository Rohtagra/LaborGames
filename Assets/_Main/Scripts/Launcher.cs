using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.HHN.FPSGame.Character
{
    public class Launcher : MonoBehaviourPunCallbacks
    {

        public void Start()
        {
            OnEnable1();
        }

        public void OnEnable1()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            Connect();
            
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected successfully");
            base.OnConnectedToMaster();
        }

        public override void OnJoinedRoom()
        {
            StartGame();

            base.OnJoinedRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Create();

            base.OnJoinRandomFailed(returnCode, message);
        }

        public void Connect()
        {
            Debug.Log("Connecting");
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Still connecting");
        }

        public void Join()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void Create()
        {
            PhotonNetwork.CreateRoom("");
        }


        public void StartGame()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
    }
}