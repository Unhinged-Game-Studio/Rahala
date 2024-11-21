using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public Image firstImage;  // First image in the cutscene
    public Image secondImage; // Second image in the cutscene
    public TextMeshProUGUI bottomText; // Text at the bottom of the screen
    public float transitionDuration = 3.0f; // Duration of the transition
    public string firstText = "Welcome to the cutscene!"; // Text for the first image
    public string secondText = "Here comes the next part!"; // Text for the second image
    public GameObject[] uiElementsToHide;  // List of other UI elements to hide during the cutscene

    private void Start()
    {
        // Start the cutscene when the game begins
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // Hide all non-cutscene UI elements
        HideUIElements();

        // Ensure firstImage is fully visible and secondImage is transparent at the start
        SetImageAlpha(firstImage, 1);
        SetImageAlpha(secondImage, 0);

        // Set the initial text
        bottomText.text = firstText;

        // Wait for 2 seconds before transitioning
        yield return new WaitForSeconds(2);

        // Fade out the first image
        yield return StartCoroutine(FadeOutImage(firstImage));

        // Optional: Add a small delay between transitions
        yield return new WaitForSeconds(0.5f);

        // Fade in the second image
        yield return StartCoroutine(FadeInImage(secondImage));

        // Update the text after the image transition
        bottomText.text = secondText;

        // After the cutscene ends, show the UI elements again
        ShowUIElements();
    }

    // Fade out the image
    private IEnumerator FadeOutImage(Image img)
    {
        Color color = img.color;
        float timer = 0;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer / transitionDuration);
            img.color = color;
            yield return null; // Wait for the next frame
        }
    }

    // Fade in the image
    private IEnumerator FadeInImage(Image img)
    {
        Color color = img.color;
        float timer = 0;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / transitionDuration);
            img.color = color;
            yield return null; // Wait for the next frame
        }
    }

    // Set the alpha value of an image (useful for initialization)
    private void SetImageAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }

    // Hide UI elements (called at the beginning of the cutscene)
    private void HideUIElements()
    {
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(false);
        }
    }

    // Show UI elements (called after the cutscene)
    private void ShowUIElements()
    {
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(true);
        }
    }
}