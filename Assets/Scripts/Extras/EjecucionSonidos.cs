using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EjecucionSonidos : MonoBehaviour
{    
    public static EjecucionSonidos Instance { get; private set; }

    [System.Serializable]
    public class Pista
    {
        public string nombre;
        public AudioClip clip;

        [Range(0, 100)]
        public float volumen = 100f;
    }

    [Header("Lista de Pistas Disponibles")]
    public List<Pista> pistas = new List<Pista>();

    [Header("Mixer de Audio")]
    public AudioMixerGroup masterMixer;

    private Dictionary<string, Pista> diccionarioPistas;
    private Dictionary<string, AudioSource> instanciasActivas;

    private void Awake()
    {        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
             
        diccionarioPistas = new Dictionary<string, Pista>();
        instanciasActivas = new Dictionary<string, AudioSource>();

        foreach (var pista in pistas)
        {
            if (!diccionarioPistas.ContainsKey(pista.nombre))
            {
                diccionarioPistas.Add(pista.nombre, pista);
            }
        }
    }

    public void ReproducirAudio(string nombrePista, bool loop = false)
    {
        if (!diccionarioPistas.ContainsKey(nombrePista))
        {
            Debug.LogWarning($"La pista '{nombrePista}' no existe en EjecucionSonidos.");
            return;
        }
                
        if (instanciasActivas.ContainsKey(nombrePista))
        {
            AudioSource activo = instanciasActivas[nombrePista];
            if (activo != null && activo.isPlaying)
            {
                Debug.Log($"La pista '{nombrePista}' ya se está reproduciendo.");
                return;
            }
            else
            {               
                instanciasActivas.Remove(nombrePista);
            }
        }

        Pista pista = diccionarioPistas[nombrePista];

        GameObject go = new GameObject($"Audio_{nombrePista}");
        go.transform.SetParent(this.transform);

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = pista.clip;
        source.loop = loop;
        source.volume = pista.volumen / 100f;

        if (masterMixer != null)
        {
            source.outputAudioMixerGroup = masterMixer;
        }

        source.Play();
                
        instanciasActivas[nombrePista] = source;

        if (!loop)
        {        
            Destroy(go, pista.clip.length);
            StartCoroutine(LimpiarInstanciaDespues(pista.clip.length, nombrePista));
        }
    }

    private System.Collections.IEnumerator LimpiarInstanciaDespues(float tiempo, string nombrePista)
    {
        yield return new WaitForSeconds(tiempo);
        instanciasActivas.Remove(nombrePista);
    }

    public void DetenerAudio(string nombrePista)
    {
        if (instanciasActivas.ContainsKey(nombrePista))
        {
            AudioSource source = instanciasActivas[nombrePista];
            if (source != null)
            {
                Destroy(source.gameObject);
            }
            instanciasActivas.Remove(nombrePista);
        }
    }
}
