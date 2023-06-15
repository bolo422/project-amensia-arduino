using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lock
{
    public class LockManager : MonoBehaviour
    {
        public static LockManager Instance { get; private set; }
        
        [SerializeField] private List<DoorLock> doorLocks;
        public List<DoorLock> DoorLocks => doorLocks;
        [SerializeField] private GameObject hook;
        public int currentDoorLockIndex = 0;
        private bool lockPicking;
        private Animator hookAnimator;
        private static readonly int Selecting = Animator.StringToHash("selecting");
        [SerializeField] private GameObject locksScreen;

        public bool IsLocksScreenActive
        {
            get => locksScreen.activeSelf;
            set
            {
                locksScreen.SetActive(value);
                if(!value)
                    ResetLockScreen();
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // this is the first instance
            }
            else
            {
                Destroy(gameObject); // this must be a duplicate from a scene reload - DESTROY!
            }
            
            // get animator in hook children
            hookAnimator = hook.GetComponentInChildren<Animator>();
            hook.transform.position = doorLocks[currentDoorLockIndex].HookSelectingPos.transform.position;
        }

        private void Update()
        {
            if (!locksScreen.activeSelf) return;
            // if player press Right arrow key, move the hook to the next door lock
            if (!lockPicking)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentDoorLockIndex++;
                    if (currentDoorLockIndex >= doorLocks.Count)
                        currentDoorLockIndex = 0;
                    hook.transform.position = doorLocks[currentDoorLockIndex].HookSelectingPos.transform.position;
                }

                // if player press Left arrow key, move the hook to the previous door lock
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentDoorLockIndex--;
                    if (currentDoorLockIndex < 0)
                        currentDoorLockIndex = doorLocks.Count - 1;
                    hook.transform.position = doorLocks[currentDoorLockIndex].HookSelectingPos.transform.position;
                }
            }

            // if player press space bar, start lock picking
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwitchHook();
            }
        }

        public void ResetLockScreen()
        {
            currentDoorLockIndex = 0;
            hook.transform.position = doorLocks[currentDoorLockIndex].HookSelectingPos.transform.position;
            lockPicking = false;
        }

        public void SwitchHook()
        {
            if (!lockPicking)
            {
                if (doorLocks[currentDoorLockIndex].Unlocked)
                    return;
                
                doorLocks[currentDoorLockIndex].Lockpicking = true;
                lockPicking = true;
                hookAnimator.SetBool(Selecting, false);
            }
            else
            {
                doorLocks[currentDoorLockIndex].Lockpicking = false;
                lockPicking = false;
                hookAnimator.SetBool(Selecting, true);
            }
        }

    }
}