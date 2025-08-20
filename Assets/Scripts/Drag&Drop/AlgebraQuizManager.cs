using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class AlgebraQuizManager : MonoBehaviour
{
    [Header("UI Referencias")]
    public Transform expressionContainer; //Panel con HorizontalLayoutGroup
    public GameObject textPrefab; //Prefab para mostrar partes de la expresion
    public GameObject dropZonePrefab; //Prefab con un TMP_Text vacio donde va la incognita
    public Button[] optionButtons; //3 Botones de opciones
    public TMP_Text feedbackText; //Texto para feedback

    [Header("Configuración")]
    public List<AlgebraQuestions> allQuestions; //Lista total de preguntas en el inspector
    public List<AlgebraQuestions> quizQuestions; //Preguntas seleccionadas para el quiz
    private int currentIndex = 0;
    private int score = 0;

    private AlgebraQuestions currentQuestion;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    void Start()
    {
        SelectRandomQuestion();
        ShowQuestion();
    }

    void SelectRandomQuestion()
    {
        quizQuestions = allQuestions.OrderBy(q => Random.value).Take(5).ToList();
    }

    void ShowQuestion()
    {
        //Limpiar feedback
        feedbackText.text = "";

        //Limpiar expresion previa
        foreach (Transform child in expressionContainer)
        {
            Destroy(child.gameObject);
        }

        //Validar índice
        if (currentIndex >= quizQuestions.Count)
        {
            EndQuiz();
            return;
        }

        currentQuestion = quizQuestions[currentIndex];

        //Construir expresión en pantalla 
        for(int i = 0; i < currentQuestion.expressionParts.Length; i++)
        {
            if(i == currentQuestion.missingIndex)
            {
                Instantiate(dropZonePrefab, expressionContainer); //Lugar vacio
            }
            else
            {
                GameObject part = Instantiate(textPrefab, expressionContainer);
                part.GetComponent<TMP_Text>().text = currentQuestion.expressionParts[i];
            }
        }

        //Configurar botones de opciones aleatoriamente
        string[] shuffledOptions = currentQuestion.options.OrderBy(x => Random.value).ToArray();

        for(int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].GetComponentInChildren<TMP_Text>().text = shuffledOptions[i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnAnswer(shuffledOptions[index]));
        }
    }

    void OnAnswer(string selectedOption)
    {
        string correctAnswer = currentQuestion.expressionParts[currentQuestion.missingIndex];

        if(selectedOption == correctAnswer)
        {
            feedbackText.text = "¡Correcto!";
            score++;

            if (audioSource != null && correctSound != null)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }
        else
        {
            feedbackText.text = $"Incorrecto. La respuesta correcta es {correctAnswer}.";

            if (audioSource != null && incorrectSound != null)
            {
                audioSource.PlayOneShot(incorrectSound);
            }
        }

        Invoke(nameof(NextQuestion), 5f);
    }

    void NextQuestion()
    {
        currentIndex++;
        ShowQuestion();
    }

    void EndQuiz()
    {
        //Guardar progreso
        PlayerPrefs.SetInt("Repaso1_Score", score);
        PlayerPrefs.Save();

        //Feedback final
        feedbackText.text = $"Quiz terminado. Tu puntuación es {score} de {quizQuestions.Count}.";

        //Volver al menú principal después de 5 segundos
        Invoke(nameof(ReturnToMenu), 5f);
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("04. Niveles");
    }
}
