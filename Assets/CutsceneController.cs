using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using DG.Tweening;


public class CutsceneController : MonoBehaviour
{
    public Image cutsceneImage; // Reference to the UI Image
    public TextMeshProUGUI cutsceneText; // Reference to the TextMeshPro component

    public Sprite[] pictures; // Array of pictures for the cutscene
    public string[] texts; // Array of texts corresponding to each picture
    public float transitionTime = 3f; // Time to display each picture

    private int currentIndex = 0; // To track the current picture

    [SerializeField]
    private CanvasGroup canvasGroup;


    [SerializeField]
    private AudioSource audioSource;

    // void Start()
    // {
        // Start the cutscene
        // if (pictures.Length > 0 && texts.Length > 0)
        //     StartCoroutine(PlayCutscene());
    // }


    public void StartCutscene()
    {

        audioSource.Play();
        canvasGroup.DOFade(1, 1);
        
        if (pictures.Length > 0 && texts.Length > 0)
            StartCoroutine(PlayCutscene());
    }

    System.Collections.IEnumerator PlayCutscene()
    {
        while (currentIndex < pictures.Length)
        {
            // Set the picture and text
            cutsceneImage.sprite = pictures[currentIndex];
            cutsceneText.text = texts[currentIndex];

            // Wait for the transition time
            yield return new WaitForSeconds(transitionTime);

            currentIndex++;
        }

        // Optionally, trigger the next scene or event after the cutscene
        Debug.Log("Cutscene Ended!");
        audioSource.Stop();
        canvasGroup.DOFade(0, 1);
    }
}
