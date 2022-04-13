using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [SerializeField] private Canvas canvas = null;

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

        /*topTransform = transform.Find("top");
        topTransform.gameObject.SetActive(false);*/

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
        cursorRectTransform.anchoredPosition = Input.mousePosition / canvas.scaleFactor;

        switch (state) {
            case GameState.WaitingToStart:
                break;
            case GameState.Playing:
                timer -= Time.deltaTime;
                timerText.text = timer.ToString("F2");
                //timerBar.fillAmount = timer / timerMax;
                break;
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            StartHackGame();
        }
    } 
    
    private void StartHackGame()
    { 
        List<string> allSequencePossibleValues = new List<string>() { "E9", "1C", "55", "BD" };
        List<string> sequencePossibleValues = new List<string>();
        int possibleValuesCount = 4;
        for (int i = 0; i < possibleValuesCount; i++) {
            int rnd = Random.Range(0, allSequencePossibleValues.Count);
            sequencePossibleValues.Add(allSequencePossibleValues[rnd]);
            allSequencePossibleValues.RemoveAt(rnd);
        }

        // Generate correct sequence
        correctSequence = new List<string>();
        int sequenceLength = 3;
        for (int i = 0; i < sequenceLength; i++) {
            correctSequence.Add(sequencePossibleValues[Random.Range(0, sequencePossibleValues.Count)]);
        }

        // Initialize
        bufferHexList = new List<string>();
        bufferSize = 5;

        //topTransform.gameObject.SetActive(false);

        state = GameState.WaitingToStart;
        timerToSet = 30f;
        timer = timerToSet;
        timerText.text = timer.ToString("F2");
        //timerBar.fillAmount = timer / timerMax;

        actionRowColIndex = 0;
        actionType = ActionType.Horizontal;

        int gridWidth = 5;
        int gridHeight = 5;
        grid = new string[gridWidth, gridHeight];
        gridCellSize = 42.5f;

        // Setup grid
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                grid[x, y] = sequencePossibleValues[Random.Range(0, sequencePossibleValues.Count)];
            }
        }

        ForceValidSequence();

        PrintGrid();
        PrintSequence();
        PrintBuffer();
        RepositionHorizontalVerticalTransforms();
    }

    private void RepositionHorizontalVerticalTransforms()
    {
        /*Color colorA = UtilsClass.GetColorFromString("426873");
        colorA.a = .4f;

        Color colorB = UtilsClass.GetColorFromString("98924D");
        colorB.a = .4f;*/

        switch (actionType) {
            default:
            case ActionType.Horizontal:
                gridHorizontalTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -actionRowColIndex * gridCellSize);
                
                gridHorizontalTransform.gameObject.SetActive(true);
                gridVerticalTransform.gameObject.SetActive(false);

                /*gridHorizontalTransform.GetComponent<Image>().color = colorB;
                gridVerticalTransform.GetComponent<Image>().color = colorA;*/
                break;
            case ActionType.Vertical:
                gridVerticalTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(actionRowColIndex * gridCellSize, 0f);

                gridHorizontalTransform.gameObject.SetActive(false);
                gridVerticalTransform.gameObject.SetActive(true);

                /*gridHorizontalTransform.GetComponent<Image>().color = colorA;
                gridVerticalTransform.GetComponent<Image>().color = colorB;*/
                break;
        }
    }

    private void PrintBuffer()
    {
        Transform bufferContainer = transform.Find("bufferContainer");
        foreach (Transform child in bufferContainer) {
            if (child == bufferSingleTemplate) continue;
            if (child == bufferBackgroundTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < bufferHexList.Count; i++) {
            Transform singleTransform = Instantiate(bufferSingleTemplate, bufferSingleTemplate.parent);
            singleTransform.gameObject.SetActive(true);
            float gridCellSize = 30f;
            singleTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0) * gridCellSize;
            singleTransform.Find("text").GetComponent<TextMeshProUGUI>().text = bufferHexList[i];
        }

        for (int i = 0; i < bufferSize; i++) {
            Transform singleTransform = Instantiate(bufferBackgroundTemplate, bufferSingleTemplate.parent);
            singleTransform.gameObject.SetActive(true);
            float gridCellSize = 30f;
            singleTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0) * gridCellSize;
        }
    }

    private void PrintSequence()
    {
        Transform sequenceContainer = transform.Find("sequenceContainer");
        foreach (Transform child in sequenceContainer) {
            if (child == sequenceSingleTemplate) continue;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < correctSequence.Count; i++) {
            Transform singleTransform = Instantiate(sequenceSingleTemplate, sequenceSingleTemplate.parent);
            singleTransform.gameObject.SetActive(true);
            float gridCellSize = 30f;
            singleTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0) * gridCellSize;
            singleTransform.Find("text").GetComponent<TextMeshProUGUI>().text = correctSequence[i];
        }
    }

    private void PrintGrid()
    {
        Transform gridMask = transform.Find("gridMask");
        Transform gridContainer = gridMask.Find("gridContainer");
        foreach (Transform child in gridContainer) {
            if (child == gridSingleTemplate) continue;
            if (child == gridHorizontalTransform) continue;
            if (child == gridVerticalTransform) continue;
            Destroy(child.gameObject);
        }

        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                Transform gridSingleTransform = Instantiate(gridSingleTemplate, gridSingleTemplate.parent);
                gridSingleTransform.gameObject.SetActive(true);
                gridSingleTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, -y) * gridCellSize;
                gridSingleTransform.Find("text").GetComponent<TextMeshProUGUI>().text = grid[x, y];
                gridSingleTransform.GetComponent<CursorController>().Setup(this, x, y);
            }
        }
    }

    private void ForceValidSequence()
    {
        int gridWidth = 5;
        int gridHeight = 5;
        bool isHorizontal = true;
        int lastRowCol = 0;

        List<Vector2Int> usedSequencePositions = new List<Vector2Int>();

        for (int i = 0; i < correctSequence.Count; i++) {
            if (isHorizontal) {
                int rowCol;
                do {
                    rowCol = Random.Range(0, gridWidth);
                } while (usedSequencePositions.Contains(new Vector2Int(rowCol, lastRowCol)));

                usedSequencePositions.Add(new Vector2Int(rowCol, lastRowCol));

                grid[rowCol, lastRowCol] = correctSequence[i];
                lastRowCol = rowCol;
            } else {
                int rowCol;
                do {
                    rowCol = Random.Range(0, gridHeight);
                } while (usedSequencePositions.Contains(new Vector2Int(lastRowCol, rowCol)));

                usedSequencePositions.Add(new Vector2Int(lastRowCol, rowCol));

                grid[lastRowCol, rowCol] = correctSequence[i];
                lastRowCol = rowCol;
            }
            isHorizontal = !isHorizontal;
        }
    }

    public void OnClicked(int x, int y)
    {
        //Debug.Log(x + ", " + y + ": " + grid[x, y]);

        if (state == GameState.GameOver) return;

        // Valid click?
        if (IsValidClick(x, y)) {
            state = GameState.Playing;

            AddToBuffer(grid[x, y]);

            if (actionType == ActionType.Horizontal) {
                actionType = ActionType.Vertical;
                actionRowColIndex = x;
            } else {
                actionType = ActionType.Horizontal;
                actionRowColIndex = y;
            }
            RepositionHorizontalVerticalTransforms();

            grid[x, y] = "[  ]";
            PrintGrid();
            PrintBuffer();
            TestWinSequence();
        }
    }

    private void TestWinSequence()
    {
        // Test if correct sequence was inputted
        if (bufferHexList.Count >= correctSequence.Count) {
            // Buffer has at least the same number of elements as sequence
            if (bufferHexList.Contains(correctSequence[correctSequence.Count - 1])) {
                // Buffer contains last sequence
                int bufferLastIndex = bufferHexList.LastIndexOf(correctSequence[correctSequence.Count - 1]);

                bool correct = true;
                for (int i = 0; i < correctSequence.Count; i++) {
                    if (bufferLastIndex - i < 0) {
                        correct = false;
                        break;
                    }
                    if (correctSequence[correctSequence.Count - 1 - i] != bufferHexList[bufferLastIndex - i]) {
                        // Does not match!
                        correct = false;
                        break;
                    }
                }

                if (correct) {
                    // All correct!
                    Debug.Log("Correct!");
                    //topTransform.gameObject.SetActive(true);
                    state = GameState.GameOver;
                } else {
                    // Not correct
                    // Keep going or is buffer full?
                    if (IsBufferFull()) {
                        // Game Over!
                        Debug.Log("GameOver!");
                        state = GameState.GameOver;
                    } else {
                        // Buffer not full, keep playing
                    }
                }
            } else {
                // First correct sequence hex not found in buffer!
                // Is buffer full?
                if (IsBufferFull()) {
                    // Game Over!
                    Debug.Log("GameOver!");
                    state = GameState.GameOver;
                } else {
                    // Buffer not full, keep playing
                }
            }
        }
    }

    private bool IsBufferFull()
    {
        return bufferHexList.Count >= bufferSize;
    }

    private void AddToBuffer(string hex)
    {
        bufferHexList.Add(hex);
    }

    private bool IsValidClick(int x, int y)
    {
        if (grid[x, y] == "[  ]") return false; // Already clicked

        switch (actionType) {
            default:
            case ActionType.Horizontal:
                return y == actionRowColIndex;
            case ActionType.Vertical:
                return x == actionRowColIndex;
        }
    }

    public void OnGridOver(int x, int y)
    {
        switch (actionType) {
            default:
            case ActionType.Horizontal:
                gridVerticalTransform.gameObject.SetActive(true);
                gridVerticalTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * gridCellSize, 0f);
                break;
            case ActionType.Vertical:
                gridHorizontalTransform.gameObject.SetActive(true);
                gridHorizontalTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -y * gridCellSize);
                break;
        }
    }

    public void OnGridOut(int x, int y)
    {
        switch (actionType) {
            default:
            case ActionType.Horizontal:
                gridVerticalTransform.gameObject.SetActive(false);
                break;
            case ActionType.Vertical:
                gridHorizontalTransform.gameObject.SetActive(false);
                break;
        }
    }
}
