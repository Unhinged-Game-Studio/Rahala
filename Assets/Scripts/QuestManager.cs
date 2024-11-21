using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] GameObject[] _destinations;
    private static QuestManager _instance;
    public static QuestManager Instance { get { return _instance; } }
    private int _index = 0;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        _destinations[0].SetActive(true);
        for (int i = 1; i < _destinations.Length; i++)
            _destinations[i].SetActive(false);
    }

    public void nextDestination()
    {
        _index++;
        if (_index < _destinations.Length)
            _destinations[_index].SetActive(true);
        //else
            //Player Ending cinematic
    }
}
