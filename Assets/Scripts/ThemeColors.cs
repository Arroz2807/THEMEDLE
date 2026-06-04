using UnityEngine;

// Clase estática que centraliza todos los colores del juego
// Tile.cs y Keyboard_Controller.cs leen de aquí para garantizar consistencia
public static class ThemeColors
{
    // Verde: letra en posición correcta — más brillante para compensar espacio gamma
    public static readonly Color Correct = new Color(0.325f, 0.706f, 0.325f);

    // Amarillo: letra presente pero en otra posición
    public static readonly Color Present = new Color(0.824f, 0.718f, 0.224f);

    // Gris oscuro: letra no está en la palabra
    public static readonly Color Absent = new Color(0.341f, 0.341f, 0.341f);

    // Gris muy oscuro: tile vacío sin evaluar
    public static readonly Color Empty = new Color(0.220f, 0.220f, 0.227f);

    // Gris medio: color inicial de las teclas del teclado
    public static readonly Color KeyDefault = new Color(0.506f, 0.518f, 0.529f);
}