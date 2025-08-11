using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DialogoTypingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text nombrePersonajeTexto;
    [SerializeField] private TMP_Text dialogoTexto;
    [SerializeField] private Image retratoPersonaje;
    [SerializeField] private GameObject panelJuego;

    [Header("Configuración")]
    [SerializeField] private Vector2 maxRetratoSize = new Vector2(200, 200);
    [SerializeField] private float velocidadEscritura = 0.05f;
    [SerializeField] private AudioSource sonidoEscritura;

    [Header("Fragmentos de diálogo")]
    [TextArea(3, 5)]
    public string[] fragmentosDialogo;

    private int indiceActual = 0;
    private bool escribiendo = false;

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
                dialogoTexto.text = fragmentosDialogo[indiceActual];
                escribiendo = false;
            }
            else
            {
                // Pasar al siguiente fragmento
                indiceActual++;
                if (indiceActual < fragmentosDialogo.Length)
                {
                    MostrarDialogo();
                }
                else
                {
                    // Fin del diálogo
                    Debug.Log("Diálogo terminado");
                    panelJuego.SetActive(true);
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    private void MostrarDialogo()
    {
        var personaje = GameManager.Instance.personajes[PlayerPrefs.GetInt("JugadorIndex", 0)];

        nombrePersonajeTexto.text = personaje.nombre;
        retratoPersonaje.sprite = personaje.personajeJugable;

        AjustarTamañoRetrato(personaje.personajeJugable);

        StopAllCoroutines();
        StartCoroutine(EfectoEscritura(fragmentosDialogo[indiceActual]));
    }

    private void AjustarTamañoRetrato(Sprite sprite)
    {
        if (sprite == null) return;

        retratoPersonaje.SetNativeSize();
        RectTransform rt = retratoPersonaje.GetComponent<RectTransform>();

        float ancho = rt.sizeDelta.x;
        float alto = rt.sizeDelta.y;

        float escala = Mathf.Min(maxRetratoSize.x / ancho, maxRetratoSize.y / alto, 1f);
        rt.sizeDelta = new Vector2(ancho * escala, alto * escala);
    }

    private IEnumerator EfectoEscritura(string texto)
    {
        escribiendo = true;
        dialogoTexto.text = "";

        foreach (char letra in texto.ToCharArray())
        {
            dialogoTexto.text += letra;
            if (sonidoEscritura != null) sonidoEscritura.Play();
            yield return new WaitForSeconds(velocidadEscritura);
        }

        escribiendo = false;
    }
}
