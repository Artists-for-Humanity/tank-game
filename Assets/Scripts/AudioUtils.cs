using UnityEngine;

public static class AudioUtils
{
    public static AudioSource PlayClipAt(AudioClip audioClip, Vector3 position)
    {
        GameObject temp = new GameObject();
        temp.transform.position = position;

        AudioSource audioSource = temp.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource.clip = audioClip;

        audioSource.Play();

        GameObject.Destroy(temp, audioClip.length);

        return audioSource;

    }
}
