using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lock
{
    public class DoorLock: MonoBehaviour
    {
        // get from game manager the values for MinimumDial and HighestDial
        // the gama manager have the following events:
        // public event Action<int> OnMinimumDialChanged;
        // public event Action<int> OnHighestDialChanged;
        // subscribe to them and change the values of the variables below

        public (int min, int max) PasswordRange { get; set; } = (400,500);

        private int currentDial;
        private GameManager gameManager;
        private bool isLockActive;
        // method Lockpicking set value o isLockActive
        private Image image;
        private Sprite lockedSprite;
        private Sprite unlockedSprite;
        public Door ActiveDoor { get; set; }
        public bool Lockpicking 
        {
            set => isLockActive = value;
        }

        private bool unlocked;
        public bool Unlocked
        {
            get => unlocked;
            set
            {
                unlocked = value;
                if (image == null) return;
                image.sprite = value ? unlockedSprite : lockedSprite;
            }
        }

        [SerializeField] private GameObject hookSelectingPos;
        public GameObject HookSelectingPos => hookSelectingPos;

        private void OnDialChanged(int value)
        {
            currentDial = value;
        }
        
        private void Awake()
        {
            image = GetComponent<Image>();
            lockedSprite = Resources.Load<Sprite>("lock_closed"); 
            unlockedSprite = Resources.Load<Sprite>("lock_open");
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.OnDialChanged += OnDialChanged;
        }

        private void Update()
        {
            if (!isLockActive || Unlocked) return;
            
            if ((currentDial >= PasswordRange.min && currentDial <= PasswordRange.max) || Input.GetKeyDown(KeyCode.K))
            {
                // Debug.Log("Door unlocked, solution: " + PasswordRange.min + " - " + PasswordRange.max + "current: " + currentDial);
                Unlocked = true;
                image.sprite = unlockedSprite;
                LockManager.Instance.SwitchHook();
                if (ActiveDoor != null)
                    ActiveDoor.LockUnlocked(this);
            }
            else
                image.sprite = lockedSprite;
        }
    }
}