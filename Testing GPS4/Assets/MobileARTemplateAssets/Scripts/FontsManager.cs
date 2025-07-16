using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FontsManager : MonoBehaviour
{
    public static FontsManager Instance { get; private set; }

    [SerializeField] private TMP_Dropdown fontSizeDropdown;
    [SerializeField] private float applyDelay = 0.1f;

    public const float SMALL_FONT_SIZE = 36f;
    public const float NORMAL_FONT_SIZE = 40f;
    public const float LARGE_FONT_SIZE = 45f;

    public int currentFontSizeOption = 1; // Default to Normal

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetupDropdown();
        ApplyFontSizeToCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Invoke(nameof(ApplyFontSizeToCurrentScene), applyDelay);
        FindDropdown();
    }

    private void FindDropdown()
    {
        if (fontSizeDropdown == null)
        {
            fontSizeDropdown = Object.FindFirstObjectByType<TMP_Dropdown>();
            if (fontSizeDropdown != null)
            {
                SetupDropdown();
            }
        }
    }

    public void OnFontSizeDropdownChanged(int index)
    {
        SetFontSize(index);
    }

    public void SetFontSize(int sizeIndex)
    {
        if (sizeIndex < 0 || sizeIndex > 2)
        {
            Debug.LogWarning("Invalid font size index: " + sizeIndex);
            return;
        }

        currentFontSizeOption = sizeIndex;
        SaveSettings();
        ApplyFontSizeToCurrentScene();

        if (fontSizeDropdown != null && fontSizeDropdown.value != sizeIndex)
        {
            fontSizeDropdown.SetValueWithoutNotify(sizeIndex);
        }
    }

    public float GetCurrentFontSize()
    {
        switch (currentFontSizeOption)
        {
            case 0: return SMALL_FONT_SIZE;
            case 1: return NORMAL_FONT_SIZE;
            case 2: return LARGE_FONT_SIZE;
            default: return NORMAL_FONT_SIZE;
        }
    }

    public void ApplyFontSizeToCurrentScene()
    {
        float baseSize = GetCurrentFontSize();

        //  for Buttons, Menus, Panels
        TextMeshProUGUI[] uiTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in uiTexts)
        {
            if (text.CompareTag("FixedSizeText")) continue;

            if (text.CompareTag("benDetails"))
            {
                // only for text in benefit's details
                switch (currentFontSizeOption)
                {
                    case 0: text.fontSize = 20f; break; // Small
                    case 1: text.fontSize = 23f; break; // Normal
                    case 2: text.fontSize = 26f; break; // Large
                }
            }
            else
            {
                text.fontSize = baseSize;
            }
        }

        //  for labels floating in AR space
        TextMeshPro[] worldTexts = Resources.FindObjectsOfTypeAll<TextMeshPro>();
        foreach (TextMeshPro text in worldTexts)
        {
            if (text.CompareTag("FixedSizeText")) continue;

            if (text.CompareTag("benDetails"))
            {
                switch (currentFontSizeOption)
                {
                    case 0: text.fontSize = 20f; break;
                    case 1: text.fontSize = 23f; break;
                    case 2: text.fontSize = 26f; break;
                }
            }
            else
            {
                text.fontSize = baseSize;
            }
        }

        Debug.Log("Font sizes applied: base " + baseSize + ", benDetails custom sizes applied");
    }


    private void SetupDropdown()
    {
        if (fontSizeDropdown != null)
        {
            fontSizeDropdown.SetValueWithoutNotify(currentFontSizeOption);
            fontSizeDropdown.onValueChanged.RemoveAllListeners();
            fontSizeDropdown.onValueChanged.AddListener(OnFontSizeDropdownChanged);
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("FontSizeOption", currentFontSizeOption);
        PlayerPrefs.Save();
        Debug.Log($"Font size setting saved: {currentFontSizeOption}");
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("FontSizeOption"))
        {
            currentFontSizeOption = PlayerPrefs.GetInt("FontSizeOption");
            Debug.Log($"Font size setting loaded: {currentFontSizeOption}");
        }
    }

    public static float GetGlobalFontSize()
    {
        return Instance != null ? Instance.GetCurrentFontSize() : NORMAL_FONT_SIZE;
    }

}
