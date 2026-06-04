using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Keyboard_Controller : MonoBehaviour
{
    [Header("Contenedor del teclado")]
    public Transform keyboardContainer; // el Keyboard_Container de la escena

    [Header("Prefab de tecla")]
    public GameObject keyPrefab; // botón simple con TMP_Text adentro

    // Referencia a la grilla para enviarle las letras
    private Tile_Grid tileGrid;

    // Guardamos los botones para poder pintarlos (feedback visual de teclado)
    private Dictionary<char, Image> keyImages = new Dictionary<char, Image>();

    // Las filas del teclado estilo QWERTY en español
    private string[] rows = new string[]
    {
        "QWERTYUIOP",
        "ASDFGHJKL",
        "ZXCVBNM"
    };

    // ── INICIALIZACIÓN ─────────────────────────────────────────────────────

    // Construye el teclado en la escena
    // Se llama desde Game_Manager al iniciar una partida
    public void BuildKeyboard(Tile_Grid grid)
    {
        tileGrid = grid;
        keyImages.Clear();

        // Limpiamos teclas viejas si existieran
        foreach (Transform child in keyboardContainer)
            Destroy(child.gameObject);

        // Por cada fila de letras, creamos una fila de botones
        foreach (string row in rows)
        {
            // Usamos el método CreateRow para crear la fila con tamaño correcto
            GameObject rowGO = CreateRow("Row_" + row[0]);

            // Creamos un botón por cada letra de la fila
            foreach (char letter in row)
                CreateKey(letter, rowGO.transform);
        }

        // Fila especial: BORRAR y CONFIRMAR
        CreateSpecialKeys();
    }

    // Crea un contenedor de fila con tamaño y layout correctos
    // Separado en su propio método para reutilizarlo en CreateSpecialKeys
    private GameObject CreateRow(string name)
    {
        // Creamos el GameObject solo con RectTransform primero
        GameObject rowGO = new GameObject(name, typeof(RectTransform));
        rowGO.transform.SetParent(keyboardContainer, false);

        // Le damos un tamaño fijo a la fila para que sea visible
        // Sin este tamaño, las filas quedan con alto cero y no se ven
        RectTransform rt = rowGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(900f, 72f);

        // Agregamos el HorizontalLayoutGroup para alinear las teclas en fila
        HorizontalLayoutGroup hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 4f;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        // Layout Element para que el Vertical Layout Group del contenedor
        // respete la altura de cada fila
        LayoutElement le = rowGO.AddComponent<LayoutElement>();
        le.minHeight = 72f;
        le.preferredHeight = 72f;

        return rowGO;
    }

    // Crea un botón de letra
    private void CreateKey(char letter, Transform parent)
    {
        // Instanciamos el prefab de tecla
        GameObject keyGO = Instantiate(keyPrefab, parent);
        keyGO.name = "Key_" + letter;

        // Obtenemos el texto y lo seteamos
        TMP_Text txt = keyGO.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.text = letter.ToString();

        // Guardamos la imagen y le damos el color inicial uniforme
        // Usamos ThemeColors para consistencia con los tiles
        Image img = keyGO.GetComponent<Image>();
        if (img != null)
        {
            img.color = ThemeColors.KeyDefault; // gris medio inicial
            keyImages[letter] = img;
        }

        // Asignamos el listener del botón: al tocar envía la letra a la grilla
        Button btn = keyGO.GetComponent<Button>();
        if (btn != null)
        {
            char captured = letter; // capturamos la variable para el closure
            btn.onClick.AddListener(() => OnLetterPressed(captured));
        }
    }

    // Crea los botones especiales BORRAR y ENTER en su propia fila
    private void CreateSpecialKeys()
    {
        // Usamos CreateRow igual que las filas de letras
        GameObject specialRowGO = CreateRow("Row_Special");

        // Botón BORRAR
        GameObject deleteGO = Instantiate(keyPrefab, specialRowGO.transform);
        deleteGO.name = "Key_Delete";
        RectTransform deleteRT = deleteGO.GetComponent<RectTransform>();
        deleteRT.sizeDelta = new Vector2(96f, 72f);
        TMP_Text deleteTxt = deleteGO.GetComponentInChildren<TMP_Text>();
        // Usamos "DEL" en lugar del símbolo ⌫ porque la fuente por defecto no lo soporta
        if (deleteTxt != null) deleteTxt.text = "←";
        Button deleteBtn = deleteGO.GetComponent<Button>();
        if (deleteBtn != null) deleteBtn.onClick.AddListener(OnDeletePressed);

        // Botón CONFIRMAR
        GameObject enterGO = Instantiate(keyPrefab, specialRowGO.transform);
        enterGO.name = "Key_Enter";
        RectTransform enterRT = enterGO.GetComponent<RectTransform>();
        enterRT.sizeDelta = new Vector2(96f, 72f);
        TMP_Text enterTxt = enterGO.GetComponentInChildren<TMP_Text>();
        if (enterTxt != null) enterTxt.text = "OK";
        Button enterBtn = enterGO.GetComponent<Button>();
        if (enterBtn != null) enterBtn.onClick.AddListener(OnEnterPressed);
    }

    // ── HANDLERS DE INPUT ──────────────────────────────────────────────────

    // El jugador tocó una tecla de letra
    private void OnLetterPressed(char letter)
    {
        tileGrid.EnterLetter(letter);
    }

    // El jugador tocó BORRAR
    private void OnDeletePressed()
    {
        tileGrid.DeleteLetter();
    }

    // El jugador tocó CONFIRMAR
    private void OnEnterPressed()
    {
        tileGrid.SubmitRow();
    }

    // ── FEEDBACK VISUAL DEL TECLADO ───────────────────────────────────────

    // Colorea una tecla según el resultado obtenido
    // Se llama desde Tile_Grid.EvaluateRow después de cada intento
    // Respeta la prioridad: Correct (verde) nunca se pisa con Present o Absent
    public void UpdateKeyColor(char letter, TileResult result)
    {
        if (!keyImages.ContainsKey(letter)) return;

        Image img = keyImages[letter];

        // Comparamos prioridades para no pisar un color mejor con uno peor
        // Verde (2) > Amarillo (1) > Gris (0)
        int prioActual = GetColorPriority(img.color);
        int prioNuevo = GetResultPriority(result);

        // Solo pintamos si el nuevo resultado tiene más prioridad que el actual
        if (prioNuevo <= prioActual) return;

        // Aplicamos el color usando ThemeColors para consistencia con los tiles
        switch (result)
        {
            case TileResult.Correct: img.color = ThemeColors.Correct; break; // verde
            case TileResult.Present: img.color = ThemeColors.Present; break; // amarillo
            case TileResult.Absent: img.color = ThemeColors.Absent; break; // gris
        }
    }

    // Devuelve la prioridad numérica de un TileResult
    // Correct=3, Present=2, Absent=1
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

    // Devuelve la prioridad del color actual de una tecla comparando con ThemeColors
    // Esto permite saber si la tecla ya fue evaluada y con qué resultado
    private int GetColorPriority(Color color)
    {
        // Comparamos con los colores de ThemeColors con una tolerancia pequeña
        if (ColorsMatch(color, ThemeColors.Correct)) return 3; // ya es verde
        if (ColorsMatch(color, ThemeColors.Present)) return 2; // ya es amarillo
        if (ColorsMatch(color, ThemeColors.Absent)) return 1; // ya es gris
        return 0; // key default, o nunca fue evaluada
    }

    // Compara dos colores con una tolerancia para evitar problemas de precisión float
    private bool ColorsMatch(Color a, Color b)
    {
        float tolerancia = 0.05f;
        return Mathf.Abs(a.r - b.r) < tolerancia &&
               Mathf.Abs(a.g - b.g) < tolerancia &&
               Mathf.Abs(a.b - b.b) < tolerancia;
    }
}