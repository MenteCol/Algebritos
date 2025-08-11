using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPersonaje", menuName = "Personaje")]

public class Personajes : ScriptableObject
{
    public Sprite personajeJugable;
    public Sprite imagen;
    public string nombre;
    public string descripcion;
}
