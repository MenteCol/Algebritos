using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelProgressManager : MonoBehaviour
{
    public int levelNumber;
    public Button levelButton;
    public TMP_Text scoreText;
    public GameObject starIcon;
    public GameObject lockIcon;

    void Start()
    {
        //Nivel 1 siempre desbloqueado
        bool unlocked = levelNumber == 1 || PlayerPrefs.GetInt("Nivel " + levelNumber + " Desbloqueado", 0) == 1;
        levelButton.interactable = unlocked;

        //Mostrar/ocultar candado
        if(lockIcon != null)
        {
            lockIcon.SetActive(!unlocked);
        }

        //Mostrar puntuación si existe
        int score = PlayerPrefs.GetInt("Nivel " + levelNumber + "_Puntuación", -1);

        if(score >= 0 && unlocked)
        {
            scoreText.text = "Puntuación: " + score + "/5";
            starIcon.SetActive(score == 5);
        }
        else
        {
            scoreText.text = "--/5";
            starIcon.SetActive(false);
        }

        //Evento al presionar el botón del nivel
        levelButton.onClick.AddListener(() => LoadLevel());
    }

    void LoadLevel()
    {
        SceneManager.LoadScene("Dia " + levelNumber);
    }
}
