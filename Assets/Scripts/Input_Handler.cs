using UnityEngine;

public class Input_Handler : MonoBehaviour
{
    
    private Tile_Grid tileGrid;

    
    private bool gameActive = false;

    
    private const string LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    
    public void Activate(Tile_Grid grid)
    {
        tileGrid = grid;
        gameActive = true;
    }

    
    public void Deactivate()
    {
        gameActive = false;
    }

    

    void Update()
    {
        
        if (!gameActive) return;
        if (tileGrid == null) return;

        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            tileGrid.DeleteLetter();
            return;
        }

        
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            tileGrid.SubmitRow();
            return;
        }

        
        foreach (char c in Input.inputString)
        {
            
            char upper = char.ToUpper(c);
            if (LETRAS.IndexOf(upper) >= 0)
            {
                tileGrid.EnterLetter(upper);
                return;
            }
        }
    }
}