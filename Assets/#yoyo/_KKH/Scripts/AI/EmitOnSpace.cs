using UnityEngine;

public class EmitOnSpace : MonoBehaviour
{
    public SoundEmitter emitter;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && emitter != null)
        {
            Debug.Log("Emit sound on space key pressed.");
            emitter.Emit();
        }
    }
}