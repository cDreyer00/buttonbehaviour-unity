using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum ButtonAnimation { Shake, Punch, Yoyo }
public enum EInteractionsType { ClickUp, ClickDown, Enter, Exit }

public class ButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool interactable = true;
    [SerializeField] FeedbackActions[] feedbackActions;
    public FeedbackActions[] FeedbackActions => feedbackActions;

    public Action onClickDown { get; private set; }
    public Action onClickUp { get; private set; }
    public Action onEnter { get; private set; }
    public Action onExit { get; private set; }

    bool dragging;
    Vector3 inputPos;

    public RectTransform RectTransform {get; private set;}
    public Image Image {get; private set;}

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Image = GetComponent<Image>();
    }

    public void AddListener(Action action, EInteractionsType interaction)
    {
        switch (interaction)
        {
            case EInteractionsType.ClickDown:
                onClickDown += action;
                return;
            case EInteractionsType.ClickUp:
                onClickUp += action;
                return;
            case EInteractionsType.Enter:
                onEnter += action;
                return;
            case EInteractionsType.Exit:
                onExit += action;
                return;
        }
    }

    public void RemoveListener(Action action, EInteractionsType interaction)
    {
        switch (interaction)
        {
            case EInteractionsType.ClickDown:
                onClickDown -= action;
                return;
            case EInteractionsType.ClickUp:
                onClickUp -= action;
                return;
            case EInteractionsType.Enter:
                onEnter -= action;
                return;
            case EInteractionsType.Exit:
                onExit -= action;
                return;
        }
    }

    public void ClearListeners()
    {
        onClickDown = null;
        onClickUp = null;
        onEnter = null;
        onExit = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
        {
            OnInteractionDeclined(EInteractionsType.ClickDown);
            return;
        }

        inputPos = Input.mousePosition;

        ExecuteInteractions(EInteractionsType.ClickDown);
        onClickDown?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable)
        {
            OnInteractionDeclined(EInteractionsType.Enter);
            return;
        }

        ExecuteInteractions(EInteractionsType.Enter);

        onEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable)
        {
            OnInteractionDeclined(EInteractionsType.Exit);
            return;
        }

        ExecuteInteractions(EInteractionsType.Exit);

        onExit?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {            
        if (inputPos != Input.mousePosition) return;
        
        if (!interactable)
        {
            OnInteractionDeclined(EInteractionsType.ClickUp);
            return;
        }

        ExecuteInteractions(EInteractionsType.ClickUp);

        onClickUp?.Invoke();
    }

    void RunFeedback(FeedbackActions feedback)
    {
        switch (feedback.buttonFeedback)
        {
            case ButtonAnimation.Shake:
                RectTransform.DOShakeAnchorPos(0.5f, 1);
                break;
            case ButtonAnimation.Punch:
                RectTransform.DOPunchAnchorPos(new Vector2(0, 1), 1);
                break;
            case ButtonAnimation.Yoyo:
                RectTransform.DOScale(0.5f, 2).SetEase(Ease.InOutSine);
                break;
            default:
                break;
        }
    }

    void ExecuteInteractions(EInteractionsType interaction)
    {
        foreach (FeedbackActions feedback in FeedbackActions)
        {
            if (feedback.declinedInteraction) continue;
            if (feedback.interactionType != interaction) continue;

            // if (feedback.audioClip) AudioManager.PlayClip(feedback.audioClip);

            RunFeedback(feedback);
        }
    }

    void OnInteractionDeclined(EInteractionsType interaction)
    {
        foreach (FeedbackActions feedback in FeedbackActions)
        {
            if (!feedback.declinedInteraction) continue;
            if (feedback.interactionType != interaction) continue;

            // if (feedback.audioClip) AudioManager.PlayClip(feedback.audioClip);

            RunFeedback(feedback);
        }
    }

}

[Serializable]
public class FeedbackActions
{
    public ButtonAnimation buttonFeedback;
    public EInteractionsType interactionType;
    public AudioClip audioClip;

    [Tooltip("triggers the feedback when the interactable is false")]
    public bool declinedInteraction;
}
