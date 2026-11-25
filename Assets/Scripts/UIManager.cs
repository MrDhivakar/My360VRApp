using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject memoryListPanel;
    public Transform contentParent;
    public GameObject memoryItemPrefab;

    [Header("Viewer References")]
    public GameObject viewerPanel;
    public PanoramaViewer panoramaViewer;

    void Start()
    {
        if (viewerPanel != null) viewerPanel.SetActive(false);
        PopulateMemoryList();
    }

    public void PopulateMemoryList()
    {
        if (contentParent == null) return;
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (MemoryManager.Instance == null) return;

        foreach (var mem in MemoryManager.Instance.memories)
        {
            GameObject item = Instantiate(memoryItemPrefab, contentParent);
            MemoryButton btn = item.GetComponent<MemoryButton>();
            if (btn != null) btn.Setup(mem, this);
        }
    }

    public void OpenMemory(MemoryData mem)
    {
        if (memoryListPanel != null) memoryListPanel.SetActive(false);
        if (viewerPanel != null) viewerPanel.SetActive(true);

        if (panoramaViewer != null)
            panoramaViewer.ApplyDirection(mem.heading);
    }

    public void CloseViewer()
    {
        if (viewerPanel != null) viewerPanel.SetActive(false);
        if (memoryListPanel != null) memoryListPanel.SetActive(true);
    }
}
