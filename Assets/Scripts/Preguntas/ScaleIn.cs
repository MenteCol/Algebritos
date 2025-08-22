using UnityEngine;

public class ScaleIn : MonoBehaviour
{
    public float duration =3f;   // tiempo que tarda en crecer
    public Vector3 finalScale = Vector3.one; // tama�o final (1,1,1 por defecto)

    private void OnEnable()
    {
        transform.localScale = Vector3.zero; // empieza peque�ito
        StartCoroutine(ScaleUp());
    }

    private System.Collections.IEnumerator ScaleUp()
    {
        float time = 0f;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = finalScale; // asegura el tama�o final exacto
    }
}
