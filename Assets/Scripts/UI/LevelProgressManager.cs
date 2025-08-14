using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelProgressManager : MonoBehaviour
{
    public string sceneName;
    public int levelNumber;
    public TMP_Text scoreText;
    public GameObject starIcon;
    public GameObject lockIcon;

    [Header("Fade Escena")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration;

    [Header("Fade Candado")]
    public float lockFadeDuration;

    private bool isUnlocked;

    void Start()
    {
        //Nivel 1 siempre desbloqueado
        if (levelNumber == 1)
        {
            PlayerPrefs.SetInt("Nivel" + levelNumber + "_Unlocked", 1);
        }

        isUnlocked = PlayerPrefs.GetInt("Nivel" + levelNumber + "_Unlocked", 0) == 1;


        //Mostrar/ocultar candado
        lockIcon.gameObject.SetActive(!isUnlocked);

        //Mostrar puntuación si esta desbloqueado
        if (isUnlocked)
        {
            int score = PlayerPrefs.GetInt("Nivel" + levelNumber + "_Score", -1);

            if (score >= 0)
            {
                scoreText.text = score + "/5";
                starIcon.gameObject.SetActive(score == 5); //Estrella solo con 5/5
            }
            else
            {
                scoreText.text = "";
                starIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            scoreText.text = "";
            starIcon.SetActive(false);
        }
    }

    public void LoadLevel()
    {
        if(!isUnlocked)
        {
            Debug.Log("Nivel bloqueado. Desbloquea el nivel para jugar.");
            return;
        }

        if(fadeCanvasGroup != null)
        {
            StartCoroutine(FadeAndLoad());
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    IEnumerator FadeAndLoad()
    {
        fadeCanvasGroup.blocksRaycasts = true;
        float t = 0f;

        while(t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
