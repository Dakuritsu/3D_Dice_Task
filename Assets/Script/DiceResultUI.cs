using TMPro;
using UnityEngine;

public class DiceResultUI : MonoBehaviour
{
    [SerializeField]
    private Transform dice;

    [SerializeField]
    private TMP_Text resultText;

    [SerializeField]
    private Vector3 offset = new Vector3(0f, 2f, 0f);

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        resultText.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        transform.position = dice.position + offset;

        if(mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0f, 180f, 0f);
        }
    }

    public void ShowResult(int value)
    {
        resultText.text = value.ToString();
        resultText.gameObject.SetActive(true);
    }

    public void HideResult()
    {
        resultText.gameObject.SetActive(false);
    }
}
