using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] optionButtons;

    private List<RunTimeQuestion> questions;
    private int currentIndex = 0;
    private int score = 0;

    public void LoadQuestions(List<RunTimeQuestion> qList)
    {
        questions = qList;
        currentIndex = 0;
        score = 0;
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (currentIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        var q = questions[currentIndex];
        questionText.text = q.questionText;

        for(int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].GetComponentInChildren<TMP_Text>().text = q.options[i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswer(index));
        }
    }

    void OnAnswer(int index)
    {
        if (index == questions[currentIndex].correctAnswerIndex)
        {
            score++;
        }
        currentIndex++;
        ShowQuestion();
    }

    void EndGame()
    {
        questionText.text = "Juego terminado. Tu puntuación: " + score + "/" + questions.Count;
        foreach (var button in optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
}
