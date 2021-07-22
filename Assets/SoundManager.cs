using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles playing sounds in the game.
/// </summary>
public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// The sounds loaded and ready to be played.
    /// </summary>
    public Dictionary<string,AudioSource> sounds = new Dictionary<string, AudioSource>();

    /// <summary>
    /// Whether sounds have been loaded yet.
    /// </summary>
    private bool soundsLoaded = false;

    /// <summary>
    /// Plays a sound with the given name.
    /// </summary>
    /// <param name="clipName">The name of the sound you want to play.</param>
    public void PlaySound(string clipName)
    {
        CheckClipName(clipName);

        sounds[clipName].Play();
    }

    /// <summary>
    /// Stops a sound currently being played.
    /// </summary>
    /// <param name="clipName">The name of the sound you want to stop.</param>
    public void StopSound(string clipName)
    {
        CheckClipName(clipName);

        sounds[clipName].Stop();
    }

    /// <summary>
    /// Checks if a clip exists.
    /// </summary>
    /// <param name="clipName">The name of the clip you want to check.</param>
    private void CheckClipName(string clipName)
    {
        if (!soundsLoaded)
            LoadSounds();

        if (!sounds.ContainsKey(clipName))
            throw new Exception($"Invalid clipName: {clipName}");
    }

    /// <summary>
    /// Loads the sounds from files.
    /// </summary>
    private void LoadSounds()
    {
        foreach (AudioSource source in Component.FindObjectsOfType<AudioSource>())
        {
            sounds.Add(source.clip.name, source);
        }

        soundsLoaded = true;
    }
}
