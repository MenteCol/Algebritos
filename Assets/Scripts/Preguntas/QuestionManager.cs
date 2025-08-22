using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class QuestionManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text[] optionTexts;
    public Button[] optionButtons;
    public GameObject correctSprite;

    public int currentLevel; //1, 2, 3...

    private List<RunTimeQuestion> questions;
    private List<RunTimeQuestion> levelQuestions;
    private int currentIndex = 0;
    private int score = 0;

    void Start()
    {
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

        //Filtrar preguntas por nivel
        questions = data.questions ?? new List<RunTimeQuestion>();
        List<RunTimeQuestion> allLevelQuestions = questions.FindAll(q => q.level == currentLevel);

        if (allLevelQuestions.Count < 5)
        {
            Debug.LogError("No hay suficientes preguntas para el nivel");
            return;
        }

        //Escoger 5 preguntas aleatorias
        levelQuestions = new List<RunTimeQuestion>();
        List<int> usedIndexes = new List<int>();

        while(levelQuestions.Count < 5)
        {
            int rand = Random.Range(0, allLevelQuestions.Count);

            if(!usedIndexes.Contains(rand))
            {
                usedIndexes.Add(rand);
                levelQuestions.Add(allLevelQuestions[rand]);
            }
        }
    }

    void ShowQuestion()
    {
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

    void OnAnswer(int index) //Aqui se ejecuta la pregunta, HEREEEEEEE!!!
    {
        RunTimeQuestion q = levelQuestions[currentIndex];

        if(index == q.correctAnswerIndex)
        {
            score++;
            EjecucionSonidos.Instance.ReproducirAudio("SFX_Win");
            correctSprite.SetActive(true);
            //Debug.Log("Respuesta correcta!");
        }

        Invoke(nameof(NextQuestion), 4f);
    }

    void EndGame()
    {
        questionText.text = "Juego terminado. Tu puntuación es: " + score + "/5";

        foreach(var btn in optionButtons)
        {
            btn.gameObject.SetActive(false);
        }

        //Guardar puntaje
        PlayerPrefs.SetInt("Nivel" + currentLevel + "_Score", score);

        //Desbloquear siguiente nivel
        if(score >= 4 && currentLevel < 3)
        {
            PlayerPrefs.SetInt("Nivel" + (currentLevel + 1) + "_Unlocked", 1);
        }

        PlayerPrefs.Save();
        Invoke(nameof(ReturnToMainMenu), 3f); //Volver al menú principal después de 3 segundos
    }

    void NextQuestion()
    {
        currentIndex++;
        ShowQuestion();
        correctSprite.SetActive(false);
    }

    void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("04. Niveles");
    }
}
