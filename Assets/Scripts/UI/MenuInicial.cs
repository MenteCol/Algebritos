using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MenuInicial : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Salir()
    {
        Application.Quit();
        Debug.Log("Juego cerrado");
    }

    public void ResetAllData()
    {
        //Borrar progreso y configuración guardada
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        //Eliminar archivo de preguntas
        string filePath = Application.persistentDataPath + "/questions.json";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Archivo de preguntas eliminado.");
        }
        else
        {
            Debug.Log("No se encontró el archivo de preguntas para eliminar.");
        }

        Debug.Log("Todos los datos han sido reiniciados.");

        //Recargar la escena actual para reflejar los cambios
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
