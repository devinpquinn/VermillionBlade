using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public void PlaySound(string soundName)
    {
        SoundController.PlayOneShot(soundName);
    }
}
