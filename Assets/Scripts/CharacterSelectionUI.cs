using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterSwitcher characterSwitcher;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private PlayerDataManager playerDataManager;

    [Header("UI Elements")]
    [SerializeField] private Image characterDisplayImage;
    [SerializeField] private Transform characterDisplayContainer;
    [SerializeField] private Text characterNameText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button useCharacterButton;
    [SerializeField] private Button unlockButton;

    [Header("Lock UI")]
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Text unlockCostText;
    [SerializeField] private Text lockedMessageText;

    [Header("Character Info")]
    [SerializeField] private Text characterDescriptionText;
    [SerializeField] private GameObject[] characterStatsUI;

    [Header("Visual Settings")]
    [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Sprite defaultLockSprite;
    [SerializeField] private float characterScale = 1f;

    [Header("Animation")]
    [SerializeField] private bool enableTransitionAnimation = true;
    [SerializeField] private float transitionSpeed = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip navigationSound;
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip unlockSound;
    [SerializeField] private AudioClip lockedSound;

    private int currentDisplayIndex = 0;
    private GameObject currentCharacterInstance;
    private AudioSource audioSource;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (characterSwitcher == null)
            characterSwitcher = FindObjectOfType<CharacterSwitcher>();

        if (moneyManager == null)
            moneyManager = FindObjectOfType<MoneyManager>();

        if (playerDataManager == null)
            playerDataManager = PlayerDataManager.Instance;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        if (previousButton != null)
            previousButton.onClick.AddListener(OnPreviousCharacter);

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextCharacter);

        if (useCharacterButton != null)
            useCharacterButton.onClick.AddListener(OnUseCharacter);

        if (unlockButton != null)
            unlockButton.onClick.AddListener(OnUnlockCharacter);

        if (characterSwitcher != null && characterSwitcher.autoSave)
        {
            currentDisplayIndex = PlayerPrefs.GetInt(characterSwitcher.saveKey, 0);
        }

        LoadCharacterUnlockStates();
        UpdateCharacterDisplay();
    }

    private void OnDestroy()
    {
        if (previousButton != null)
            previousButton.onClick.RemoveListener(OnPreviousCharacter);

        if (nextButton != null)
            nextButton.onClick.RemoveListener(OnNextCharacter);

        if (useCharacterButton != null)
            useCharacterButton.onClick.RemoveListener(OnUseCharacter);

        if (unlockButton != null)
            unlockButton.onClick.RemoveListener(OnUnlockCharacter);
    }

    private void LoadCharacterUnlockStates()
    {
        if (characterSwitcher == null) return;

        if (playerDataManager != null && playerDataManager.GetCurrentPlayerData() != null)
        {
            var playerData = playerDataManager.GetCurrentPlayerData();

            if (!string.IsNullOrEmpty(playerData.unlocked_cosmetics))
            {
                try
                {
                    string json = playerData.unlocked_cosmetics;
                    json = json.Replace("[", "").Replace("]", "").Replace("\"", "");
                    string[] unlockedNames = json.Split(',');

                    foreach (var characterData in characterSwitcher.characters)
                    {
                        bool isUnlocked = System.Array.Exists(unlockedNames, name =>
                 name.Trim().Equals(characterData.characterName, System.StringComparison.OrdinalIgnoreCase));

                        if (isUnlocked)
                        {
                            characterData.isUnlocked = true;
                        }
                    }

                    Debug.Log("Loaded character unlock states from Supabase");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Failed to parse unlocked cosmetics: " + e.Message);
                }
            }
        }
        else
        {
            characterSwitcher.LoadUnlockStates();
        }
    }

    private void UpdateCharacterDisplay()
    {
        if (characterSwitcher == null || characterSwitcher.characters.Count == 0)
        {
            Debug.LogWarning("No characters available to display");
            return;
        }

        currentDisplayIndex = Mathf.Clamp(currentDisplayIndex, 0, characterSwitcher.characters.Count - 1);

        var character = characterSwitcher.characters[currentDisplayIndex];

        if (characterNameText != null)
        {
            characterNameText.text = character.characterName;
            characterNameText.color = character.isUnlocked ? unlockedColor : lockedColor;
        }

        if (characterDescriptionText != null)
        {
            characterDescriptionText.text = character.isUnlocked
          ? "A powerful warrior ready for battle!"
                       : "Unlock to use this character";
        }

        UpdateCharacterVisual(character);
        UpdateLockState(character);
        UpdateButtonStates(character);
        UpdateNavigationButtons();
    }

    private void UpdateCharacterVisual(CharacterSwitcher.CharacterData character)
    {
        if (currentCharacterInstance != null)
        {
            Destroy(currentCharacterInstance);
        }

        if (characterDisplayImage != null)
        {
            if (character.characterSprite != null)
            {
                characterDisplayImage.sprite = character.characterSprite;
                characterDisplayImage.color = character.isUnlocked ? unlockedColor : lockedColor;
                characterDisplayImage.gameObject.SetActive(true);
            }
            else
            {
                characterDisplayImage.gameObject.SetActive(false);
            }
        }

        if (character.characterPrefab != null && characterDisplayContainer != null)
        {
            currentCharacterInstance = Instantiate(character.characterPrefab, characterDisplayContainer);
            currentCharacterInstance.transform.localPosition = Vector3.zero;
            currentCharacterInstance.transform.localRotation = Quaternion.identity;
            currentCharacterInstance.transform.localScale = Vector3.one * characterScale;

            if (!character.isUnlocked)
            {
                Renderer[] renderers = currentCharacterInstance.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.materials)
                    {
                        material.color = lockedColor;
                    }
                }
            }
        }
    }

    private void UpdateLockState(CharacterSwitcher.CharacterData character)
    {
        bool isLocked = !character.isUnlocked;

        if (lockOverlay != null)
        {
            lockOverlay.SetActive(isLocked);
        }

        if (lockIcon != null)
        {
            lockIcon.gameObject.SetActive(isLocked);
            if (defaultLockSprite != null)
            {
                lockIcon.sprite = defaultLockSprite;
            }
        }

        if (unlockCostText != null)
        {
            if (isLocked && character.unlockCost > 0)
            {
                unlockCostText.text = "Cost: $" + character.unlockCost;
                unlockCostText.gameObject.SetActive(true);
            }
            else
            {
                unlockCostText.gameObject.SetActive(false);
            }
        }

        if (lockedMessageText != null)
        {
            if (isLocked)
            {
                lockedMessageText.text = "LOCKED";
                lockedMessageText.gameObject.SetActive(true);
            }
            else
            {
                lockedMessageText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateButtonStates(CharacterSwitcher.CharacterData character)
    {
        bool isLocked = !character.isUnlocked;

        if (useCharacterButton != null)
        {
            useCharacterButton.interactable = !isLocked;

            Text buttonText = useCharacterButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                bool isCurrentlyUsed = characterSwitcher.currentCharacterIndex == currentDisplayIndex;
                buttonText.text = isCurrentlyUsed ? "SELECTED" : "SELECT";
            }
        }

        if (unlockButton != null)
        {
            bool canAfford = true;
            if (moneyManager != null && character.unlockCost > 0)
            {
                canAfford = moneyManager.GetCurrentMoney() >= character.unlockCost;
            }

            unlockButton.interactable = isLocked && canAfford;
            unlockButton.gameObject.SetActive(isLocked);

            Text buttonText = unlockButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = canAfford ? "UNLOCK ($" + character.unlockCost + ")" : "NOT ENOUGH MONEY";
            }
        }
    }

    private void UpdateNavigationButtons()
    {
        if (characterSwitcher == null) return;

        if (previousButton != null)
        {
            previousButton.interactable = characterSwitcher.characters.Count > 1;
        }

        if (nextButton != null)
        {
            nextButton.interactable = characterSwitcher.characters.Count > 1;
        }
    }

    private void OnPreviousCharacter()
    {
        if (isTransitioning || characterSwitcher == null) return;

        PlaySound(navigationSound);

        currentDisplayIndex--;
        if (currentDisplayIndex < 0)
        {
            currentDisplayIndex = characterSwitcher.characters.Count - 1;
        }

        if (enableTransitionAnimation)
        {
            StartCoroutine(TransitionToCharacter());
        }
        else
        {
            UpdateCharacterDisplay();
        }
    }

    private void OnNextCharacter()
    {
        if (isTransitioning || characterSwitcher == null) return;

        PlaySound(navigationSound);

        currentDisplayIndex++;
        if (currentDisplayIndex >= characterSwitcher.characters.Count)
        {
            currentDisplayIndex = 0;
        }

        if (enableTransitionAnimation)
        {
            StartCoroutine(TransitionToCharacter());
        }
        else
        {
            UpdateCharacterDisplay();
        }
    }

    private void OnUseCharacter()
    {
        if (characterSwitcher == null) return;

        var character = characterSwitcher.characters[currentDisplayIndex];

        if (!character.isUnlocked)
        {
            PlaySound(lockedSound);
            Debug.Log("Cannot use locked character!");
            return;
        }

        PlaySound(selectSound);

        characterSwitcher.SwitchToCharacter(currentDisplayIndex);

        UpdateButtonStates(character);

        Debug.Log("Character selected: " + character.characterName);
    }

    private void OnUnlockCharacter()
    {
        if (characterSwitcher == null) return;

        var character = characterSwitcher.characters[currentDisplayIndex];

        if (character.isUnlocked)
        {
            Debug.Log("Character already unlocked!");
            return;
        }

        int currentMoney = moneyManager != null ? moneyManager.GetCurrentMoney() : 0;

        if (currentMoney < character.unlockCost)
        {
            PlaySound(lockedSound);
            Debug.Log("Not enough money! Need " + character.unlockCost + ", have " + currentMoney);
            return;
        }

        if (moneyManager != null && character.unlockCost > 0)
        {
            moneyManager.SpendMoney(character.unlockCost);
        }

        characterSwitcher.UnlockCharacter(currentDisplayIndex);

        SaveUnlockedCharacterToSupabase(character.characterName);

        PlaySound(unlockSound);

        UpdateCharacterDisplay();

        Debug.Log("Character unlocked: " + character.characterName);
    }

    private void SaveUnlockedCharacterToSupabase(string characterName)
    {
        if (playerDataManager == null || playerDataManager.GetCurrentPlayerData() == null)
            return;

        var playerData = playerDataManager.GetCurrentPlayerData();

        List<string> unlockedList = new List<string>();

        if (!string.IsNullOrEmpty(playerData.unlocked_cosmetics))
        {
            string json = playerData.unlocked_cosmetics;
            json = json.Replace("[", "").Replace("]", "").Replace("\"", "");
            if (!string.IsNullOrEmpty(json))
            {
                unlockedList.AddRange(json.Split(','));
            }
        }

        if (!unlockedList.Contains(characterName))
        {
            unlockedList.Add(characterName);
        }

        playerData.unlocked_cosmetics = "[\"" + string.Join("\",\"", unlockedList) + "\"]";
        playerData.updated_at = System.DateTime.UtcNow.ToString("o");

        playerDataManager.SavePlayerData(playerData);

        Debug.Log("Saved unlocked character to Supabase: " + characterName);
    }

    private System.Collections.IEnumerator TransitionToCharacter()
    {
        isTransitioning = true;

        float elapsed = 0f;
        float duration = 1f / transitionSpeed;

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / (duration / 2f));
            yield return null;
        }

        UpdateCharacterDisplay();

        elapsed = 0f;
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = elapsed / (duration / 2f);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        isTransitioning = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void OpenCharacterSelection()
    {
        gameObject.SetActive(true);
        UpdateCharacterDisplay();
    }

    public void CloseCharacterSelection()
    {
        gameObject.SetActive(false);
    }
}
