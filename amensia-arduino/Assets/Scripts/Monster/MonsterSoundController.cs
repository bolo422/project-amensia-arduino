using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterSoundController : MonoBehaviour
    {
        [SerializeField] private AudioSource heartbeatSlow;
        [SerializeField] private AudioSource heartbeatFast;
        [SerializeField] private AudioSource monsterSounds;
        [SerializeField] private float fastHeartbeatDistance;
        [SerializeField] private float audioDistance;
        private Dictionary<AudioSource, float> audioSources;
        private Transform player;
        private bool playerInsideRange;

        private void Awake()
        {
            audioSources = new Dictionary<AudioSource, float>();
            foreach (Transform child in transform)
            {
                var audioSource = child.GetComponent<AudioSource>();
                audioSources.Add(audioSource, audioSource.volume);
            }
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        private void FixedUpdate()
        {
            var distanceToPlayer = Vector2.Distance(transform.position, player.position);
            AdjustAudios(distanceToPlayer);
            if (distanceToPlayer > audioDistance)
            {
                return;
            }
            if (distanceToPlayer < fastHeartbeatDistance)
            {
                if(heartbeatSlow.isPlaying)
                    heartbeatSlow.Stop();
                if(!heartbeatFast.isPlaying)
                    heartbeatFast.Play();
            }
            else
            {
                if(heartbeatFast.isPlaying)
                    heartbeatFast.Stop();
                if(!heartbeatSlow.isPlaying)
                    heartbeatSlow.Play();
            }
        }

        private void AdjustAudios(float distanceToPlayer)
        {
            foreach (var audioSource in audioSources)
            {
                SetVolume(audioSource.Key, distanceToPlayer, audioSource.Value);
            }
        }

        private void SetVolume(AudioSource audioSource, float distanceToPlayer, float maxVolume)
        {
            if (distanceToPlayer >= audioDistance)
            {
                audioSource.volume = 0.0f;
            }
            else if (distanceToPlayer <= 0.0f)
            {
                audioSource.volume = maxVolume;
            }
            else
            {
                audioSource.volume = ((audioDistance - distanceToPlayer) / audioDistance) * maxVolume;
            }
        }
    }
}