using System.Collections;
using System.Collections.Generic;
using Com.HHN.FPSGame.Character;
using UnityEngine;

namespace Com.HHN.FPSGame
{
    public class MainMenu : MonoBehaviour
{

    public Launcher launcher;
    public void JoinMatch()
    {
        launcher.Join();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreateMatch()
    {
        launcher.Create();
    }
}

}