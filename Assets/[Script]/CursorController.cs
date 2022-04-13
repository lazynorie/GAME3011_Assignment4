using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private HackingMiniGame hackingMinigameUI;
    private int x;
    private int y;
    
     public void Setup(HackingMiniGame hackingMinigameUI, int x, int y) {
        this.hackingMinigameUI = hackingMinigameUI;
        this.x = x;
        this.y = y;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        hackingMinigameUI.OnClicked(x, y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hackingMinigameUI.OnGridOver(x, y);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hackingMinigameUI.OnGridOut(x, y);
    }
}
