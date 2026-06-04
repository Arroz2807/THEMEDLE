using UnityEngine;
using System.Collections.Generic;

public class Word_Manager : MonoBehaviour
{
    // La palabra secreta actual que el jugador debe adivinar
    public string SecretWord { get; private set; }

    // ── SELECCIÓN DE PALABRA ──────────────────────────────────────────────

    // Recibe la categoría y dificultad elegidas en Panel_SelectionMenu
    // y elige una palabra al azar del archivo de texto correspondiente
    public void SelectWord(string category, string difficulty)
    {
        // Log de diagnóstico: muestra exactamente qué recibimos
        Debug.Log("SelectWord recibió: categoria='" + category + "' dificultad='" + difficulty + "'");

        // Construimos el nombre del archivo según categoría y dificultad
        string fileName = BuildFileName(category, difficulty);

        // Log de diagnóstico: muestra qué archivo intenta cargar
        Debug.Log("Intentando cargar: 'Palabras/" + fileName + "'");

        // Cargamos el archivo desde la carpeta Resources/Palabras
        TextAsset textAsset = Resources.Load<TextAsset>("Palabras/" + fileName);

        if (textAsset == null)
        {
            // Log de error con el nombre exacto para poder compararlo con el archivo real
            Debug.LogError("NO encontrado: 'Palabras/" + fileName +
                "' — verificá que el archivo se llame exactamente '" + fileName + ".txt'");
            SecretWord = "GATO";
            return;
        }

        // Dividimos el contenido del archivo en líneas, una palabra por línea
        // Ignoramos líneas vacías y espacios en blanco
        string[] lineas = textAsset.text.Split(
            new char[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        // Limpiamos cada línea (quitamos espacios extra y caracteres invisibles)
        List<string> palabras = new List<string>();
        foreach (string linea in lineas)
        {
            // Trim() saca espacios, \t, y otros whitespace del inicio y fin
            string palabra = linea.Trim().ToUpper();
            if (palabra.Length > 0)
                palabras.Add(palabra);
        }

        if (palabras.Count == 0)
        {
            Debug.LogWarning("El archivo " + fileName + ".txt está vacío");
            SecretWord = "GATO";
            return;
        }

        // Elegimos una palabra al azar de la lista
        int index = Random.Range(0, palabras.Count);
        SecretWord = palabras[index];

        Debug.Log("Palabra seleccionada: '" + SecretWord + "' (archivo: " + fileName + ")");
    }

    // Construye el nombre del archivo a partir de categoría y dificultad
    private string BuildFileName(string category, string difficulty)
    {
        // Convertimos a minúsculas y quitamos espacios
        string cat = category.ToLower().Trim();
        string dif = difficulty.ToLower().Trim();

        // Log para ver exactamente qué strings estamos comparando
        Debug.Log("BuildFileName — cat='" + cat + "' dif='" + dif + "'");

        // Normalizamos la categoría
        string catNorm;
        if (cat.Contains("animal")) catNorm = "animales";
        else if (cat.Contains("pais")) catNorm = "paises";
        else if (cat.Contains("comida")) catNorm = "comidas";
        else catNorm = "animales";

        // Normalizamos la dificultad
        // Nota: también contemplamos el string con acento "fácil" y "difícil"
        string difNorm;
        if (dif.Contains("facil") || dif.Contains("f\u00e1cil")) difNorm = "facil";
        else if (dif.Contains("medio")) difNorm = "medio";
        else if (dif.Contains("dificil") || dif.Contains("dif\u00edcil")) difNorm = "dificil";
        else difNorm = "facil";

        string resultado = catNorm + "_" + difNorm;
        Debug.Log("BuildFileName resultado: '" + resultado + "'");
        return resultado;
    }
}