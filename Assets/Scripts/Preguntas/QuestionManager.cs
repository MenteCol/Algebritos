using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class QuestionManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text[] optionTexts;
    public Button[] optionButtons;

    private List<RunTimeQuestion> questions;
    private List<RunTimeQuestion> levelQuestions;
    private int currentIndex = 0;
    private int score = 0;
    private int currentLevel;

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("Nivel Actual", 1); // Obtener el nivel actual del jugador
        LoadQuestions();
        ShowQuestion();
    }

    public void LoadQuestions()
    {
        string filePath = Application.persistentDataPath + "/questions.json";

        if(!File.Exists(filePath))
        {
            Debug.LogError("No se encontró el archivo de preguntas.");
            return;
        }

        string json = File.ReadAllText(filePath);
        RunTimeQuestionList data = JsonUtility.FromJson<RunTimeQuestionList>(json);

        //Cargar todas las preguntas
        questions = data.questions ?? new List<RunTimeQuestion>();

        //Filtrar preguntas por nivel
        levelQuestions = questions.FindAll(q => q.level == currentLevel);

        //Mezclar aleatoriamente
        for(int i = 0; i < levelQuestions.Count; i++)
        {
            RunTimeQuestion temp = levelQuestions[i];
            int randomIndex = UnityEngine.Random.Range(i, levelQuestions.Count);
            levelQuestions[i] = levelQuestions[randomIndex];
            levelQuestions[randomIndex] = temp;
        }

        //Limitar a 5 preguntas por nivel
        if (levelQuestions.Count > 5)
        {
            levelQuestions = levelQuestions.GetRange(0, 5);
        }

        if (levelQuestions.Count == 0)
        {
            Debug.LogError("No hay preguntas para el nivel " + currentLevel);
            return;
        }
    }

    void ShowQuestion()
    {
        if(levelQuestions.Count == 0)
        {
            questionText.text = "No hay preguntas disponibles para este nivel.";
            return;
        }

        if(currentIndex >= levelQuestions.Count)
        {
            EndGame();
            return;
        }

        RunTimeQuestion q = levelQuestions[currentIndex];
        questionText.text = q.questionText;

        for(int i = 0; i < optionTexts.Length; i++)
        {
            optionTexts[i].text = q.options[i];
            int index = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswer(index));
        }
    }

    void OnAnswer(int index)
    {
        RunTimeQuestion q = levelQuestions[currentIndex];

        if(index == q.correctAnswerIndex)
        {
            score++;
            Debug.Log("Respuesta correcta!");
        }
        else
        {
            Debug.Log("Respuesta incorrecta. La respuesta correcta era: " + q.options[q.correctAnswerIndex]);
        }

        currentIndex++;
        ShowQuestion();
    }

    void EndGame()
    {
        questionText.text = "Juego terminado. Tu puntuación: " + score + "/" + levelQuestions.Count;

        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if(score > 0 && currentLevel >= unlockedLevel && currentLevel < 3)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            Debug.Log("¡Nuevo nivel desbloqueado!");
        }
        
        PlayerPrefs.Save();

        //Volver al menú principal después de 3 segundos
        Invoke("ReturnToMainMenu", 3f);
    }

    void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("04. Niveles");
    }
}
