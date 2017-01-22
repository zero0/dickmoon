using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarPulse : MonoBehaviour
{
    public Image star;
    public float pulseSpeed;

    private float _timeOffset;

    protected void Start()
    {
        _timeOffset = Random.Range( -1f, 1f );
    }

    protected void Update()
    {
        _timeOffset += pulseSpeed * Time.deltaTime;
        float a = Mathf.Sin( _timeOffset );

        Color c = star.color;
        c.a = a;
        star.color = c;
    }
}
