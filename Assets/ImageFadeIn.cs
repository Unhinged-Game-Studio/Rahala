using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFadeIn : MonoBehaviour
{
    public Image[] images; // Images to fade in
    public float fadeDuration = 2.0f; // Duration for each fade

    void Start()
    {
        StartCoroutine(FadeImagesSequentially());
    }

    IEnumerator FadeImagesSequentially()
    {
        foreach (Image img in images)
        {
            Color color = img.color;
            color.a = 0; // Fully transparent
            img.color = color;

            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0, 1, timer / fadeDuration); // Gradually fade in
                img.color = color;
                yield return null;
            }
            yield return new WaitForSeconds(1); // Wait before fading in the next image
        }
    }
}
