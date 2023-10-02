using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float rotationSpeed = 0.5f;

    /// <summary>
    /// InputAction that produces a Vector2 value for movement
    /// </summary>
    [SerializeField]
    private InputActionReference movementAction;

    [SerializeField]
    private InputActionReference lookAround;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var direction = movementAction.action.ReadValue<Vector2>();
        if (direction != Vector2.zero)
        {
            Vector3 movement = new Vector3(direction.x, 0f, direction.y);
            transform.Translate(movement * Time.deltaTime * speed, Space.Self);
        }

        var look = lookAround.action.ReadValue<Vector2>();
        if (look != Vector2.zero)
        {
            Vector3 rotation = new Vector3(0f,look.x, 0f);
            transform.Rotate(rotation * Time.deltaTime * rotationSpeed, Space.Self);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player");
        }
    }
}
