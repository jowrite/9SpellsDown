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
        Debug.Log($"TryPlay: card={cd?.cardName ?? "NULL"} owner={(owner != null ? owner.playerName : "NULL")} pos={screenPosition}");

        // find zone
        DropZone zone = FindFirstObjectByType<DropZone>();
        bool zoneValid = false;
        if (zone == null)
        {
            Debug.LogWarning("TryPlay: No DropZone found in scene!");
        }
        else
        {
            zoneValid = zone.IsValidDrop(screenPosition);
            Debug.Log($"TryPlay: zone found. IsValidDrop={zoneValid}");
        }

        if (!zoneValid)
        {
            Debug.Log("TryPlay: invalid drop - returning to hand.");
            ReturnToHand();
            return;
        }

        // quick data guards
        if (cd == null)
        {
            Debug.LogError("TryPlay: CardData is NULL - cannot play.");
            ReturnToHand();
            return;
        }

        if (owner == null)
        {
            Debug.LogError("TryPlay: Owner PlayerData is NULL - cannot play.");
            ReturnToHand();
            return;
        }

        // TurnManager presence
        if (TurnManager.turn == null)
        {
            Debug.LogError("TryPlay: TurnManager.turn is NULL!");
            ReturnToHand();
            return;
        }

        // Detailed current player check + debug instance ids
        PlayerData current = TurnManager.turn.GetCurrentPlayer();
        if (current == null)
        {
            Debug.LogError("TryPlay: TurnManager returned null current player!");
            ReturnToHand();
            return;
        }

        Debug.Log($"TryPlay: owner id={owner.GetInstanceID()} name={owner.playerName}  current id={current.GetInstanceID()} name={current.playerName}");

        // ownership validation (strict by reference)
        if (current != owner)
        {
            Debug.Log($"TryPlay: Out of turn. Current player is {current.playerName}. Owner is {owner.playerName}. Rejecting play.");
            ReturnToHand();
            return;
        }

        // human-only
        if (!owner.isHuman)
        {
            Debug.Log("TryPlay: owner is not human, ignoring.");
            ReturnToHand();
            return;
        }

        // All good — register play first, then animate to zone
        Debug.Log($"TryPlay: registering play for {owner.playerName} card {cd.cardName}");
        TrickManager.tm.PlayCard(owner, cd, gameObject);

        // Reparent + animate into the play area (use TrickManager transform, so visuals remain)
        SnapToZone(zone.transform);
    }
    //Old logic, trying something new above
    ////Check if its the player's turn
    ////Use screen position to see if the card was tapped in the drop zone area

    //DropZone zone = FindFirstObjectByType<DropZone>();
    //bool validDrop = zone != null && zone.IsValidDrop(screenPosition);

    //if (!validDrop)
    //{
    //    ReturnToHand();
    //    return;
    //}

    ////Block out of turn plays
    //if (TurnManager.turn == null)
    //{
    //    Debug.LogError("TurnManager missing!");
    //    ReturnToHand();
    //    return;
    //}

    //PlayerData current = TurnManager.turn.GetCurrentPlayer();
    //if (current != owner)
    //{
    //    Debug.Log($"{owner.playerName} tried to play out of turn.");
    //    ReturnToHand();
    //    return;
    //}

    ////Register play
    //TrickManager.tm.PlayCard(owner, cd, gameObject);

    ////Anim to the play area
    //SnapToZone(zone.transform);

    private void ReturnToHand()
    {
        transform.DOLocalMove(startPos, 0.3f)
           .SetEase(Ease.OutBack)
           .OnComplete(() => canvasGroup.blocksRaycasts = true); // Re-enable raycasting after return
    }

    private void SnapToZone(Transform zoneTransform)
    {

        // Reparent to TrickManager play area so the card sits under TrickManager in hierarchy
        Transform playArea = TrickManager.tm.transform;
        Vector3 worldTarget = zoneTransform.position;

        // Make sure world position stays the same when reparenting
        transform.SetParent(playArea, worldPositionStays: true);

        // Convert world target into local position for the playArea
        Vector3 localTarget = playArea.InverseTransformPoint(worldTarget);

        Sequence snapSeq = DOTween.Sequence();

        snapSeq.Append(transform.DOLocalMove(localTarget, snapDuration).SetEase(snapEase));
        snapSeq.Join(transform.DOScale(1.2f, snapDuration / 2).SetLoops(2, LoopType.Yoyo));
        snapSeq.InsertCallback(snapDuration * 0.5f, () => TriggerEffects(worldTarget));
        ////Animate position, scale and fade
        //Sequence snapSeq = DOTween.Sequence();

        ////Move & scale
        //snapSeq.Append(transform.DOLocalMove(targetPosition, snapDuration).SetEase(snapEase));
        //snapSeq.Join(transform.DOScale(1.2f, snapDuration / 2).SetLoops(2, LoopType.Yoyo));

        ////Add sparkle + sound at halfway point
        //snapSeq.InsertCallback(snapDuration * 0.5f, () => TriggerEffects(targetPosition));

        //snapSeq.OnComplete(() => GET RID OF IT ALL
        //{
        //    ////Tell Trick Manager befor destroying **might not need this tbd
        //    //if (cd != null && owner != null && owner.isHuman)
        //    //{
        //    //    //Re-parent into play area
        //    //    transform.SetParent(TrickManager.tm.transform, true);

        //    //    TrickManager.tm.PlayCard(owner, cd, gameObject);
        //    //}

        //    zone.OnCardPlayed(this); // Notify drop zone

        //});

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