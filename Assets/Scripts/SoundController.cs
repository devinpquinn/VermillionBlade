using UnityEngine;

public class SoundController : MonoBehaviour
{
	/// <summary>
	/// Plays an AudioClip from Resources as a one shot with optional volume and pitch.
	/// </summary>
	/// <param name="clipName">Name of the AudioClip in Resources.</param>
	/// <param name="volume">Volume of the clip (default 1.0f).</param>
	/// <param name="pitch">Pitch of the clip (default 1.0f).</param>
	public static void PlayOneShot(string clipName, float volume = 1.0f, float pitch = 1.0f)
	{
		AudioClip clip = Resources.Load<AudioClip>(clipName);
		if (clip == null)
		{
			Debug.LogWarning($"SoundController: AudioClip '{clipName}' not found in Resources.");
			return;
		}
		// Create a temporary GameObject to play the sound
		GameObject tempGO = new GameObject("TempAudio");
		AudioSource aSource = tempGO.AddComponent<AudioSource>();
		aSource.clip = clip;
		aSource.volume = volume;
		aSource.pitch = pitch;
		aSource.Play();
		Object.Destroy(tempGO, clip.length / Mathf.Abs(pitch));
	}
}
