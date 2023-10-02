using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerReaction : MonoBehaviour
{

    // This usually done with a singleton pattern, in this case will pass directly the reference
    [SerializeField]
    public Gameplay _gameplay;

    public bool _playerCollision = false;

    private MeshRenderer _meshRenderer;


    // Start is called before the first frame update
    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        if (_gameplay == null) {
            Debug.LogError("We wont be able to update the score, no Gameplay reference");
        }

        // Nice blueish-violetish colour
        var color = Color.HSVToRGB(Random.Range(190f, 280f) / 360f, 1.0f, Random.Range(0.5f, 1.0f));
        _meshRenderer.material.color = color;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !_playerCollision)
        {
            _playerCollision = true;
            Debug.Log("There is a collision between Player and " + other.gameObject.name);
            _meshRenderer.material.color = Color.red;

            if (_gameplay != null)
            {
                _gameplay.Score += 1;
            }
        }
    }
}
