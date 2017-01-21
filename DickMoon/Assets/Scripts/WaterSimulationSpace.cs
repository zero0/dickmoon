using UnityEngine;
using System.Collections;

public class WaterSimulationSpace : MonoBehaviour
{
    public ParticleSystem waterParticleView;

    public Transform centerOfGravity;
    public Vector3 gravity = new Vector3( 0, -9.81f, 0 );
    public int maxParticlesPerCell = 10;
    public int numGridCellX = 10;
    public int numGridCellY = 10;
    public Rect gridArea = new Rect();

    private Vector2 _gridCellSize = new Vector2();
    private ParticleSystem.Particle[] _particleBuff;
    private Vector3[] _pressureGrid;
    private int[] _numParticlesInGrid;

    protected void Start()
    {
        _gridCellSize.x = gridArea.width / numGridCellX;
        _gridCellSize.y = gridArea.height / numGridCellY;

        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        _particleBuff = new ParticleSystem.Particle[ waterParticleView.maxParticles ];
        _pressureGrid = new Vector3[ gridX * gridY ];
        _numParticlesInGrid = new int[ gridX * gridY ];

        for( int i = 0, _pressureGridLength = _pressureGrid.Length; i < _pressureGridLength; i++ )
        {
            Vector3 pressure = _pressureGrid[ i ];
            pressure = Random.onUnitSphere;
            _pressureGrid[ i ] = pressure;
        }
    }

    private int GetIndex( Vector3 pos, out int x, out int y )
    {
        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        x = (int)( ( ( pos.x - gridArea.x ) / _gridCellSize.x ) ); /// _gridArea.width ) * _gridCellSize.x );
        y = (int)( ( ( pos.y - gridArea.y ) / _gridCellSize.y ) ); /// _gridArea.height ) * _gridCellSize.y );

        int index = x + ( y * gridX );
        return index;
    }

    private int GetIndex( Vector3 pos )
    {
        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        int x = (int)( ( ( pos.x - gridArea.x ) / gridArea.width ) * _gridCellSize.x );
        int y = (int)( ( ( pos.y - gridArea.y ) / gridArea.height ) * _gridCellSize.y );

        int index = x + ( y * gridY );
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
        int pad = 3;
        System.Array.Clear(_numParticlesInGrid, 0, gridX * gridY);
        for (int y = 0; y != gridY; ++y)
        {
            for (int x = 0; x != gridX; ++x)
            {
                int index = GetIndex(x, y);
                int value = -1;

                if (x < pad || x > gridX - pad - 1)
                {
                    value = 0;
                }
                if (y < pad || y > gridY - pad - 1)
                {
                    value = 0;
                }

                _numParticlesInGrid[index] = value;
            }
        }
    }

    protected void Update()
    {
        if( centerOfGravity == null ) return;
        Vector3 cog = centerOfGravity.position;

        int gridX = (int)( gridArea.width / _gridCellSize.x );
        int gridY = (int)( gridArea.height / _gridCellSize.y );

        ResetGrid();

        int numParticles = waterParticleView.GetParticles( _particleBuff );

        for( int i = 0; i != numParticles; i++ )
        {
            ParticleSystem.Particle p = _particleBuff[ i ];

            Vector3 pos = p.position;
            Vector3 dir = gravity;  //cog - pos;
            Vector3 vel = dir;//Vector3.Normalize( dir );

            int x, y;
            int index = GetIndex( pos, out x, out y );
            if( index < 0 )
            {
                index = 0;
            }
            else if( index >= _numParticlesInGrid.Length )
            {
                index = _numParticlesInGrid.Length - 1;
            }

            if(_numParticlesInGrid[ index ] < 0 )
            {

            }
            else if( _numParticlesInGrid[ index ] == maxParticlesPerCell )
            {
                //Vector3 pressureDir = _pressureGrid[ index ];
                //int pressue = _numParticlesInGrid[ index ];

                vel += _pressureGrid[ index ] * 5;
            }
            else
            {
                _numParticlesInGrid[ index ]++;
            }

            p.velocity = vel;// + gravity;

            _particleBuff[ i ] = p;
        }

        waterParticleView.SetParticles( _particleBuff, numParticles );
    }

    void OnDrawGizmos()
    {
        OnDrawDebug();
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
                    Gizmos.DrawLine( centerPos, centerPos + _pressureGrid[ index ] * 5 );
                    //imm.AddLine(centerPos, centerPos + _pressureGrid[index] * 5, Color.red);

                    int value = _numParticlesInGrid[index];
                    if( value < 0 )
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawWireCube(centerPos, Vector3.one * 2);
                    }
                    else
                    {
                        Gizmos.DrawWireCube( centerPos, Vector3.one * value );
                    }
                }
            }
        }

        //imm.EndImmediate();
    }
}
