using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HackingMiniGame : MonoBehaviour
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver,
    }

    public enum ActionType
    {
        Horizontal,
        Vertical,
    }

    [SerializeField] private Canvas canvs = null;

    private GameState state;
    private float timer;
    private float timerToSet;
    private int actionRowColIndex;
    private ActionType actionType;
    private List<string> bufferHexList;
    private int bufferSize;
    private List<string> correctSequence;
    private string[,] grid;
    private float gridCellSize;

    private TextMeshProUGUI timerText;
    //private Image timerBar;

    private RectTransform cursorRectTransform;
    private Transform gridSingleTemplate;
    private Transform bufferSingleTemplate;
    private Transform bufferBackgroundTemplate;
    private Transform sequenceSingleTemplate;
    private Transform topTransform;
    
    private Transform gridHorizontalTransform;
    private Transform gridVerticalTransform;

    private void Awake()
    {
        state = GameState.WaitingToStart;
        Transform gridMask = transform.Find("gridMask");
        Transform gridContainer = gridMask.Find("gridContainer");
        gridSingleTemplate = gridContainer.Find("gridSingleTemplate");
        gridSingleTemplate.gameObject.SetActive(false);

        Transform bufferContainer = transform.Find("bufferContainer");
        bufferSingleTemplate = bufferContainer.Find("bufferSingleTemplate");
        bufferSingleTemplate.gameObject.SetActive(false);
        
        bufferBackgroundTemplate = bufferContainer.Find("bufferBackgroundTemplate");
        bufferBackgroundTemplate.gameObject.SetActive(false);

        Transform sequenceContainer = transform.Find("sequenceContainer");
        sequenceSingleTemplate = sequenceContainer.Find("sequenceSingleTemplate");
        sequenceSingleTemplate.gameObject.SetActive(false);

        topTransform = transform.Find("top");
        topTransform.gameObject.SetActive(false);

        gridHorizontalTransform = gridContainer.Find("gridHorizontalTransform");
        gridVerticalTransform = gridContainer.Find("gridVerticalTransform");

        timerText = transform.Find("timerText").GetComponent<TextMeshProUGUI>();
        //timerBar = transform.Find("timerBar").GetComponent<Image>();

        cursorRectTransform = transform.Find("cursor").GetComponent<RectTransform>();
        Cursor.visible = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        StartHackGame();
    }

   

    // Update is called once per frame
    void Update()
    {
        
    } 
    
    private void StartHackGame()
    { 
        throw new NotImplementedException(); 
    }
}
