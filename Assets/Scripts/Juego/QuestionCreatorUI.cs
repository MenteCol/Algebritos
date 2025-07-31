using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class QuestionCreatorUI : MonoBehaviour
{
    //public RunTimeQuestion runTimeQuestion;

    public TMP_InputField questionInput;
    public TMP_InputField[] optionInputs;
    public int correctAnswer;
    public Toggle[] correctOptionToggles;

    private List<RunTimeQuestion> questionList = new();

    public void SetCorrectIndex(int index)
    {
        correctAnswer = index;
    }

    //public void SaveQuestion()
    //{
    //    if(!IsFormValid())
    //    {
    //        Debug.LogError("Formulario inválido. Asegúrate de que todos los campos estén llenos y que una opción sea correcta.");
    //        return;
    //    }

    //    RunTimeQuestion q = new();
    //    {
    //        q.questionText = questionInput.text;
    //        q.correctAnswerIndex = correctAnswer;
    //        q.options = new string[4];
    //    };

    //    for (int i = 0; i < 4; i++)
    //    {
    //        q.options[i] = optionInputs[i].text;
    //    }

    //    questionList.Add(q);
    //    Debug.Log("Pregunta guardada: " + questionInput.text);
    //    ClearInputs();
    //}

    public void SaveQuestion()
    {
        if (!IsFormValid())
        {
            Debug.LogError("Formulario inválido.");
            return;
        }
                
        RunTimeQuestion q = new();
        {
            q.questionText = questionInput.text;
            q.correctAnswerIndex = correctAnswer;
            q.options = new string[4];
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
            questionList = existingData.questions;
        }
        else
        {
            questionList = new List<RunTimeQuestion>();
        }
                
        questionList.Add(q);

        RunTimeQuestionList data = new() { questions = questionList };
        string jsonFinal = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, jsonFinal);

        Debug.Log("Pregunta guardada y añadida al archivo.");
        ClearInputs();
    }


    public void SaveToFile()
    {
        RunTimeQuestionList data = new() { questions = questionList };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + "/questions.json", json);
        Debug.Log("Guardado en: " + Application.persistentDataPath);
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
    }

    bool IsFormValid()
    {
        if(string.IsNullOrWhiteSpace(questionInput.text))
        {
            return false;
        }

        foreach (var input in optionInputs)
        {
            if(string.IsNullOrWhiteSpace(input.text))
            {
                return false;
            }
        }

        int togglesOn = 0;
        for(int i = 0; i < correctOptionToggles.Length; i++)
        {
            if (correctOptionToggles[i].isOn)
            {
                correctAnswer = i;
                togglesOn++;
            }
        }

        return togglesOn == 1;
    }
}
