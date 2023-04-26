using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private Vector2 direction;
    public Vector2 mouseWorldPos;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = direction * speed;

        Vector2 facingDiretion = mouseWorldPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(facingDiretion.y,facingDiretion.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);
    }

    public void OnMousePos()
    {

    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void OnMousePos(InputAction.CallbackContext context)
    {
        mouseWorldPos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }
}
