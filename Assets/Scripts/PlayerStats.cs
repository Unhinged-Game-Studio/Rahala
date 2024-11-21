using System.Timers;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStats : MonoBehaviour
{
    private static PlayerStats _instance;
    public static PlayerStats Instance { get { return _instance; } }
    private UIManager _uiManager;
    private float _elapsedTime = 0f;
    private int _coinsCollected = 0;
    private int _numberOfLandmarks = 0;
    private float _staminaMeter = 100f;

    public AudioSource coinsound;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        _uiManager = UIManager.Instance;
    }

    void Update()
    {

    }

    public void isSprinting()
    {
        _elapsedTime = 0f;
        if (_staminaMeter > 0f)
            _staminaMeter -= 0.5f;
    }

    public void isNotSprinting()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 2f && _staminaMeter < 100f)
            _staminaMeter += 2f;
    }

    public void coinCollected(int amount)
    {
        _coinsCollected += amount;
        _uiManager.displayCoin();
        coinsound.Play();
    }
    public void coinloss(int amount)
    {
        _coinsCollected = Mathf.Clamp(_coinsCollected - amount, 0, 100000);
        _uiManager.displayCoin();
    }

    public float getCoinsCollected()
    {
        return _coinsCollected;
    }

    public float getNumberOfLandmarks()
    {
        return _numberOfLandmarks;
    }

    public float getStaminaMeter()
    {
        return _staminaMeter;
    }
}
