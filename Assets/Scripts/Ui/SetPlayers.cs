using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SetPlayers : MonoBehaviour
{
    [Header("UI Panels and Groups")]
    [SerializeField] private List<GameObject> Panels = new List<GameObject>();
    [SerializeField] private List<CanvasGroup> PanelGroups = new List<CanvasGroup>();

    [Header("Player Positions and Indicators")]
    [SerializeField] private List<RectTransform> PlayersPos = new List<RectTransform>();
    [SerializeField] private RectTransform player1Indicator;
    [SerializeField] private RectTransform player2Indicator;
    [SerializeField] private float fadeDuration = 0.5f;

    private int currentPanelIndex = 0;
    private int player1Index = 0;
    private int player2Index = 1;

    private bool isPlayer1Locked = false;
    private bool isPlayer2Locked = false;

    private float inputThreshold = 0.5f;

    private void Start()
    {
        player1Indicator.anchoredPosition = PlayersPos[player1Index].anchoredPosition;
        player2Indicator.anchoredPosition = PlayersPos[player2Index].anchoredPosition;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isPlayer1Locked)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            HandleMove(ref player1Index, input.x, player2Index, player1Indicator);
        }
    }

    public void OnMove1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !isPlayer2Locked)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            HandleMove(ref player2Index, input.x, player1Index, player2Indicator);
        }
    }

    public void OnApply(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isPlayer1Locked = true;
            CheckIfBothPlayersLocked();
        }
    }

    public void OnApply1(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isPlayer2Locked = true;
            CheckIfBothPlayersLocked();
        }
    }

    public void SetPlayerCount(int howManyPlayer)
    {
        PanelGroups[0]
        .DOFade(0f, fadeDuration)
        .OnComplete(() =>
        {
            Panels[0].SetActive(false);
            Panels[1].SetActive(true);
            PanelGroups[1].alpha = 0f;
            PanelGroups[1].DOFade(1f, fadeDuration);
        });
    }

    private void HandleMove(ref int playerIndex, float horizontalInput, int otherPlayerIndex, RectTransform indicator)
    {
        if (Mathf.Abs(horizontalInput) < inputThreshold) return;

        int direction = horizontalInput > 0 ? 1 : -1;
        int tentativeIndex = Mathf.Clamp(playerIndex + direction, 0, PlayersPos.Count - 1);

        if (tentativeIndex == otherPlayerIndex)
        {
            int skipIndex = Mathf.Clamp(tentativeIndex + direction, 0, PlayersPos.Count - 1);
            if (skipIndex != otherPlayerIndex && skipIndex != playerIndex)
            {
                tentativeIndex = skipIndex;
            }
            else return;
        }

        if (tentativeIndex != playerIndex)
        {
            playerIndex = tentativeIndex;
            Vector2 targetPos = PlayersPos[playerIndex].anchoredPosition;
            indicator.DOAnchorPos(targetPos, 0.2f);
        }
    }
    private void CheckIfBothPlayersLocked()
    {
        if (isPlayer1Locked && isPlayer2Locked)
        {

            SceneManager.LoadScene(1);
        }
    }

    private void OnDisable()
    {
        PanelGroups[1]
        .DOFade(0f, fadeDuration)
        .OnComplete(() =>
        {
            Panels[1].SetActive(false);
            Panels[0].SetActive(true);
            PanelGroups[0].alpha = 0f;
            PanelGroups[0].DOFade(1f, fadeDuration);
        });
    }
}