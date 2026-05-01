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

    [SerializeField]
    private float upForce = 3f, moveForce = 2f, rotationForce  = 3f;

    [SerializeField]
    private float velocityThreshold = 0.05f, rotationThreshold = 0.03f;

    [SerializeField]
    private DiceResultUI resultUI;

    private readonly DiceFace[] faces =
    {
        new DiceFace{value = 5, localDirection = Vector3.up},
        new DiceFace{value = 2, localDirection = Vector3.down},
        new DiceFace{value = 4, localDirection = Vector3.right},
        new DiceFace{value = 3, localDirection = Vector3.left},
        new DiceFace{value = 1, localDirection = Vector3.forward},
        new DiceFace{value = 6, localDirection = Vector3.back}
    };
    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isRolling;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        startPosition = rb.position;
        startRotation = rb.rotation;

        resultUI.HideResult();

        isRolling = false;
    }

    void Update()
    {
        if(Mouse.current == null) return;
        
        if(Mouse.current.leftButton.wasPressedThisFrame && !isRolling)
        {
            Roll();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ResetDice();
        }
    }

    // Apply random impulse and torque
    private void Roll()
    {
        isRolling = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        resultUI.HideResult();
        
        Vector3 diceDirection = new Vector3(Random.Range(-moveForce, moveForce), upForce, Random.Range(-moveForce, moveForce));
        Vector3 torqueDirection = new Vector3(Random.Range(-rotationForce , rotationForce ),Random.Range(-rotationForce , rotationForce ),Random.Range(-rotationForce , rotationForce ));
        
        rb.AddForce(diceDirection, ForceMode.Impulse);
        rb.AddTorque(torqueDirection, ForceMode.Impulse);

        // Debug.Log("Dice launched");
        
        StartCoroutine(WaitForDiceToStop());
    }

    private IEnumerator WaitForDiceToStop()
    {
        yield return new WaitForSeconds(2.5f);

        while(rb.linearVelocity.magnitude > velocityThreshold || rb.angularVelocity.magnitude > rotationThreshold)
        {
            yield return null;
        }

        int result = GetTopFace();

        isRolling = false;

        resultUI.ShowResult(result);
    }

    // Determine which face is aligned with world up
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

    private void ResetDice()
    {
        StopAllCoroutines();

        isRolling = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = startPosition;
        rb.rotation = startRotation;

        resultUI.HideResult();
        
    }
}
