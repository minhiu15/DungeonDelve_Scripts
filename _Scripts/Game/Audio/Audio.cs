using FMODUnity;
using UnityEngine;

public class Audio : MonoBehaviour
{
    [SerializeField] private new EventReference audio;
    public void Play() => AudioManager.PlayOneShot(audio, transform.position);
}
