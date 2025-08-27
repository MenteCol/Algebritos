using UnityEngine;
using UnityEngine.InputSystem;

public class GestionSonidos : MonoBehaviour
{
    public void Start()
    {
        EjecucionSonidos.Instance.ReproducirAudio("Musica_Cancion1", true);
    }

    //private void Update()
    //{
    //    if (Keyboard.current.spaceKey.isPressed)
    //    {
    //        ReproducirOneShot("SFX_Win");
    //    }

    //    if (Keyboard.current.kKey.wasPressedThisFrame)            
    //    {
    //        ReproducirOneShot("SFX_Fail");
    //    }
    //}

    public void SonidoTecla()
    { 
        ReproducirOneShot("SFX_Tecla");
    }

    public void ReproducirOneShot(string nombre)
    { 
        EjecucionSonidos.Instance.ReproducirAudio(nombre);
    }
}
