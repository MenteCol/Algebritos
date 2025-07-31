using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class GestionPreguntas : MonoBehaviour
{
    [Header("Referencias UI")]
    public Transform contentParent; // Content del ScrollView
    public GameObject questionItemPrefab; // Prefab con ItemsUI_Preguntas

    private List<RunTimeQuestion> loadedQuestions = new();
    private string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath + "/questions.json";
        LoadQuestionsFromFile();
    }

    private void OnEnable()
    {
        filePath = Application.persistentDataPath + "/questions.json";
        LoadQuestionsFromFile();
    }

    void LoadQuestionsFromFile()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Archivo de preguntas no encontrado.");
            return;
        }

        string json = File.ReadAllText(filePath);
        RunTimeQuestionList data = JsonUtility.FromJson<RunTimeQuestionList>(json);
        loadedQuestions = data.questions;
        RenderQuestionList();
    }

    void RenderQuestionList()
    {
        // Limpiar elementos previos en UI
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        for (int i = 0; i < loadedQuestions.Count; i++)
        {
            var question = loadedQuestions[i];

            GameObject itemGO = Instantiate(questionItemPrefab, contentParent);
            ItemsUI_Preguntas itemUI = itemGO.GetComponent<ItemsUI_Preguntas>();

            itemUI.numberInput.text = i.ToString();
            itemUI.questionText.text = question.questionText;
            itemUI.correctIndexInput.text = question.correctAnswerIndex.ToString();

            int currentIndex = i;

            itemUI.deleteButton.onClick.AddListener(() =>
            {
                DeleteQuestion(currentIndex);
            });

            itemUI.numberInput.onEndEdit.AddListener((newValue) =>
            {
                if (int.TryParse(newValue, out int newIndex))
                {
                    if (newIndex >= 0 && newIndex < loadedQuestions.Count && newIndex != currentIndex)
                    {
                        SwapQuestions(currentIndex, newIndex);
                    }
                    else
                    {
                        // Restaurar el número original si el nuevo no es válido
                        itemUI.numberInput.text = currentIndex.ToString();
                    }
                }
            });
        }
    }

    void DeleteQuestion(int index)
    {
        loadedQuestions.RemoveAt(index);
        SaveQuestionsToFile();
        RenderQuestionList();
    }

    void SwapQuestions(int from, int to)
    {
        RunTimeQuestion temp = loadedQuestions[from];
        loadedQuestions.RemoveAt(from);
        loadedQuestions.Insert(to, temp);

        SaveQuestionsToFile();
        RenderQuestionList();
    }

    void SaveQuestionsToFile()
    {
        RunTimeQuestionList data = new() { questions = loadedQuestions };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Preguntas reordenadas/actualizadas en: " + filePath);
    }
}
