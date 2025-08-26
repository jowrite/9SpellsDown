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
    private Camera mainCamera;

    private void Start()
    {
        originalScale = transform.localScale;
        startPos = transform.localPosition;
        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;
        mainCamera = Camera.main;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Card {cd.cardName} tapped by {owner.playerName}");
        //Debug.Log(eventData);

        if (!owner.isHuman) return;
        isDragging = true;
        canvasGroup.blocksRaycasts = false; // Disable raycasting to allow drag events to pass through
        canvasGroup.alpha = 0.8f; // Slightly fade for feedback
        
        //Bring to front with same parent
        transform.SetAsLastSibling();

        transform.DOScale(originalScale * 1.1f, 0.1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Debug.Log($"Card parent: {transform.parent.name}, Active: {gameObject.activeInHierarchy}");
        transform.position = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        canvasGroup.alpha = 1f;
        transform.DOScale(originalScale, 0.1f);

        TryPlay(eventData.position); //Pass screen coordinates directly
    }

    public void TryPlay(Vector2 screenPosition)
    {
        Debug.Log($"TryPlay: card={cd?.cardName ?? "NULL"} owner={(owner != null ? owner.playerName : "NULL")} pos={screenPosition}");

        // Find all zones and check which one is valid
        DropZone[] allZones = FindObjectsByType<DropZone>(FindObjectsSortMode.None);
        DropZone validZone = null; ;
        
        foreach (DropZone zone in allZones)
        {
            if (zone.IsValidDrop(screenPosition))
            {
                validZone = zone;
                Debug.Log($"TryPlay: valid drop zone found: {zone.name}");
                break; // Stop after finding the first valid zone
            }
        }

        if (validZone == null)
        {
            Debug.Log($"TryPlay: no valid drop zone found for {owner.playerName} card {cd.cardName}. Returning to hand.");
            ReturnToHand();
            return;
        }

        //Check if it's the player's turn
        if (TurnManager.turn == null)
        {
            Debug.LogError("TryPlay: TurnManager.turn is NULL");
            ReturnToHand();
            return;
        }

        PlayerData currentPlayer = TurnManager.turn.GetCurrentPlayer();
        if (currentPlayer == null)
        {
            Debug.LogError("TryPlay: Current player is NULL");
            ReturnToHand();
            return;
        }

        //Other debug check, might not need
        if (currentPlayer != owner)
        {
            Debug.Log($"TryPlay: Out of turn. Current player is {currentPlayer.playerName}, but {owner.playerName} tried to play.");
            ReturnToHand();
            return;
        }
        
        // All good — register play first, then animate to zone
        Debug.Log($"TryPlay: registering play for {owner.playerName} card {cd.cardName}");
        TrickManager.tm.PlayCard(owner, cd, gameObject);

        //Convert screen position to world position for effects
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f; // Assuming a 2D game in XY plane

        // Reparent + animate into the play area (use TrickManager transform, so visuals remain)
        SnapToZone(validZone.transform, worldPosition);
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
        transform.DOLocalMove(startPos, returnDuration)
           .SetEase(Ease.OutBack)
           .OnComplete(() =>
           {
               canvasGroup.blocksRaycasts = true; // Re-enable raycasting after return
               transform.localScale = originalScale; // Ensure scale is reset
           }); 
    }

    private void SnapToZone(Transform zoneTransform, Vector3 screenPosition)
    {

        // Reparent to TrickManager play area so the card sits under TrickManager in hierarchy
        Transform playArea = TrickManager.tm.transform;
        
        Vector3 worldTarget = zoneTransform.position;
        worldTarget.z = 0f; // Ensure z=0 for 2D
        
        transform.SetParent(playArea, true);

        Sequence snapSeq = DOTween.Sequence();

        snapSeq.Append(transform.DOLocalMove(worldTarget, snapDuration).SetEase(snapEase));
        snapSeq.Join(transform.DOScale(1.2f, snapDuration / 2).SetLoops(2, LoopType.Yoyo));
        snapSeq.InsertCallback(snapDuration * 0.5f, () => TriggerEffects(worldTarget));
        snapSeq.OnComplete(() =>
        {
            zoneTransform.GetComponent<DropZone>().OnCardPlayed(this); // Notify drop zone
        });
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

        transform.DOScale(originalScale * 2f, duration) // Scale up slightly
            .SetDelay(delay)
            .SetEase(Ease.OutCubic);
    }

}