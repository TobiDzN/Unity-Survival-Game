using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Transform itemsContent;
    [SerializeField] private ItemSlotUI slotPrefab;

    [Header("Drag Visual")]
    [SerializeField] private RawImage dragIcon; // DragIcon (created on canvas)

    [Header("Grid size")]
    [SerializeField] private int slotCount = 49;
    private bool isDragging = false;

    private ItemSlotUI[] uiSlots;
    private int hoverIndex = -1;
    private int selectedIndex = -1;
    private int draggingIndex = -1;

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged += Refresh;

        BuildSlots();
        Refresh();

        if (dragIcon != null)
            dragIcon.enabled = false;
    }

    private void BuildSlots()
    {
        for (int i = itemsContent.childCount - 1; i >= 0; i--)
            Destroy(itemsContent.GetChild(i).gameObject);

        uiSlots = new ItemSlotUI[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            var slot = Instantiate(slotPrefab, itemsContent);
            slot.Init(i);

            slot.OnHoverEnter += HandleHoverEnter;
            slot.OnHoverExit += HandleHoverExit;
            slot.OnClick += HandleClick;

            slot.OnBeginDragSlot += HandleBeginDrag;
            slot.OnDragSlot += HandleDrag;
            slot.OnEndDragSlot += HandleEndDrag;
            slot.OnDropOnSlot += HandleDrop;

            uiSlots[i] = slot;
        }
    }

    public void Refresh()
    {
        if (uiSlots == null || inventory == null) return;

        for (int i = 0; i < uiSlots.Length; i++)
        {
            var item = (i < inventory.items.Count) ? inventory.items[i] : null;
            var amount = (i < inventory.amounts.Count) ? inventory.amounts[i] : 0;
            uiSlots[i].SetItem(item, amount);
        }

        UpdateHighlights();
    }

    private void HandleHoverEnter(int index)
    {
        if (isDragging) return;
        hoverIndex = index;
        UpdateHighlights();
    }

    private void HandleHoverExit(int index)
    {
        if (isDragging) return;
        if (hoverIndex == index) hoverIndex = -1;
        UpdateHighlights();
    }

    private void HandleClick(int index)
    {
        selectedIndex = index;
        UpdateHighlights();
    }

    private void UpdateHighlights()
    {
        if (uiSlots == null || inventory == null) return;

        for (int i = 0; i < uiSlots.Length; i++)
        {
            bool isHover = (i == hoverIndex);
            bool isSelected = (i == selectedIndex);

            if (isHover)
            {
                var item = (i < inventory.items.Count) ? inventory.items[i] : null;
                Texture hoverTex = (item != null) ? item.hoverHighlightIcon : null;
                uiSlots[i].SetHighlight(true, hoverTex);
            }
            else if (isSelected)
            {
                uiSlots[i].SetHighlight(true); // Perm Texture on the prefab
            }
            else
            {
                uiSlots[i].SetHighlight(false);
            }
        }
    }

    //  DRAG & DROP
    private void HandleBeginDrag(int fromIndex)
    {
        draggingIndex = fromIndex;

        // if there is no item > don't drag at all
        var item = (fromIndex < inventory.items.Count) ? inventory.items[fromIndex] : null;
        if (item == null)
        {
            draggingIndex = -1;
            return;
        }

        if (dragIcon != null)
        {
            dragIcon.texture = item.itemIcon;
            dragIcon.enabled = true;
            dragIcon.raycastTarget = false;
            dragIcon.transform.SetAsLastSibling();
            UpdateDragIconPosition();
        }
        isDragging = true;
        hoverIndex = -1;        // Clean stack "hover"
        UpdateHighlights();
    }

    private void HandleDrag(int fromIndex)
    {
        if (!isDragging) return;
        UpdateDragIconPosition();
    }

    private void HandleEndDrag(int fromIndex)
    {
        isDragging = false;
        draggingIndex = -1;

        if (dragIcon != null) dragIcon.enabled = false;

        hoverIndex = -1;     
        UpdateHighlights();
    }

    private void HandleDrop(int fromIndex, int toIndex)
    {
        // if we didn't drag
        if (fromIndex < 0 || toIndex < 0) return;
        if (fromIndex == toIndex) return;

        EnsureListSize(inventory.items, slotCount);
        EnsureListSize(inventory.amounts, slotCount);

        // Swap items
        var tmpItem = inventory.items[fromIndex];
        inventory.items[fromIndex] = inventory.items[toIndex];
        inventory.items[toIndex] = tmpItem;

        // Swap amounts
        var tmpAmt = inventory.amounts[fromIndex];
        inventory.amounts[fromIndex] = inventory.amounts[toIndex];
        inventory.amounts[toIndex] = tmpAmt;

        Refresh();

        hoverIndex = -1;
        UpdateHighlights();
    }

    private void UpdateDragIconPosition()
    {
        if (dragIcon == null) return;
        dragIcon.rectTransform.position = Input.mousePosition;
    }

    private void EnsureListSize<T>(System.Collections.Generic.List<T> list, int size)
    {
        while (list.Count < size) list.Add(default);
    }



    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
    }

}
