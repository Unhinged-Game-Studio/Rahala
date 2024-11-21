using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }
    private PlayerStats _playerStats;
    private float _coinElapsedTime = 0f;
    private float _coinDisappearTime = 2f;
    private float _staminaElapsedTime = 0f;
    private float _staminaDisappearTime = 2f;
    private bool _coinActivating = false;
    private bool _instructionActivating = false;
    private bool _staminaActivating = false;
    private string[] _Names = {"Spain", "Egypt", "Iraq", "Saudi Arabia", "India", "China"};
    private int _index = 0;
    [SerializeField] GameObject _coinDisplay;
    [SerializeField] TextMeshProUGUI _coinText;
    [SerializeField] GameObject _instructionsPanel;
    [SerializeField] GameObject _staminaMeter;
    [SerializeField] Slider _staminaSlider;
    [SerializeField] TextMeshProUGUI _country;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        _playerStats = PlayerStats.Instance;
        _coinDisplay.GetComponent<CanvasGroup>().alpha = 0;
        _coinDisplay.SetActive(false);
        _staminaMeter.GetComponent<CanvasGroup>().alpha = 0;
        _staminaMeter.SetActive(false);
        _country.text = _Names[_index];
    }

    void Update()
    {
        _coinElapsedTime += Time.deltaTime;
        _staminaElapsedTime += Time.deltaTime;
        if (_coinElapsedTime > _coinDisappearTime && _coinActivating)
        {
            _coinDisplay.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => {
                deactivateCoin();
            });
        }
        if (_staminaElapsedTime > _staminaDisappearTime && _staminaActivating)
        {
            _staminaMeter.GetComponent<CanvasGroup>().DOFade(0,0.5f).OnComplete(() => {
                deactivateStamina();
            });
        }
    }

    private void deactivateCoin()
    {
        _coinDisplay.SetActive(false);
        _coinActivating = false;
    }

    private void deactivateStamina()
    {
        _staminaMeter.SetActive(false);
        _staminaActivating = false;
    }

    public void displayCoin()
    {
        _coinElapsedTime = 0f;
        _coinDisplay.SetActive(true);
        if (!_coinActivating)
        {
            _coinDisplay.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            _coinActivating = true;
        }
        _coinText.text = _playerStats.getCoinsCollected().ToString();
    }

    public void showInstructions()
    {
        if (!_instructionActivating)
        {
            _instructionsPanel.GetComponent<RectTransform>().DOAnchorPosX(130, 0.5f, false);
            _instructionActivating = true;
        }
        else if (_instructionActivating)
        {
            _instructionsPanel.GetComponent<RectTransform>().DOAnchorPosX(-60, 0.5f, false);
            _instructionActivating = false;
        }
    }

    public void updateStamina()
    {
        _staminaSlider.value = _playerStats.getStaminaMeter();
        _staminaElapsedTime = 0f;
        if (!_staminaActivating)
        {
            _staminaMeter.SetActive(true);
            _staminaMeter.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            _staminaActivating = true;
        }
    }

    public void updateQuest()
    {
        _index++;
        _country.text = _Names[_index];
    }
}
