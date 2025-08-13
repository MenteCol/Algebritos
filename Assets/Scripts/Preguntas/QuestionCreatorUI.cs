using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCreatorUI : MonoBehaviour
{
    public TMP_InputField questionInput;
    public TMP_InputField[] optionInputs;
    public int correctAnswer;
    public Toggle[] correctOptionToggles;
    public TMP_Dropdown levelDropdown;

    public TMP_Text textoError;
    public GameObject panelError;
    public Button gestionPreguntas;

    private List<RunTimeQuestion> questionList = new();

    void Start()
    {
        panelError.SetActive(false);
        ActualizarBotonGestion();

        #region Evitar mas de un Toggle On.
        for (int i = 0; i < correctOptionToggles.Length; i++)
        {
            int index = i;
            correctOptionToggles[i].onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    for (int j = 0; j < correctOptionToggles.Length; j++)
                    {
                        if (j != index)
                            correctOptionToggles[j].isOn = false;
                    }
                }
            });
        }
        #endregion

        // Opciones del dropdown de niveles
        if(levelDropdown != null)
        {
            levelDropdown.ClearOptions();
            levelDropdown.AddOptions(new List<string> { "Nivel 01", "Nivel 02", "Nivel 03" });
            levelDropdown.value = 0;
        }
    }

    public void SetCorrectIndex(int index)
    {
        correctAnswer = index;
    }

    public void SaveQuestion()
    {
        if (!IsFormValid(out string errorMsg))
        {
            textoError.text = errorMsg;
            panelError.SetActive(true);
            Debug.LogError("Error al guardar la pregunta: " + errorMsg);
            return;
        }

        RunTimeQuestion q = new();
        {
            q.questionText = questionInput.text;
            q.correctAnswerIndex = correctAnswer;
            q.options = new string[optionInputs.Length];
            q.level = levelDropdown != null ? levelDropdown.value + 1 : 1;
        }

        for (int i = 0; i < 4; i++)
        {
            q.options[i] = optionInputs[i].text;
        }

        string filePath = Application.persistentDataPath + "/questions.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            RunTimeQuestionList existingData = JsonUtility.FromJson<RunTimeQuestionList>(json);
            questionList = existingData.questions ?? new List<RunTimeQuestion>();
        }
        else
        {
            questionList = new List<RunTimeQuestion>();
        }

        questionList.Add(q);

        RunTimeQuestionList data = new() { questions = questionList };
        string jsonFinal = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, jsonFinal);

        Debug.Log("Pregunta guardada con nivel: " + q.level);
        ClearInputs();
        ActualizarBotonGestion();
    }

    void ClearInputs()
    {
        questionInput.text = "";
        foreach (var input in optionInputs)
        {
            input.text = "";
        }

        foreach (var toggle in correctOptionToggles)
        {
            toggle.isOn = false;
        }

        if (levelDropdown != null)
        {
            levelDropdown.value = 0;
        }
    }

    bool IsFormValid(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(questionInput.text))
        {
            errorMessage = "La pregunta está vacía.";
            return false;
        }

        for (int i = 0; i < optionInputs.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(optionInputs[i].text))
            {
                errorMessage = $"La opción {i + 1} está vacía.";
                return false;
            }
        }

        int togglesOn = 0;

        for (int i = 0; i < correctOptionToggles.Length; i++)
        {
            if (correctOptionToggles[i].isOn)
            {
                correctAnswer = i;
                togglesOn++;
            }
        }

        if (togglesOn != 1)
        {
            errorMessage = "Debe marcar una opción como correcta.";
            return false;
        }

        if(levelDropdown != null && levelDropdown.value < 0)
        {
            errorMessage = "Debe seleccionar un nivel.";
            return false;
        }

        errorMessage = "";
        return true;
    }

    public void ActualizarBotonGestion()
    {
        string filePath = Application.persistentDataPath + "/questions.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            RunTimeQuestionList existingData = JsonUtility.FromJson<RunTimeQuestionList>(json);

            if (existingData.questions != null && existingData.questions.Count > 0)
            {
                gestionPreguntas.interactable = true;
                return;
            }
        }

        gestionPreguntas.interactable = false;
    }

}
