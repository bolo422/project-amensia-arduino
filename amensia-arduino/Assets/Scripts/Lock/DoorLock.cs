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
        private int minimumDial = 0;
        private int highestDial = 0;
        private (int min, int max) passwordRange;
        private int currentDial;
        private GameManager gameManager;
        private bool isLockActive;
        // method Lockpicking set value o isLockActive
        private Image image;
        private bool unlocked;
        private Sprite lockedSprite;
        private Sprite unlockedSprite;
        public bool Lockpicking 
        {
            set => isLockActive = value;
        }
        
        public bool Unlocked
        {
            get => unlocked;
        }

        [SerializeField] private GameObject hookSelectingPos;
        public GameObject HookSelectingPos => hookSelectingPos;

        private void OnMinimumDialChanged(int value)
        {
            minimumDial = value;
            SetPasswordRange();
        }
    
        private void OnHighestDialChanged(int value)
        {
            highestDial = value;
            SetPasswordRange();
        }
    
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
            
            minimumDial = gameManager.MinimumDial;
            highestDial = gameManager.HighestDial;
        
            gameManager.OnMinimumDialChanged += OnMinimumDialChanged;
            gameManager.OnHighestDialChanged += OnHighestDialChanged;
            gameManager.OnDialChanged += OnDialChanged;
        
            SetPasswordRange();
        }

        public void SetPasswordRange()
        {
            var range = highestDial - minimumDial;
            var fivePercentOfRange = Mathf.FloorToInt(range * gameManager.LockPickingDifficulty);

            var lowerBound = UnityEngine.Random.Range(minimumDial, highestDial - fivePercentOfRange);
            var upperBound = lowerBound + fivePercentOfRange;

            passwordRange = (lowerBound, upperBound);
            unlocked = false;
            Debug.Log("lock: " + transform.name + " - password: " + passwordRange);
        }
        
        private void Update()
        {
            if (!isLockActive || unlocked) return;

            if (currentDial >= passwordRange.min && currentDial <= passwordRange.max)
            {
                Debug.Log("Door unlocked, solution: " + passwordRange.min + " - " + passwordRange.max + "current: " + currentDial);
                unlocked = true;
                image.sprite = unlockedSprite;
                DoorManager.Instance.SwitchHook();
            }
            else
                image.sprite = lockedSprite;
        }
    }
}