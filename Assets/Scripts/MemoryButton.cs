using UnityEngine;
using UnityEngine.UI;

public class MemoryButton : MonoBehaviour
{
    public Text titleText;
    private MemoryData _memoryData;
    private UIManager _uiManager;

    public void Setup(MemoryData data, UIManager uiManager)
    {
        _memoryData = data;
        _uiManager = uiManager;
        if (titleText != null) titleText.text = data.title;

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => _uiManager.OpenMemory(_memoryData));
    }
}
