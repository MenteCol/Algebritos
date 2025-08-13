using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelProgressManager : MonoBehaviour
{
    public Button[] levelButtons;

    private void Start()
    {
        //Cargar progreso del jugador
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        //Bloquear/desbloquear botones
        for(int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = (i + i) <= unlockedLevel;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelIndex); // Guardar el nivel seleccionado
        SceneManager.LoadScene("Dia " + levelIndex); // Cargar la escena del nivel seleccionado
    }
}
