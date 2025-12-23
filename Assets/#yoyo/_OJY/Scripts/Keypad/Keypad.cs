using UnityEngine;

enum pad 
{
    number,
    reset,
    ok
}

public class Keypad : MonoBehaviour
{
    [Header("키패드 본체")]
    [SerializeField] TouchKeypad touchkeypad;

    [Header("번호")]
    [SerializeField] int keypadNumber;

    [Header("패드 종류")]
    [SerializeField] pad padInfo;

    bool bPressed;
    Collider pressObj;

    private void Awake()
    {
        bPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (touchkeypad == null || bPressed) return;

        if (other.CompareTag("FingerTip"))
        {
            bPressed = true;
            pressObj = other;

            switch (padInfo)
            {
                case pad.number:
                    touchkeypad.SetNumber(keypadNumber);
                    break;
                case pad.reset:
                    touchkeypad.ResetNumber();
                    break;
                case pad.ok:
                    touchkeypad.SubmitNumber();
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == pressObj)
        {
            bPressed = false;
            pressObj = null;
        }
    }
}
