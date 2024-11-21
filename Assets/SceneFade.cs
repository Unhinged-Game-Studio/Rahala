using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFade : MonoBehaviour
{
    public Image fadeImage; // Reference to the Image UI element
    public float fadeDuration = 1f; // Duration of the fade-out effect

    private bool isFading = false;

    void Start()
    {
        // Ensure the image starts fully transparent
        fadeImage.color = new Color(0f, 0f, 0f, 0f); // Black color with 0 alpha
    }

    // Call this method to trigger the fade-out and scene transition
    public void StartFadeOut(string sceneToLoad)
    {
        if (isFading)
            return; // Prevent multiple triggers while fading

        isFading = true;
        StartCoroutine(FadeOutAndLoadScene(sceneToLoad));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        // Fade to black (alpha = 1)
        float timeElapsed = 0f;

        // Fade in (to black)
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            fadeImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(timeElapsed / fadeDuration));
            yield return null;
        }

        // Now that the fade is complete, load the next scene
        SceneManager.LoadScene(sceneName);
    }
}
