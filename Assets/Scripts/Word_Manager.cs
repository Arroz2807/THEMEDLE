using UnityEngine;
using System.Collections.Generic;

public class Word_Manager : MonoBehaviour
{

    public string SecretWord { get; private set; }


    public void SelectWord(string category, string difficulty)
    {

        Debug.Log("SelectWord recibió: categoria='" + category + "' dificultad='" + difficulty + "'");


        string fileName = BuildFileName(category, difficulty);


        Debug.Log("Intentando cargar: 'Palabras/" + fileName + "'");


        TextAsset textAsset = Resources.Load<TextAsset>("Palabras/" + fileName);

        if (textAsset == null)
        {

            Debug.LogError("NO encontrado: 'Palabras/" + fileName +
                "' — verificá que el archivo se llame exactamente '" + fileName + ".txt'");
            SecretWord = "GATO";
            return;
        }


        string[] lineas = textAsset.text.Split(
            new char[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );


        List<string> palabras = new List<string>();
        foreach (string linea in lineas)
        {

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


        int index = Random.Range(0, palabras.Count);
        SecretWord = palabras[index];

        Debug.Log("Palabra seleccionada: '" + SecretWord + "' (archivo: " + fileName + ")");
    }


    private string BuildFileName(string category, string difficulty)
    {

        string cat = category.ToLower().Trim();
        string dif = difficulty.ToLower().Trim();


        Debug.Log("BuildFileName — cat='" + cat + "' dif='" + dif + "'");

        string catNorm;
        if (cat.Contains("animal")) catNorm = "animales";
        else if (cat.Contains("pais")) catNorm = "paises";
        else if (cat.Contains("comida")) catNorm = "comidas";
        else catNorm = "animales";


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