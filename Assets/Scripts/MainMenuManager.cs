using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;

    [SerializeField] List<GameObject> Panels = new List<GameObject>();
    List<CanvasGroup> PanelGroups = new List<CanvasGroup>();
    int previousPanelIndex = 0;
    [SerializeField] float fadeDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        foreach (GameObject panel in Panels)
        {
            PanelGroups.Add(panel.GetComponent<CanvasGroup>());
        }

        ChangePanel(0);
    }

    public void ChangePanel(int panelIndex)
    {
        PanelGroups[previousPanelIndex]
        .DOFade(0f, fadeDuration)
        .OnComplete(() =>
        {
            Panels[previousPanelIndex].SetActive(false);
            Panels[panelIndex].SetActive(true);
            PanelGroups[panelIndex].alpha = 0f;
            PanelGroups[panelIndex].DOFade(1f, fadeDuration);
            previousPanelIndex = panelIndex;
        });
    }
}