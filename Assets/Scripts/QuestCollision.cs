using UnityEngine;

public class QuestCollision : MonoBehaviour
{
    [SerializeField] GameObject _Location;
    private QuestManager _questManager;
    private Compass _compass;
    private UIManager _uiManager;
    [SerializeField] CutsceneController cutsceneController;

    void Start()
    {
        _questManager = QuestManager.Instance;
        _compass = Compass.Instance;
        _uiManager = UIManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _questManager.nextDestination();
            _compass.RemoveQuestMarker();
            _uiManager.updateQuest();
            //Replace this with showing the cutscene
            _Location.SetActive(false);
            this.gameObject.SetActive(false);
            cutsceneController.StartCutscene();
        }
    }
}
