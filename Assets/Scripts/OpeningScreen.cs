using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScreen : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainGame"); // Replace "MainGame" with your actual scene name
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartGame();
        }
    }
}
