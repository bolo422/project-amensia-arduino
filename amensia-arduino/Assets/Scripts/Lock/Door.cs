using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lock
{
    public class Door: MonoBehaviour
    {
        private bool playerInsideRange;
        private GameManager gameManager;
        private Dictionary<DoorLock, (int min, int max)> passwords;
        private Dictionary<DoorLock, bool> locksUnlocked;
        private LockManager lockManager;
        private List<DoorLock> doorLocks;
        private int minimumDial = 0;
        private int highestDial = 0;
        private bool playerIsInteracting;
        private bool keyWasPressed;

        public List<bool> debugLockValues;

        private void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.OnDialLimitsChanged += OnDialLimitsChanged;
            lockManager = LockManager.Instance;
            doorLocks = lockManager.DoorLocks;
            SetPasswords();
            SetPasswordToLocks();
            locksUnlocked = new Dictionary<DoorLock, bool>();
            debugLockValues = new List<bool>();
            foreach (var doorLock in doorLocks)
            {
                locksUnlocked.Add(doorLock, false);
                debugLockValues.Add(false);
            }
            SetLocksUnlocked();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //check if player is inside the range of the door
            if (other.CompareTag("Player"))
            {
                playerInsideRange = true;
                gameManager.InteractMessage = true;
                SetActiveDoor(this);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInsideRange = false;
                gameManager.InteractMessage = false;
                playerIsInteracting = false;
                SetActiveDoor(null);
                lockManager.IsLocksScreenActive = false;
            }
        }
        
        private void OnDialLimitsChanged()
        {
            SetPasswords();
            SetPasswordToLocks();
        }

        private void SetPasswordToLocks()
        {
            foreach (var doorLock in doorLocks)
            {
                doorLock.PasswordRange = passwords[doorLock];
            }
        }
        
        private void SetLocksUnlocked()
        {
            // Debug.Log("SetLocksUnlocked: " + gameObject.name);
            foreach (var doorLock in doorLocks)
            {
                doorLock.Unlocked = locksUnlocked[doorLock];
                // Debug.Log("door: " + gameObject.name + " lock: " + doorLock.name + " unlocked: " + doorLock.Unlocked);
            }
        }

        private void SetActiveDoor(Door door)
        {
            foreach (var doorLock in doorLocks)
            {
                doorLock.ActiveDoor = door;
            }
        }

        private (int min, int max) GetPasswordCombination()
        {
            highestDial = gameManager.HighestDial;
            minimumDial = gameManager.MinimumDial;
            
            var range = highestDial - minimumDial;
            var fivePercentOfRange = Mathf.FloorToInt(range * gameManager.LockPickingDifficulty);

            var lowerBound = Random.Range(minimumDial, highestDial - fivePercentOfRange);
            var upperBound = lowerBound + fivePercentOfRange;

            return (lowerBound, upperBound);
        }

        public void SetPasswords()
        {
            passwords = new Dictionary<DoorLock, (int min, int max)>();
            foreach (var doorLock in doorLocks)
            {
                passwords.Add(doorLock, GetPasswordCombination());
            }
        }

        private void Update()
        {
            if (playerInsideRange && Input.GetKeyDown(KeyCode.E) && !keyWasPressed)
            {
                keyWasPressed = true;
                gameManager.InteractMessage = false;
                playerIsInteracting = true;
                SetPasswordToLocks();
                SetLocksUnlocked();
                lockManager.IsLocksScreenActive = true;
            }
            else if (Input.GetKeyUp(KeyCode.E))
                keyWasPressed = false;
        }

        public void LockUnlocked(DoorLock doorLock)
        {
            // Debug.Log("Lock unlocked: " + doorLock.name + " in door: " + gameObject.name);
            locksUnlocked[doorLock] = true;
            debugLockValues[locksUnlocked.Keys.ToList().IndexOf(doorLock)] = true;
            if (locksUnlocked.ContainsValue(false)) return;
            Debug.Log("Door unlocked");
            Destroy(gameObject);
        }
    }
}