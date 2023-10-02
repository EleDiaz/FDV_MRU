using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    // This usually done with a singleton pattern, in this case will pass directly the reference
    [SerializeField]
    private Gameplay _gameplay;

    [SerializeField]
    private GameObject obstacle;

    [SerializeField]
    private Transform initialOrigin;

    [SerializeField]
    private float innerRadius = 5f;

    [SerializeField]
    private float outerRadius = 50f;

    [SerializeField]
    private int amount = 25;

    // Start is called before the first frame update
    void Start()
    {
        if (obstacle == null)
        {
            Debug.LogError("No obstacle assigned to GenerateMap");
            return;
        }

        if (_gameplay == null)
        {
            Debug.LogError("We wont be able to update the MAX score, no Gameplay reference");
        }
        _gameplay.MaxScore = amount;

        for (int i = 0; i < amount; i++)
        {
            var distance = Random.Range(innerRadius, outerRadius);
            var angle = Random.Range(0, 360f);
            var position = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * distance + initialOrigin.position;

            var gameObject = Instantiate(obstacle, position, Quaternion.identity);
            gameObject.name = "Obstacle " + i;
            gameObject.GetComponent<PlayerReaction>()._gameplay = _gameplay;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
