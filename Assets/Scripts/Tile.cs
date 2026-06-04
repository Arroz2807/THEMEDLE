using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    // Colores del juego — ahora vienen de ThemeColors para ser consistentes
    [Header("Colores")]
    public Color colorCorrect = ThemeColors.Correct;
    public Color colorPresent = ThemeColors.Present;
    public Color colorAbsent = ThemeColors.Absent;
    public Color colorEmpty = ThemeColors.Empty;

    // La letra que contiene este tile en este momento
    private char currentLetter = ' ';

    // Referencias lazy: se obtienen la primera vez que se necesitan
    // Esto evita el problema de Awake no ejecutarse en objetos desactivados
    private TMP_Text _letterText;
    private Image _background;

    // Propiedad que obtiene el TMP_Text la primera vez y lo cachea
    private TMP_Text LetterText
    {
        get
        {
            if (_letterText == null)
                _letterText = GetComponentInChildren<TMP_Text>(true);
            return _letterText;
        }
    }

    // Propiedad que obtiene la Image la primera vez y la cachea
    private Image Background
    {
        get
        {
            if (_background == null)
                _background = GetComponent<Image>();
            return _background;
        }
    }

    // ── ESTADO DEL TILE ───────────────────────────────────────────────────

    // Muestra una letra en el tile (cuando el jugador tipea)
    public void SetLetter(char letter)
    {
        currentLetter = letter;
        LetterText.text = letter.ToString().ToUpper();
    }

    // Borra la letra del tile (cuando el jugador borra)
    public void ClearLetter()
    {
        currentLetter = ' ';
        LetterText.text = "";
    }

    // Devuelve la letra actual del tile
    public char GetLetter()
    {
        return currentLetter;
    }

    // Pinta el tile según el resultado de la evaluación
    public void SetResult(TileResult result)
    {
        switch (result)
        {
            case TileResult.Correct:
                // La letra está en la posición correcta → verde
                Background.color = colorCorrect;
                break;
            case TileResult.Present:
                // La letra existe en la palabra pero en otra posición → amarillo
                Background.color = colorPresent;
                break;
            case TileResult.Absent:
                // La letra no existe en la palabra → gris
                Background.color = colorAbsent;
                break;
            case TileResult.Empty:
                // Tile vacío sin evaluar
                Background.color = colorEmpty;
                break;
        }
    }

    // Resetea el tile a su estado inicial (fondo vacío, sin letra)
    public void Reset()
    {
        ClearLetter();
        Background.color = colorEmpty;
    }
}

// Enum para los posibles resultados de evaluación de un tile
public enum TileResult
{
    Empty,   // No evaluado aún
    Correct, // Letra en posición correcta → verde
    Present, // Letra presente en otra posición → amarillo
    Absent   // Letra no está en la palabra → gris
}