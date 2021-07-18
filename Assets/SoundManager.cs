using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Dictionary<string,AudioSource> sounds;

    void Start()
    {
        sounds = new Dictionary<string, AudioSource>();
        foreach (AudioSource source in Component.FindObjectsOfType<AudioSource>())
        {
            sounds.Add(source.clip.name, source);
        }
    }

    public void PlaySound(string clipName)
    {
        CheckClipName(clipName);

        sounds[clipName].Play();
    }

    public void StopSound(string clipName)
    {
        CheckClipName(clipName);

        sounds[clipName].Stop();
    }

    private void CheckClipName(string clipName)
    {
        if (!sounds.ContainsKey(clipName))
            throw new Exception($"Invalid clipName: {clipName}");
    }
}
