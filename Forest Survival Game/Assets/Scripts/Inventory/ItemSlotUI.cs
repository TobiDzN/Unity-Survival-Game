using System;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI")]
    [SerializeField] private RawImage icon;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private RawImage highlight;

    public int Index { get; private set; }

    public event Action<int> OnHoverEnter;
    public event Action<int> OnHoverExit;
    public event Action<int> OnClick;

    // Drag events
    public event Action<int> OnBeginDragSlot;
    public event Action<int> OnDragSlot;
    public event Action<int> OnEndDragSlot;
    public event Action<int, int> OnDropOnSlot; // fromIndex, toIndex
    private Texture defaultHighlightTexture;

    private void Awake()
    {
        if (highlight != null)
            defaultHighlightTexture = highlight.texture;
    }

    public void Init(int index)
    {
        Index = index;
        SetHighlight(false);
    }

    public void SetItem(MaterialsData item, int amount)
    {
        if (item != null)
        {
            icon.texture = item.itemIcon;
            icon.enabled = true;
            if (amountText != null) amountText.text = (amount > 1) ? amount.ToString() : "";
        }
        else
        {
            icon.texture = null;
            icon.enabled = false;
            if (amountText != null) amountText.text = "";
        }
    }

    public void SetHighlight(bool on, Texture hoverTex = null)
    {
        if (highlight == null) return;

        if (!on)
        {
            highlight.enabled = false;
            highlight.texture = defaultHighlightTexture; //return to default
            return;
        }

        highlight.enabled = true;
        highlight.texture = (hoverTex != null) ? hoverTex : defaultHighlightTexture;
    }


    public void OnPointerEnter(PointerEventData eventData) => OnHoverEnter?.Invoke(Index);
    public void OnPointerExit(PointerEventData eventData) => OnHoverExit?.Invoke(Index);
    public void OnPointerClick(PointerEventData eventData) => OnClick?.Invoke(Index);

    public void OnBeginDrag(PointerEventData eventData) => OnBeginDragSlot?.Invoke(Index);
    public void OnDrag(PointerEventData eventData) => OnDragSlot?.Invoke(Index);
    public void OnEndDrag(PointerEventData eventData) => OnEndDragSlot?.Invoke(Index);

    // release on the slot that we drop at
    public void OnDrop(PointerEventData eventData)
    {
        var from = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<ItemSlotUI>() : null;
        if (from == null) return;

        OnDropOnSlot?.Invoke(from.Index, Index);
    }


}
