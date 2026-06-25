using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Keyboard_Controller : MonoBehaviour
{
    [Header("Contenedor del teclado")]
    public Transform keyboardContainer; 

    [Header("Prefab de tecla")]
    public GameObject keyPrefab; 

    
    private Tile_Grid tileGrid;

    
    private Dictionary<char, Image> keyImages = new Dictionary<char, Image>();

    
    private string[] rows = new string[]
    {
        "QWERTYUIOP",
        "ASDFGHJKL",
        "ZXCVBNM"
    };

    
    public void BuildKeyboard(Tile_Grid grid)
    {
        tileGrid = grid;
        keyImages.Clear();

        
        foreach (Transform child in keyboardContainer)
            Destroy(child.gameObject);

        
        foreach (string row in rows)
        {
            
            GameObject rowGO = CreateRow("Row_" + row[0]);

            
            foreach (char letter in row)
                CreateKey(letter, rowGO.transform);
        }

        CreateSpecialKeys();
    }


    private GameObject CreateRow(string name)
    {
        GameObject rowGO = new GameObject(name, typeof(RectTransform));
        rowGO.transform.SetParent(keyboardContainer, false);

        RectTransform rt = rowGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(900f, 72f);

        HorizontalLayoutGroup hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 4f;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        LayoutElement le = rowGO.AddComponent<LayoutElement>();
        le.minHeight = 72f;
        le.preferredHeight = 72f;

        return rowGO;
    }


    private void CreateKey(char letter, Transform parent)
    {

        GameObject keyGO = Instantiate(keyPrefab, parent);
        keyGO.name = "Key_" + letter;


        TMP_Text txt = keyGO.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.text = letter.ToString();


        Image img = keyGO.GetComponent<Image>();
        if (img != null)
        {
            img.color = ThemeColors.KeyDefault; 
            keyImages[letter] = img;
        }


        Button btn = keyGO.GetComponent<Button>();
        if (btn != null)
        {
            char captured = letter;
            btn.onClick.AddListener(() => OnLetterPressed(captured));
        }
    }


    private void CreateSpecialKeys()
    {

        GameObject specialRowGO = CreateRow("Row_Special");


        GameObject deleteGO = Instantiate(keyPrefab, specialRowGO.transform);
        deleteGO.name = "Key_Delete";
        RectTransform deleteRT = deleteGO.GetComponent<RectTransform>();
        deleteRT.sizeDelta = new Vector2(96f, 72f);
        TMP_Text deleteTxt = deleteGO.GetComponentInChildren<TMP_Text>();

        if (deleteTxt != null) deleteTxt.text = "←";
        Button deleteBtn = deleteGO.GetComponent<Button>();
        if (deleteBtn != null) deleteBtn.onClick.AddListener(OnDeletePressed);


        GameObject enterGO = Instantiate(keyPrefab, specialRowGO.transform);
        enterGO.name = "Key_Enter";
        RectTransform enterRT = enterGO.GetComponent<RectTransform>();
        enterRT.sizeDelta = new Vector2(96f, 72f);
        TMP_Text enterTxt = enterGO.GetComponentInChildren<TMP_Text>();
        if (enterTxt != null) enterTxt.text = "OK";
        Button enterBtn = enterGO.GetComponent<Button>();
        if (enterBtn != null) enterBtn.onClick.AddListener(OnEnterPressed);
    }


    private void OnLetterPressed(char letter)
    {
        tileGrid.EnterLetter(letter);
    }


    private void OnDeletePressed()
    {
        tileGrid.DeleteLetter();
    }


    private void OnEnterPressed()
    {
        tileGrid.SubmitRow();
    }


    public void UpdateKeyColor(char letter, TileResult result)
    {
        if (!keyImages.ContainsKey(letter)) return;

        Image img = keyImages[letter];


        int prioActual = GetColorPriority(img.color);
        int prioNuevo = GetResultPriority(result);


        if (prioNuevo <= prioActual) return;


        switch (result)
        {
            case TileResult.Correct: img.color = ThemeColors.Correct; break; 
            case TileResult.Present: img.color = ThemeColors.Present; break;
            case TileResult.Absent: img.color = ThemeColors.Absent; break;
        }
    }


    private int GetResultPriority(TileResult result)
    {
        switch (result)
        {
            case TileResult.Correct: return 3;
            case TileResult.Present: return 2;
            case TileResult.Absent: return 1;
            default: return 0;
        }
    }


    private int GetColorPriority(Color color)
    {

        if (ColorsMatch(color, ThemeColors.Correct)) return 3; 
        if (ColorsMatch(color, ThemeColors.Present)) return 2; 
        if (ColorsMatch(color, ThemeColors.Absent)) return 1;
        return 0; // key default, o nunca fue evaluada
    }


    private bool ColorsMatch(Color a, Color b)
    {
        float tolerancia = 0.05f;
        return Mathf.Abs(a.r - b.r) < tolerancia &&
               Mathf.Abs(a.g - b.g) < tolerancia &&
               Mathf.Abs(a.b - b.b) < tolerancia;
    }
}