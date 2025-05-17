using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public CardData cd;
    private Vector3 startPos;
    public PlayerData owner;
    private CanvasGroup canvasGroup;

    [Header("Anim Settings")]
    public float snapDuration = 0.3f;
    public Ease snapEase = Ease.OutCubic;

    [Header("VFX/SFX")]
    public GameObject playVFX; //Particle system or prefab
    public AudioClip playSFX; //Card play sound


    private void Start()
    {
        startPos = transform.position;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; // Disable raycasting to allow drag events to pass through
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; //Move card to follow finger
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Re-enable raycasting
        TryPlay(); //Check if the card was dropped in the drop zone
    }


    public void TryPlay()
    {
        //Check if its the player's turn
        //Use screen position to see if the card was tapped in the drop zone area

        Vector2 screenPos = Input.mousePosition;

        DropZone zone = FindFirstObjectByType<DropZone>();
        RectTransform rt = zone.GetComponent<RectTransform>();

        if(RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos))
        {
            SnapToZone(rt.position, zone);
        }
        else
        {
            Debug.Log("Not in drop zone");
        }
    }

    private void SnapToZone(Vector3 targetPosition, DropZone zone)
    {
        RectTransform rt = GetComponent<RectTransform>();
        CanvasGroup group = GetComponent<CanvasGroup>();

        //Disable interaction
        if (group) group.blocksRaycasts = false;

        //Kill any ongoing tweens 
        rt.DOKill();

        //Animate position, scale and fade
        Sequence snapSeq = DOTween.Sequence();

        //Move & scale
        snapSeq.Append(rt.DOMove(targetPosition, snapDuration).SetEase(snapEase));
        snapSeq.Join(rt.DOScale(1.2f, snapDuration / 2).SetLoops(2, LoopType.Yoyo));
        
        //Add sparkle + sound at halfway point
        snapSeq.InsertCallback(snapDuration * 0.5f, () =>
        {
            TriggerEffects(targetPosition);
        });
        
        //Fade & destroy after play
        snapSeq.OnComplete(() =>
        {
            if (group) group.DOFade(0f, 0.2f).OnComplete(() =>
            {
               zone.OnCardPlayed(this);
                Destroy(gameObject);
            });
            else
            {
                zone.OnCardPlayed(this);
                Destroy(gameObject);
            }
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
}
