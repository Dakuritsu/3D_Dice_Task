using UnityEngine;
using UnityEngine.InputSystem;

public class Dice : MonoBehaviour
{   
    [SerializeField] private float upForce = 3f, moveForce = 2f, rotationForce  = 10f;

    private Rigidbody rb;
    private bool isRolling;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Mouse.current != null)
        {
            if(Mouse.current.leftButton.wasPressedThisFrame && !isRolling)
            {
                Roll();
            }
        }

    }

    private void Roll()
    {
        isRolling = true;
        
        Vector3 diceDirection = new Vector3(Random.Range(-moveForce, moveForce), upForce, Random.Range(-moveForce, moveForce));
        Vector3 torqueDirection = new Vector3(Random.Range(-rotationForce , rotationForce ),Random.Range(-rotationForce , rotationForce ),Random.Range(-rotationForce , rotationForce ));
        
        rb.AddForce(diceDirection, ForceMode.Impulse);
        rb.AddTorque(torqueDirection, ForceMode.Impulse);

        
    }
}
