using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveController : MonoBehaviour
{
    public float lifetime = 5;
    public float speed = 1;
    public float waveHeight = 1;
	public float decayDelay = 0.3f;
    public float heightDecayOverTime = 0;
    public Transform center = null;
    public Image waveImage;
    public Sprite[] waveSprites;
    public float waveAnimFrameRate;

    private int _currentFrame;
    private float _frameTimer;
    private float _fullAnimTime;

    protected void Start()
    {
        _currentFrame = 0;
        _frameTimer = 0f;
        _fullAnimTime = waveAnimFrameRate / (float)waveSprites.Length;
    }

    protected void Update()
    {
        if( waveAnimFrameRate > 0 )
        {
            _frameTimer += Time.deltaTime;

            float framePercent = _frameTimer / _fullAnimTime;
            int frame = Mathf.Clamp( Mathf.FloorToInt( ( framePercent * (float)waveSprites.Length ) ), 0, waveSprites.Length - 1 );
            if( _currentFrame != frame )
            {
                _currentFrame = frame;
                waveImage.sprite = waveSprites[ _currentFrame ];
            }
        }

        if (center != null)
        {
            Vector3 dir = Vector3.Normalize(center.position - transform.position);
            Vector3 tangent = Vector3.Cross(dir, Vector3.forward * Mathf.Sign(speed));
            transform.right = tangent;
            transform.position += (tangent * Mathf.Abs(speed) * Time.deltaTime);
        }

		decayDelay -= Time.deltaTime;
		if (decayDelay < 0f)
		{
        	waveHeight -= heightDecayOverTime * Time.deltaTime;
		}

        transform.localScale = Vector3.one * Mathf.Clamp( waveHeight, 0, 2);

        if( waveHeight < 0 )
        {
            lifetime = 0;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            GameObject.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        coll.gameObject.SendMessage("HitByWave", waveHeight, SendMessageOptions.DontRequireReceiver);

        waveHeight -= 1;
    }
}