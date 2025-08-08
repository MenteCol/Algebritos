using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DragQuestion
{
    public string questionText;
    public Sprite[] optionsSprites;
    public int correctIndex;
}

public class DragDropManager : MonoBehaviour
{
    public TMP_Text questionText;
    public GameObject[] optionObjects;
    public DropZone dropZone;

    public List<DragQuestion> questions;
    private int currentQuestionIndex = 0;
    private int score = 0;

    void Start()
    {
        LoadQuestion();
    }

    public void CheckAnswer(Option option)
    {
        if (option.isCorrect)
        {
            score++;
            Debug.Log("Correct! Score: " + score);
            dropZone.Blink(Color.green);
        }
        else
        {
            Debug.Log("Incorrect! Score: " + score);
            dropZone.Blink(Color.red);
        }
        Invoke(nameof(NextQuestion), 1f);
    }

    void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex < questions.Count)
        {
            LoadQuestion();
        }
        else
        {
            Debug.Log("Quiz completed! Final Score: " + score);
        }
    }

    void LoadQuestion()
    {
        DragQuestion q = questions[currentQuestionIndex];
        questionText.text = q.questionText;

        for(int i = 0; i < optionObjects.Length; i++)
        {
            Option opt = optionObjects[i].GetComponent<Option>();
            opt.ResetPosition();
            Image img = optionObjects[i].GetComponent<Image>();
            img.sprite = q.optionsSprites[i];
            opt.isCorrect = (i == q.correctIndex);
        }

        foreach(Transform child in dropZone.transform)
        {
            if(child.CompareTag("Highlight") == false)
            {
                child.SetParent(null);
            }
        }
    }
}
