using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float radiusSeparation = 1f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned to SimpleFollow");
            return;
        }
        var direction = target.transform.position - transform.position;

        if (direction.magnitude < radiusSeparation)
        {
            return;
        }


        transform.Translate(direction.normalized * Time.deltaTime * speed, Space.World);
    }
}
