using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

[System.Serializable]
public class FragmentoDialogo
{
    public string nombre;
    [TextArea(3, 5)]
    public string texto;
    public Sprite retrato;
}

public class DialogoTypingManager : MonoBehaviour
{
    [Header("UI Elementos")]
    [SerializeField] private TMP_Text nombrePersonajeTexto;
    [SerializeField] private TMP_Text dialogoTexto;
    [SerializeField] private Image retratoPersonaje;
    public GameObject panelJuego;

    [Header("Configuración de Diálogo")]
    [SerializeField] private FragmentoDialogo[] fragmentosDialogo;
    [SerializeField] private float velocidadEscritura = 0.05f;
    [SerializeField] private AudioSource sonidoEscritura;

    private int indiceActual = 0;
    private bool escribiendo = false;
    private string textoCompleto;

    private void Start()
    {
        MostrarDialogo();
    }

    private void Update()
    {
        // Detectar avance con espacio o clic izquierdo
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (escribiendo)
            {
                // Mostrar texto completo al instante
                StopAllCoroutines();
                dialogoTexto.text = textoCompleto;
                escribiendo = false;
            }
            else
            {
                SiguienteDialogo();   
            }
        }
    }

    private void MostrarDialogo()
    {
        if(indiceActual >= fragmentosDialogo.Length)
        {
            panelJuego.SetActive(true);
            gameObject.SetActive(false);
            return; // No hay más diálogos
        }

        var fragmento = fragmentosDialogo[indiceActual];

        if(string.IsNullOrEmpty(fragmento.nombre))
        {
            var personaje = GameManager.Instance.personajes[PlayerPrefs.GetInt("JugadorIndex", 0)];
            nombrePersonajeTexto.text = personaje.nombre;
            retratoPersonaje.sprite = personaje.personajeJugable;
            AjustarTamañoRetrato(personaje.personajeJugable);
        }
        else
        {
            nombrePersonajeTexto.text = fragmento.nombre;
            retratoPersonaje.sprite = fragmento.retrato;
            AjustarTamañoRetrato(fragmento.retrato);
        }

        StopAllCoroutines();
        StartCoroutine(EfectoEscritura(fragmento.texto));
    }

    private IEnumerator EfectoEscritura(string texto)
    {
        escribiendo = true;
        dialogoTexto.text = "";
        textoCompleto = texto;

        foreach (char letra in textoCompleto)
        {
            dialogoTexto.text += letra;
            if (sonidoEscritura != null) sonidoEscritura.Play();
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
    }

    private void SiguienteDialogo()
    {
        indiceActual++;
        MostrarDialogo();
    }

    private void AjustarTamañoRetrato(Sprite sprite)
    {
        if (sprite == null) return;
        retratoPersonaje.preserveAspect = true;
        retratoPersonaje.SetNativeSize();

        float escala = 0.5f;
        retratoPersonaje.rectTransform.sizeDelta *= escala;
    }
}
