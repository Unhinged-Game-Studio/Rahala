using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    private static Compass _instance;
    public static Compass Instance { get { return _instance; } }
    public GameObject IconPrefab;
    List<QuestMarker> questMarkers = new List<QuestMarker>();
    public RawImage compassImage;
    public GameObject shipCamera;
    public GameObject camelCamera;
    private Transform player;

    public float maxDistance = 1000f;

    float compassUnit;

    public QuestMarker[] countries;
    private GameObject _reference;
    private int _index = 0;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        compassUnit = compassImage.rectTransform.rect.width / 360f;
    
        AddQuestMarker(countries[0]);
    }

    private void Update()
    {
        if (shipCamera.activeInHierarchy)
            player = shipCamera.transform;
        else if (camelCamera.activeInHierarchy)
            player = camelCamera.transform;
        compassImage.uvRect = new Rect (player.localEulerAngles.y / 360f, 0f, 1f, 1f);
        foreach (QuestMarker marker in questMarkers)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);

            float dst = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), marker.position);
            float scale = 0f;

            if (dst < maxDistance)
                scale = 1f - (dst / maxDistance);
            if (scale < 0.1f)
                scale = 0.5f;
            
            marker.image.rectTransform.localScale = Vector3.one * scale;
        }
    }

    public void AddQuestMarker(QuestMarker marker)
    {
        GameObject newMarker = Instantiate(IconPrefab, compassImage.transform);
        _reference = newMarker;
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        questMarkers.Add(marker);
    }

    public void RemoveQuestMarker()
    {
        Destroy(_reference);
        questMarkers.RemoveAt(0);
        _index++;
        if (_index < countries.Length)
            AddQuestMarker(countries[_index]);
    }

    Vector2 GetPosOnCompass (QuestMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);
    
        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);
        return new Vector2(compassUnit * angle, 0f);
    }
}
