using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro

public class GeminiChatBot : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputQuestion; // Asigna el InputField aquí
    [SerializeField] private TMP_Text textResponse; // Asigna el Text aquí
    [SerializeField] private Button buttonSend; // Asigna el Button aquí

    private string apiKey = "AIzaSyA_aTcZ_5BumS0RoHEaMWykwkDpYfaI658"; // Reemplaza con tu key de Gemini

    private void Start()
    {
        buttonSend.onClick.AddListener(OnSendButtonClicked);
    }

    private void OnSendButtonClicked()
    {
        string question = inputQuestion.text.ToLower().Trim(); // Elimina espacios sobrantes
        if (string.IsNullOrEmpty(question)) return;

        // Validación ampliada con énfasis en "operación combinada"
        string[] algebraTopics = new string[] {
            "álgebra", "expresión", "algebraica", "polinomio", "monomio", "binomio", "trinomio", "grado",
            "término", "factor", "común", "agrupación", "diferencia", "cuadrados", "cuadrado perfecto",
            "suma", "resta", "multiplicación", "división", "simplificación", "operaciones", "combinadas",
            "sistema", "variable", "partes", "ecuación", "ecuaciones", "básico", "fundamentos", "tipos",
            "operación", "combinada" // Añadido para asegurar detección
        };

        bool isAlgebraRelated = false;
        foreach (string topic in algebraTopics)
        {
            if (question.Contains(topic))
            {
                isAlgebraRelated = true;
                break;
            }
        }

        if (!isAlgebraRelated)
        {
            textResponse.text = "Lo siento, solo puedo responder preguntas sobre álgebra según los temas de Fundamentos Algebraicos, Casos Algebraicos y Operaciones Básicas. Por favor, haz una pregunta relacionada con estos temas.";
            return;
        }

        textResponse.text = "Procesando...";
        StartCoroutine(SendRequestToGemini(question));
    }

    private IEnumerator SendRequestToGemini(string prompt)
    {
        // Instrucción reforzada con énfasis en operación combinada
        string systemInstruction = "Eres un asistente experto en álgebra, limitado a los siguientes temas: \n" +
            "- Fundamentos Algebraicos: ¿Qué es el álgebra? (rama de las matemáticas que usa símbolos como variables y reglas para resolver ecuaciones y estudiar relaciones), expresiones algebraicas, partes de una expresión algebraica (variables, términos, coeficientes), polinomios, grados (el exponente más alto de una variable en un polinomio), tipos de expresiones (monomios, binomios, trinomios y polinomios).\n" +
            "- Casos Algebraicos: factorización (factor común, factor común por agrupación, diferencia de cuadrados perfectos, trinomio cuadrado perfecto, trinomio de la forma x² + bx + c, trinomio de la forma ax² + bx + c).\n" +
            "- Operaciones Básicas: suma y resta de expresiones algebraicas, multiplicación y división de monomios y polinomios, simplificación de expresiones, operaciones combinadas (expresiones con múltiples operaciones como suma, resta, multiplicación y división, resueltas con PEMDAS).\n" +
            "Proporciona definiciones claras (ej: una operación combinada es una expresión con múltiples operaciones resueltas por orden de prioridad), incluye ejemplos cuando sea posible, y rechaza cualquier pregunta fuera de estos temas con: 'Lo siento, solo puedo responder preguntas sobre álgebra según los temas de Fundamentos Algebraicos, Casos Algebraicos y Operaciones Básicas. Por favor, formula una pregunta sobre estos temas.'";
        string jsonBody = $@"
        {{
            ""contents"": [{{
                ""parts"": [{{
                    ""text"": ""{systemInstruction}\n\nUsuario: {prompt}""
                }}]
            }}]
        }}";

        string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                textResponse.text = "Error: " + www.error + " (Verifica tu API Key o conexión)";
            }
            else
            {
                string responseJson = www.downloadHandler.text;
                try
                {
                    GenerateContentResponse response = JsonUtility.FromJson<GenerateContentResponse>(responseJson);
                    if (response.candidates != null && response.candidates.Length > 0 && response.candidates[0].content != null && response.candidates[0].content.parts != null && response.candidates[0].content.parts.Length > 0)
                    {
                        string answer = response.candidates[0].content.parts[0].text;
                        textResponse.text = answer;
                    }
                    else
                    {
                        textResponse.text = "No se recibió una respuesta válida de la API.";
                    }
                }
                catch (System.Exception e)
                {
                    textResponse.text = "Error al procesar la respuesta: " + e.Message;
                }
            }
        }
    }

    // Clases para parsear el JSON de respuesta
    [System.Serializable]
    private class GenerateContentResponse
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    private class Candidate
    {
        public Content content;
    }

    [System.Serializable]
    private class Content
    {
        public Part[] parts;
    }

    [System.Serializable]
    private class Part
    {
        public string text;
    }
}