using UnityEngine;

public class Cutscenes : MonoBehaviour
{
    [SerializeField] CutsceneController[] _managers;
    private int _index = 0;
    private float _elapsedTime = 0;

    void Start()
    {
        _managers[0].StartCutscene();
        _index++;
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > 15 )
        {
            if (_index < _managers.Length)
                _managers[_index].StartCutscene();
            _index++;
            _elapsedTime = 0;
        }
    }
}
