using UnityEngine;
using System.Collections;

public class WaterSimulationSpace : MonoBehaviour
{
#if false
    public ParticleSystem waterParticleView;

    public Transform centerOfGravity;
    public Vector3 gravity = new Vector3( 0, -9.81f, 0 );
    public int maxParticlesPerCell = 10;
    public int numGridCellX = 10;
    public int numGridCellY = 10;
    public Rect gridArea = new Rect();
    public Quaternion pullForceDirection = Quaternion.identity;
    public float pullForce = 0;
    public bool debugDraw = false;

    private Vector2 _gridCellSize = new Vector2();
    private ParticleSystem.Particle[] _particleBuff;
    private Vector3[,] _pressureGrid;
    private float[,] _densityGrid;

    protected void Start()
    {
        _gridCellSize.x = gridArea.width / numGridCellX;
        _gridCellSize.y = gridArea.height / numGridCellY;

        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        _particleBuff = new ParticleSystem.Particle[ waterParticleView.maxParticles ];
        _pressureGrid = new Vector3[ gridX, gridY ];
        _densityGrid = new float[ gridX, gridY ];

        Vector3 cog = centerOfGravity.position;

        for (int y = 0; y != gridY; ++y)
        {
            for (int x = 0; x != gridX; ++x)
            {
                Vector3 pressure = Vector3.zero;
                _pressureGrid[x,y] = pressure.normalized;
            }
        }
    }

    private int GetIndex( Vector3 pos, out int x, out int y )
    {
        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        x = Mathf.Clamp( (int)( ( pos.x - gridArea.x ) / _gridCellSize.x ), 0, gridX - 1 ); /// _gridArea.width ) * _gridCellSize.x );
        y = Mathf.Clamp( (int)( ( pos.y - gridArea.y ) / _gridCellSize.y ), 0, gridY - 1 ); /// _gridArea.height ) * _gridCellSize.y );

        int index = x + ( y * gridX );
        return index;
    }
    
    private int GetIndex( int x, int y )
    {
        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        int index = x + ( y * gridX );
        return index;
    }

    private void ResetGrid()
    {
        int gridX = (int)(gridArea.width / _gridCellSize.x);
        int gridY = (int)(gridArea.height / _gridCellSize.y);
        int pad = 6;

        System.Array.Clear(_densityGrid, 0, gridX * gridY);

        for (int y = 0; y != gridY; ++y)
        {
            for (int x = 0; x != gridX; ++x)
            {
                int index = GetIndex(x, y);
                float value = -1;

                if (x < pad || x > gridX - pad - 1)
                {
                    value = 0;
                }
                if (y < pad || y > gridY - pad - 1)
                {
                    value = 0;
                }

                _densityGrid[x,y] = value;
                _pressureGrid[x, y] = (pullForceDirection * Vector3.up) * pullForce;
            }
        }
    }

    protected void LateUpdate()
    {
        if( centerOfGravity == null ) return;
        Vector3 cog = centerOfGravity.localPosition;

        if( Input.GetKey( KeyCode.Space ) )
        {
            pullForce += 10 * Time.deltaTime;
        }
        else
        {
            pullForce -= 20 * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pullForceDirection *= Quaternion.Euler( 0, 0, 15 * Time.deltaTime );
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            pullForceDirection *= Quaternion.Euler( 0, 0, 15 * -Time.deltaTime );
        }

        pullForce = Mathf.Clamp(pullForce, 0, 10);

        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        ResetGrid();

        Transform t = waterParticleView.transform;

        int numParticles = waterParticleView.GetParticles( _particleBuff );

        for (int i = 0; i != numParticles; i++)
        {
            ParticleSystem.Particle p = _particleBuff[i];

            Vector3 pos = (p.position);
            Vector3 dir = Vector3.Normalize(cog - pos);
            Vector3 vel = Vector3.zero;

            int x, y;
            int index = GetIndex(pos, out x, out y);
            if (index < 0)
            {
                continue;
            }
            else if (index >= _densityGrid.Length)
            {
                continue;
            }

            try
            {
                if (_densityGrid[x, y] < 0)
                {
                    continue;
                }
                else
                {
                    _densityGrid[x, y] += 1;
                }
            }
            catch( System.Exception e )
            {
                x = x;
            }
        }

        for ( int i = 0; i != numParticles; i++)
        {
            ParticleSystem.Particle p = _particleBuff[ i ];

            Vector3 pos = (p.position);
            Vector3 cogDir = (cog - pos).normalized;
            Vector3 vel = Vector3.zero;

            int x, y;
            int index = GetIndex( pos, out x, out y );
            if( index < 0 )
            {
                index = 0;
                p.lifetime = 0;
                _particleBuff[i] = p;

                continue;
            }
            else if( index >= _densityGrid.Length )
            {
                index = _densityGrid.Length - 1;
                p.lifetime = 0;
                _particleBuff[i] = p;

                continue;
            }

            if (_densityGrid[ x,y ] < 0 )
            {
                vel = Vector3.zero;
                p.velocity = vel;
                p.lifetime = 0;
                _particleBuff[i] = p;

                continue;
            }

            //vel += nepos.x * _densityGrid[x, y];

            p.velocity = vel + _pressureGrid[x, y] + cogDir * gravity.magnitude * Time.deltaTime;

            pos.z = 0;
            p.position = pos;
            p.lifetime = 1;

            _particleBuff[ i ] = p;
        }

        waterParticleView.SetParticles( _particleBuff, numParticles );
    }

    void OnDrawGizmos()
    {
        if( debugDraw )
        {
            OnDrawDebug();
        }
    }

    private void OnDrawDebug()
    {
      
        if( gridArea.width == 0 || gridArea.height == 0 )
        {
            return;
        }
        if(_gridCellSize.x == 0 || _gridCellSize.y == 0 )
        {
            _gridCellSize.x = gridArea.width / numGridCellX;
            _gridCellSize.y = gridArea.height / numGridCellY;
        }
        //ImmediateModeRendering imm = ImmediateModeRendering.GetImmediateRenderingForLayer(0);
        //imm.BeginImmediateOrthoDraw(ImmediateModeRenderingTopology.Line, ImmediateModeRenderingVertexFormat.VertexColor);

        //Gizmos.DrawWireCube( _gridArea.center, _gridArea.size );

        Vector3 pos = transform.position;
        Vector3 bottomRight = new Vector3( gridArea.xMin, gridArea.yMin, 0 ) + pos;
        Vector3 bottomLeft = new Vector3( gridArea.xMax, gridArea.yMin, 0) + pos;
        Vector3 topRight = new Vector3( gridArea.xMin, gridArea.yMax, 0) + pos;
        Vector3 topLeft = new Vector3( gridArea.xMax, gridArea.yMax, 0) + pos;

        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        for( int i = 0; i != gridX; ++i )
        {
            float x = ( i + 1 ) * _gridCellSize.x;
            Gizmos.DrawLine( bottomRight + Vector3.right * x, topRight + Vector3.right * x );
            //imm.AddLine(bottomRight + Vector3.right * x, topRight + Vector3.right * x, Color.white);
        }

        for( int i = 0; i != gridY; ++i )
        {
            float y = ( i + 1 ) * _gridCellSize.y;
            Gizmos.DrawLine( bottomRight + Vector3.up * y, bottomLeft + Vector3.up * y );
            //imm.AddLine(bottomRight + Vector3.up * y, bottomLeft + Vector3.up * y, Color.white);
        }

        if ( _pressureGrid != null && _pressureGrid.Length > 0 )
        {
            for( int y = 0; y != gridY; ++y )
            {
                for( int x = 0; x != gridX; ++x )
                {
                    int index = GetIndex( x, y );

                    Vector3 centerPos = new Vector3( ( x * _gridCellSize.x ) + gridArea.x + ( 0.5f * _gridCellSize.x ), ( y * _gridCellSize.y ) + gridArea.y + ( 0.5f * _gridCellSize.y ), 0 ) + pos;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine( centerPos, centerPos + _pressureGrid[x, y] * 5 );
                    //imm.AddLine(centerPos, centerPos + _pressureGrid[index] * 5, Color.red);

                    float value = _densityGrid[x, y];
                    if( value < 0 )
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(centerPos, Vector3.one * 2);
                    }
                    else
                    {
                        Gizmos.DrawWireCube( centerPos, Vector3.one * value );
                    }
                }
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + (pullForceDirection * Vector3.up) * pullForce);

        //imm.EndImmediate();
    }
#endif
}
