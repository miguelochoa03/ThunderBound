using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    // when you click start, it takes to the game scene
    public void OnStartClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}
