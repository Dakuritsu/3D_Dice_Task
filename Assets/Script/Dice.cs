using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dice : MonoBehaviour
{   
    [SerializeField] private float upForce = 3f, moveForce = 2f, rotationForce  = 10f;
    [SerializeField] private float velocityThreshold = 0.05f, rotationThreshold = 0.05f;

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

        Debug.Log("Dice launched");
        
        StartCoroutine(WaitForDiceToStop());
    }

    private IEnumerator WaitForDiceToStop()
    {
        yield return new WaitForSeconds(2f);

        while(rb.linearVelocity.magnitude > velocityThreshold || rb.angularVelocity.magnitude > rotationThreshold)
        {
            yield return null;
        }

        isRolling = false;

        Debug.Log("Dice stopped");
    }
}
