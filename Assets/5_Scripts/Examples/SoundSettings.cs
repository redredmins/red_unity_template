using System;
using System.Collections.Generic;
using UnityEngine;
using RedMinS;

namespace RedMinS.Examples
{
    [Serializable]
    public struct SoundEntry
    {
        public SoundLabel label;
        public AudioClip clip;
    }

    public class SoundSettings : MonoBehaviour
    {
        [SerializeField] List<SoundEntry> sounds;

        void Start()
        {
            foreach (var entry in sounds)
            {
                Core.app.sound.RegisterClip(entry.label.ToString(), entry.clip);
            }
        }
    }
}
