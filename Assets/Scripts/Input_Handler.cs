using UnityEngine;

public class Input_Handler : MonoBehaviour
{
    // Referencia a la grilla para enviarle el input del teclado físico
    private Tile_Grid tileGrid;

    // Solo procesamos input cuando hay una partida activa
    private bool gameActive = false;

    // String con todas las letras válidas para filtrar rápido
    private const string LETRAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // ── INICIALIZACIÓN ─────────────────────────────────────────────────────

    // Activa el handler al iniciar una partida
    public void Activate(Tile_Grid grid)
    {
        tileGrid = grid;
        gameActive = true;
    }

    // Desactiva el handler al terminar una partida
    public void Deactivate()
    {
        gameActive = false;
    }

    // ── UPDATE ─────────────────────────────────────────────────────────────

    void Update()
    {
        // Solo procesamos si el juego está activo
        if (!gameActive) return;
        if (tileGrid == null) return;

        // BACKSPACE → borrar última letra
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            tileGrid.DeleteLetter();
            return;
        }

        // ENTER o RETURN → confirmar fila
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            tileGrid.SubmitRow();
            return;
        }

        // Letras A-Z → detectamos con Input.inputString que es mucho más eficiente
        // que iterar todos los KeyCodes en cada frame
        foreach (char c in Input.inputString)
        {
            // Convertimos a mayúscula y verificamos que sea una letra válida
            char upper = char.ToUpper(c);
            if (LETRAS.IndexOf(upper) >= 0)
            {
                tileGrid.EnterLetter(upper);
                return; // una letra por frame
            }
        }
    }
}