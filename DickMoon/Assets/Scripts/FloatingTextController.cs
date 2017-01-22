using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingTextController : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.up;
    public Text text;
    public float lifetime = 2;
    private float _timer = 0;

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        GetComponent< RectTransform >().Translate( moveDirection * Time.deltaTime );

        Color c = text.color;
        c.a = 1f - ( _timer / lifetime );
        text.color = c;

        if( _timer >= lifetime )
        {
            GameObject.Destroy( gameObject );
        }
    }
}
