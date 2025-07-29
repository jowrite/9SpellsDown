using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    public CardData cd;
    public PlayerData owner;
    public TMPro.TextMeshPro valueTop;
    public TMPro.TextMeshPro valueBottom;
    public SpriteRenderer elementIcon;

    [Header("Settings")]
    public float snapDuration = 0.3f;
    public Ease snapEase = Ease.OutCubic;
    public float dragScale = 1.1f;
    public float returnDuration = 0.3f;

    [Header("VFX/SFX")]
    public GameObject playVFX; //Particle system or prefab
    public AudioClip playSFX; //Card play sound

    private Vector3 startPos;
    private Vector3 originalScale;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private bool isDragging;

    private void Start()
    {
        originalScale = transform.localScale;
        startPos = transform.localPosition;
        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Card {cd.cardName} tapped by {owner.playerName}");
        Debug.Log(eventData);
        
        if (!owner.isHuman) return;
        isDragging = true;
        canvasGroup.blocksRaycasts = false; // Disable raycasting to allow drag events to pass through
        transform.SetParent(transform.root); // Move card to root to avoid UI hierarchy issues
        //transform.DOScale(dragScale, 0.2f); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        transform.position = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        //transform.DOScale(1f, 0.2f);
        TryPlay(eventData.position); 
    }

    public void TryPlay(Vector2 screenPosition)
    {
        //Check if its the player's turn
        //Use screen position to see if the card was tapped in the drop zone area

        DropZone zone = FindFirstObjectByType<DropZone>();
        if (zone != null && RectTransformUtility.RectangleContainsScreenPoint(
            zone.GetComponent<RectTransform>(), screenPosition))
        {
            SnapToZone(zone.transform.position, zone);
        }
        else
        {
            //Return to hand if not dropped in a valid zone
            ReturnToHand();
        }
    }

    private void ReturnToHand()
    {
        transform.DOLocalMove(startPos, 0.3f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => canvasGroup.blocksRaycasts = true); // Re-enable raycasting after return
    }

    private void SnapToZone(Vector3 targetPosition, DropZone zone)
    {
        //Animate position, scale and fade
        Sequence snapSeq = DOTween.Sequence();

        //Move & scale
        snapSeq.Append(transform.DOLocalMove(targetPosition, snapDuration).SetEase(snapEase));
        snapSeq.Join(transform.DOScale(1.2f, snapDuration / 2).SetLoops(2, LoopType.Yoyo));

        //Add sparkle + sound at halfway point
        snapSeq.InsertCallback(snapDuration * 0.5f, () => TriggerEffects(targetPosition));

        //Fade & destroy after play
        snapSeq.OnComplete(() => {
            zone.OnCardPlayed(this); // Notify drop zone
            Destroy(gameObject); // Destroy card after play
        });

    }
    private void TriggerEffects(Vector3 worldPos)
    {
        if (playVFX)
        {
            GameObject vfx = Instantiate(playVFX, worldPos, Quaternion.identity, transform.root);
            Destroy(vfx, 2f); // Destroy after 2 seconds
        }

        //if (AudioManager.am && playSFX)
        //{
        //    AudioManager.am.PlaySFX(playSFX);
        //}
    }

    public void SetCardVisuals()
    {
        if (cd == null)
        {
            Debug.LogError("CardData is null!");
            return;
        }

        if (valueTop != null)
            valueTop.text = cd.value.ToString();
        else
            Debug.LogError("valueTop TextMeshPro is not assigned!");

        if (valueBottom != null)
            valueBottom.text = cd.value.ToString();
        else
            Debug.LogError("valueBottom TextMeshPro is not assigned!");

        if (elementIcon != null && cd.elementIcon != null)
            elementIcon.sprite = cd.elementIcon;
        else
            Debug.LogError("elementIcon or CardData.elementIcon is missing!");
    }

    public void AnimToPosition(Vector2 targetPos, float delay = 0f, float duration = 0.4f)
    {
        Debug.Log($"Animating from: {transform.position} to {targetPos}");

        //Convert to local space if necessary
        if (transform.parent != null)
        {
            targetPos = transform.parent.InverseTransformPoint(targetPos);
        }

        transform.DOLocalMove(targetPos, duration) // Move to target position
            .SetDelay(delay)
            .SetEase(Ease.OutCubic);

        transform.DOScale(originalScale, duration) // Scale back to original size
            .SetDelay(delay)
            .SetEase(Ease.OutCubic);
    }
}
