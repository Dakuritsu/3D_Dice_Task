using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dice : MonoBehaviour
{   
    [System.Serializable]
    private struct DiceFace
    {
        public int value;
        public Vector3 localDirection;
    }

    
    private readonly DiceFace[] faces =
    {
        new DiceFace{value = 5, localDirection = Vector3.up},
        new DiceFace{value = 2, localDirection = Vector3.down},
        new DiceFace{value = 4, localDirection = Vector3.right},
        new DiceFace{value = 3, localDirection = Vector3.left},
        new DiceFace{value = 1, localDirection = Vector3.forward},
        new DiceFace{value = 6, localDirection = Vector3.back}
    };

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

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
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

        Debug.Log("Dice stopped");

        int result = GetTopFace();
        Debug.Log($"Result: {result}");

        isRolling = false;
    }

    private int GetTopFace()
    {
        int topValue = 0;
        float bestAlignment = -1f;

        foreach(DiceFace face in faces)
        {   
            Vector3 worldDirection = transform.TransformDirection(face.localDirection);
            float alignment = Vector3.Dot(worldDirection, Vector3.up);
            
            if(alignment > bestAlignment)
            {
                topValue = face.value;
                bestAlignment = alignment;
            }
        }
        return topValue;
    }
}
