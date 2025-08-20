using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelUI
    {
        public Button levelButton;         // Botón del minijuego
        public TMP_Text descriptionText;   // Texto debajo del botón (ej: "Repaso de álgebra")
        public string sceneName;           // Nombre de la escena que se abrirá
    }

    public LevelUI[] levels;

    private void Start()
    {
        SetupMenu();
    }

    void SetupMenu()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            int index = i;
            levels[i].levelButton.onClick.RemoveAllListeners();
            levels[i].levelButton.onClick.AddListener(() => LoadLevel(levels[index].sceneName));

            // Si quieres mostrar una descripción genérica
            if (levels[i].descriptionText != null && string.IsNullOrEmpty(levels[i].descriptionText.text))
            {
                levels[i].descriptionText.text = "Minijuego de repaso";
            }
        }
    }

    void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
