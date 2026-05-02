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

    [SerializeField] private float minUpForce = 2f;
    [SerializeField] private float maxUpForce = 4f;

    [SerializeField] private float minMoveForce = 3f;
    [SerializeField] private float maxMoveForce = 6f;

    [SerializeField] private float minRotationForce = 2f;
    [SerializeField] private float maxRotationForce = 5f;

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
        resultUI.HideResult();

        ResetPhysics();

        Vector3 throwDirection = CreateThrowDirection();
        Vector3 tumbleTorque = CreateTumbleTorque();

        rb.AddForce(throwDirection, ForceMode.Impulse);
        rb.AddTorque(tumbleTorque, ForceMode.Impulse);

        StartCoroutine(WaitForDiceToStop());
    }

    private void ResetPhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Create a random throw direction for the dice
    private Vector3 CreateThrowDirection()
    {
        return new Vector3(
            RandomForce(minMoveForce, maxMoveForce),
            Random.Range(minUpForce, maxUpForce),
            RandomForce(minMoveForce, maxMoveForce)
        );
    }

    // Return a random force value
    private float RandomForce(float minForce, float maxForce)
    {
        float force = Random.Range(minForce, maxForce);

        return (Random.value < 0.5f) ? -force : force;
    }

    // Add extra rotation
    private Vector3 CreateTumbleTorque()
    {
        return new Vector3(RandomForce(minRotationForce, maxRotationForce), 0, RandomForce(minRotationForce, maxRotationForce));
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

        ResetPhysics();

        rb.position = startPosition;
        rb.rotation = startRotation;

        resultUI.HideResult();
        
    }
}
