using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScore : MonoBehaviour
{

    // This usually done with a singleton pattern, in this case will pass directly the reference
    [SerializeField]
    private Gameplay _gameplay;

    private TMPro.TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TMPro.TextMeshProUGUI>();
        if (_text == null)
        {
            Debug.LogError("No TextMeshProUGUI component found");
            return;
        }

        if (_gameplay == null)
        {
            Debug.LogError("We wont be able to update the score at the UI, no Gameplay reference");
        }
        _gameplay.scoreChanged += UpdateScores;
        _gameplay.maxScoreChanged += UpdateScores;
    }


    // We dont really need _score, in this case because we keep simple, but in other cases a better isolation around the score properties could be done.
    void UpdateScores(int _score)
    {
        _text.text = _gameplay.Score.ToString() + " / " + _gameplay.MaxScore.ToString();
    }

}
