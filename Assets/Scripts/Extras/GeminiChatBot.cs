using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro

public class GeminiChatBot : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputQuestion; // Asigna el InputField aqu�
    [SerializeField] private TMP_Text textResponse; // Asigna el Text aqu�
    [SerializeField] private Button buttonSend; // Asigna el Button aqu�

    private string apiKey = "AIzaSyA_aTcZ_5BumS0RoHEaMWykwkDpYfaI658"; // Reemplaza con tu key de Gemini

    private void Start()
    {
        buttonSend.onClick.AddListener(OnSendButtonClicked);
    }

    private void OnSendButtonClicked()
    {
        string question = inputQuestion.text.ToLower().Trim(); // Elimina espacios sobrantes
        if (string.IsNullOrEmpty(question)) return;

        // Validaci�n ampliada con �nfasis en "operaci�n combinada"
        string[] algebraTopics = new string[] {
            "�lgebra", "expresi�n", "algebraica", "polinomio", "monomio", "binomio", "trinomio", "grado",
            "t�rmino", "factor", "com�n", "agrupaci�n", "diferencia", "cuadrados", "cuadrado perfecto",
            "suma", "resta", "multiplicaci�n", "divisi�n", "simplificaci�n", "operaciones", "combinadas",
            "sistema", "variable", "partes", "ecuaci�n", "ecuaciones", "b�sico", "fundamentos", "tipos",
            "operaci�n", "combinada" // A�adido para asegurar detecci�n
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
            textResponse.text = "Lo siento, solo puedo responder preguntas sobre �lgebra seg�n los temas de Fundamentos Algebraicos, Casos Algebraicos y Operaciones B�sicas. Por favor, haz una pregunta relacionada con estos temas.";
            return;
        }

        textResponse.text = "Procesando...";
        StartCoroutine(SendRequestToGemini(question));
    }

    private IEnumerator SendRequestToGemini(string prompt)
    {
        // Instrucci�n reforzada con �nfasis en operaci�n combinada
        string systemInstruction = "Eres un asistente experto en �lgebra, limitado a los siguientes temas: \n" +
            "- Fundamentos Algebraicos: �Qu� es el �lgebra? (rama de las matem�ticas que usa s�mbolos como variables y reglas para resolver ecuaciones y estudiar relaciones), expresiones algebraicas, partes de una expresi�n algebraica (variables, t�rminos, coeficientes), polinomios, grados (el exponente m�s alto de una variable en un polinomio), tipos de expresiones (monomios, binomios, trinomios y polinomios).\n" +
            "- Casos Algebraicos: factorizaci�n (factor com�n, factor com�n por agrupaci�n, diferencia de cuadrados perfectos, trinomio cuadrado perfecto, trinomio de la forma x� + bx + c, trinomio de la forma ax� + bx + c).\n" +
            "- Operaciones B�sicas: suma y resta de expresiones algebraicas, multiplicaci�n y divisi�n de monomios y polinomios, simplificaci�n de expresiones, operaciones combinadas (expresiones con m�ltiples operaciones como suma, resta, multiplicaci�n y divisi�n, resueltas con PEMDAS).\n" +
            "Proporciona definiciones claras (ej: una operaci�n combinada es una expresi�n con m�ltiples operaciones resueltas por orden de prioridad), incluye ejemplos cuando sea posible, y rechaza cualquier pregunta fuera de estos temas con: 'Lo siento, solo puedo responder preguntas sobre �lgebra seg�n los temas de Fundamentos Algebraicos, Casos Algebraicos y Operaciones B�sicas. Por favor, formula una pregunta sobre estos temas.'";
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
                textResponse.text = "Error: " + www.error + " (Verifica tu API Key o conexi�n)";
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
                        textResponse.text = "No se recibi� una respuesta v�lida de la API.";
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