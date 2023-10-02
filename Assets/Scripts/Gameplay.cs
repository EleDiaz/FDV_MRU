using UnityEngine;
using UnityEngine.InputSystem;

public class Gameplay : MonoBehaviour
{

    public int Score
    {
        get { return _score; }
        set
        {
            scoreChanged?.Invoke(value);
            _score = value;
        }
    }

    public int MaxScore
    {
        get { return _maxScore; }
        set
        {
            maxScoreChanged?.Invoke(value);
            _maxScore = value;
        }
    }

    private int _score = 0;
    public int _maxScore = 0;

    public delegate void ScoreChange(int score);

    public event ScoreChange scoreChanged;

    public event ScoreChange maxScoreChanged;


    // We obtain the active InputActionAsset to enable it
    [SerializeField]
    private InputActionAsset inputActionsActive;

    void Start()
    {
        inputActionsActive.Enable();
    }

    void Update()
    {

    }
}
