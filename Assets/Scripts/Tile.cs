using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{

    [Header("Colores")]
    public Color colorCorrect = ThemeColors.Correct;
    public Color colorPresent = ThemeColors.Present;
    public Color colorAbsent = ThemeColors.Absent;
    public Color colorEmpty = ThemeColors.Empty;

    
    private char currentLetter = ' ';

    
    private TMP_Text _letterText;
    private Image _background;

    
    private TMP_Text LetterText
    {
        get
        {
            if (_letterText == null)
                _letterText = GetComponentInChildren<TMP_Text>(true);
            return _letterText;
        }
    }

    
    private Image Background
    {
        get
        {
            if (_background == null)
                _background = GetComponent<Image>();
            return _background;
        }
    }

    
    public void SetLetter(char letter)
    {
        currentLetter = letter;
        LetterText.text = letter.ToString().ToUpper();
    }

    
    public void ClearLetter()
    {
        currentLetter = ' ';
        LetterText.text = "";
    }

    
    public char GetLetter()
    {
        return currentLetter;
    }

    
    public void SetResult(TileResult result)
    {
        switch (result)
        {
            case TileResult.Correct:
                
                Background.color = colorCorrect;
                break;
            case TileResult.Present:
                
                Background.color = colorPresent;
                break;
            case TileResult.Absent:
                
                Background.color = colorAbsent;
                break;
            case TileResult.Empty:
                
                Background.color = colorEmpty;
                break;
        }
    }

    
    public void Reset()
    {
        ClearLetter();
        Background.color = colorEmpty;
    }
}

public enum TileResult
{
    Empty,   
    Correct, 
    Present, 
    Absent   
}