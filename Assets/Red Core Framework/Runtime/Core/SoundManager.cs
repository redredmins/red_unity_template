using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS
{
    public class SoundManager : MonoBehaviour //SingletonMonobehaviour<SoundManager>
    {
        [SerializeField] AudioSource bgmAudio;
        [SerializeField] AudioSource[] soundAudios;
        [SerializeField] AudioSource alertAudio;

        
        public float volumeOfSound { private set; get; }
        AudioClip _curBgm = null;

        CoroutineOperator _corOper;


        void Awake()
        {
            _corOper = new CoroutineOperator(this);

            ChangeSoundVolume(PlayerPrefs.GetFloat(ConfigString.F_SoundVolume, 1f));
        }

        public void ChangeSoundVolume(float volume)
        {
            volumeOfSound = volume;
            PlayerPrefs.SetFloat(ConfigString.F_SoundVolume, volumeOfSound);
            
            for (int i = 0; i < soundAudios.Length; ++i)
            {
                soundAudios[i].volume = volumeOfSound;
            }
            alertAudio.volume = volumeOfSound;
        }
        
        public void PlayEffectSound(AudioClip sound)
        {
            AudioSource audio = GetEmptySoundAudio();
            audio.clip = sound;
            audio.Play();
        }

        //비어있는 오디오 소스 반환
        AudioSource GetEmptySoundAudio()
        {
            int lageindex = 0;
            float lageProgress = 0;
            for (int i = 0; i < soundAudios.Length; i++)
            {
                if (soundAudios[i].isPlaying == false)
                {
                    return soundAudios[i];
                }

                // 만약 비어있는 오디오 소스 없으면 가장 진행도가 높은 오디오 소스 반환
                float progress = soundAudios[i].time / soundAudios[i].clip.length;
                if (progress > lageProgress)
                {
                    lageindex = i;
                    lageProgress = progress;
                }
            }
            return soundAudios[lageindex];
        }

    }
}