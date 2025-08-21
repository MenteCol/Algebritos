using UnityEngine;

[System.Serializable]
public class AlgebraQuestions
{
    public string[] expressionParts; //Ejemplo: ["2", "+", "X", "=", "5"]
    public int missingIndex; //Indice del elemento faltante que se convertira en Dropzone
    public string[] options; //Opciones de respuesta (3 textos, una es correcta)
    public string questionText; //Texto de la pregunta
}

