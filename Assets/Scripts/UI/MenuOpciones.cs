/* Antes de que la nacion de fuego llegara
 * 
 * 
using UnityEngine;
using UnityEngine.Audio;   

public class MenuOpciones : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }
    public void CambiarVolumen(float volumen)
    {
        audioMixer.SetFloat("Volumen", volumen);
    }
    public void CambiarCalidad(int calidadIndex)
    {
        QualitySettings.SetQualityLevel(calidadIndex);
    }
}


 */

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderVolumen;

    [Header("Opciones visuales")]
    [SerializeField] private Toggle toggleFullScreen;
    [SerializeField] private TMPro.TMP_Dropdown dropdownCalidad;

    private const string PREF_VOLUMEN = "VolumenGuardado";
    private const string PREF_FULLSCR = "FullScreenGuardado";
    private const string PREF_CALIDAD = "CalidadGuardada";

    void Start()
    {        
        if (sliderVolumen != null)
        {
            sliderVolumen.minValue = 0.0001f;
            sliderVolumen.maxValue = 1f;
        }
             
        if (PlayerPrefs.HasKey(PREF_VOLUMEN)) // Uso del pref guardado
        {
            float volumenLineal = PlayerPrefs.GetFloat(PREF_VOLUMEN);
            float volumenDB = Mathf.Log10(volumenLineal) * 20f;
            audioMixer.SetFloat("Volumen", volumenDB);

            if (sliderVolumen != null)
                sliderVolumen.value = volumenLineal;
        }
        else if (sliderVolumen != null)
        {
            float valorInicial = sliderVolumen.value;
            float volumenDB = Mathf.Log10(valorInicial) * 20f;
            audioMixer.SetFloat("Volumen", volumenDB);
        }
                
        if (toggleFullScreen != null)
        {
            if (PlayerPrefs.HasKey(PREF_FULLSCR))
            {
                bool fs = PlayerPrefs.GetInt(PREF_FULLSCR) == 1;
                Screen.fullScreen = fs;
                toggleFullScreen.isOn = fs;
            }
            else
            {
                toggleFullScreen.isOn = Screen.fullScreen;
            }
        }
                
        if (dropdownCalidad != null)
        {
            if (PlayerPrefs.HasKey(PREF_CALIDAD))
            {
                int q = PlayerPrefs.GetInt(PREF_CALIDAD);
                QualitySettings.SetQualityLevel(q);
                dropdownCalidad.value = q;
            }
            else
            {
                dropdownCalidad.value = QualitySettings.GetQualityLevel();
            }
        }
    }

    public void CambiarVolumen(float volumenLineal)
    {
        volumenLineal = Mathf.Clamp(volumenLineal, 0.0001f, 1f); // evitar log(0)
        float volumenDB = Mathf.Log10(volumenLineal) * 20f;

        audioMixer.SetFloat("Volumen", volumenDB);
        PlayerPrefs.SetFloat(PREF_VOLUMEN, volumenLineal);
        PlayerPrefs.Save();
    }

    public void PantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
        PlayerPrefs.SetInt(PREF_FULLSCR, pantallaCompleta ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void CambiarCalidad(int calidadIndex)
    {
        QualitySettings.SetQualityLevel(calidadIndex);
        PlayerPrefs.SetInt(PREF_CALIDAD, calidadIndex);
        PlayerPrefs.Save();
    }
}

