using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WaterSimulationSpace : MonoBehaviour
{
    public ParticleSystem waterParticleView;
    public Vector2 gridCellSize = new Vector2( 10, 10 );
    public Rect gridArea = new Rect( 0, 0, 960, 640 );

    public Transform centerOfGravity;
    public Vector3 gravity = new Vector3( 0, -9.81f, 0 );
    public int maxParticlesPerCell = 10;

    private ParticleSystem.Particle[] _particleBuff;
    private Vector3[] _pressureGrid;
    private int[] _numParticlesInGrid;

    protected void Start()
    {
        int gridX = (int)( gridArea.width / gridCellSize.x );
        int gridY = (int)( gridArea.height / gridCellSize.y );

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
        int gridX = (int)( gridArea.width / gridCellSize.x );
        int gridY = (int)( gridArea.height / gridCellSize.y );

        x = (int)( ( ( pos.x - gridArea.x ) / gridArea.width ) * gridCellSize.x );
        y = (int)( ( ( pos.y - gridArea.y ) / gridArea.height ) * gridCellSize.y );

        int index = x + ( y * gridY );
        return index;
    }

    private int GetIndex( Vector3 pos )
    {
        int gridX = (int)( gridArea.width / gridCellSize.x );
        int gridY = (int)( gridArea.height / gridCellSize.y );

        int x = (int)( ( ( pos.x - gridArea.x ) / gridArea.width ) * gridCellSize.x );
        int y = (int)( ( ( pos.y - gridArea.y ) / gridArea.height ) * gridCellSize.y );

        int index = x + ( y * gridY );
        return index;
    }

    private int GetIndex( int x, int y )
    {
        int gridX = (int)( gridArea.width / gridCellSize.x );
        int gridY = (int)( gridArea.height / gridCellSize.y );

        int index = x + ( y * gridY );
        return index;
    }

    protected void Update()
    {
        if( centerOfGravity == null ) return;
        Vector3 cog = centerOfGravity.position;

        int gridX = (int)( gridArea.width / gridCellSize.x );
        int gridY = (int)( gridArea.height / gridCellSize.y );

        int numParticles = waterParticleView.GetParticles( _particleBuff );
        System.Array.Clear( _numParticlesInGrid, 0, gridX * gridY );

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

            if( _numParticlesInGrid[ index ] == maxParticlesPerCell )
            {
                //Vector3 pressureDir = _pressureGrid[ index ];
                //int pressue = _numParticlesInGrid[ index ];

                vel += _pressureGrid[ index ] * 5;
            }
            else
            {
                _numParticlesInGrid[ index ]++;
            }

            p.velocity = vel + gravity;

            _particleBuff[ i ] = p;
        }

        waterParticleView.SetParticles( _particleBuff, numParticles );
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube( gridArea.center, gridArea.size );

        Vector3 bottomRight = new Vector3( gridArea.xMin, gridArea.yMin, 0 );
        Vector3 bottomLeft = new Vector3( gridArea.xMax, gridArea.yMin, 0 );
        Vector3 topRight = new Vector3( gridArea.xMin, gridArea.yMax, 0 );
        Vector3 topLeft = new Vector3( gridArea.xMax, gridArea.yMax, 0 );

        int gridX = (int)( gridArea.width / gridCellSize.x );
        int gridY = (int)( gridArea.height / gridCellSize.y );

        for( int i = 0; i != gridX; ++i )
        {
            float x = ( i + 1 ) * gridCellSize.x;
            Gizmos.DrawLine( bottomRight + Vector3.right * x, topRight + Vector3.right * x );
        }

        for( int i = 0; i != gridY; ++i )
        {
            float y = ( i + 1 ) * gridCellSize.y;
            Gizmos.DrawLine( bottomRight + Vector3.up * y, bottomLeft + Vector3.up * y );
        }

        if( _pressureGrid != null && _pressureGrid.Length > 0 )
        {
            Gizmos.color = Color.red;
            for( int y = 0; y != gridY; ++y )
            {
                for( int x = 0; x != gridX; ++x )
                {
                    int index = GetIndex( x, y );

                    Vector3 centerPos = new Vector3( ( x * gridCellSize.x ) + gridArea.x, ( y * gridCellSize.y ) + gridArea.y, 0 );

                    Gizmos.DrawLine( centerPos, centerPos + _pressureGrid[ index ] * 5 );
                    Gizmos.DrawWireCube( centerPos, Vector3.one * _numParticlesInGrid[ index ] );
                }
            }
        }
    }
}
