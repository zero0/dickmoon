using UnityEngine;
using System.Collections.Generic;

public enum ImmediateModeRenderingTopology
{
    None,

    Line,
    Triangle,
};

[System.Flags]
public enum ImmediateModeRenderingVertexFormat
{
    None =                  0x00,

    Vertex =                0x01,
    Color =                 0x02,
    UV =                    0x04,

    VertexColor =           Vertex | Color,
    VertexColorUV =         Vertex | Color | UV
};

public class ImmediateModeRendering : MonoBehaviour
{
    private static Material DefaultBlitMaterial = null;

    private class ImmediateModeRenderingCommand
    {
        public bool isOrtho;
        public int layer;
        public ImmediateModeRenderingVertexFormat vertexFormat;
        public ImmediateModeRenderingTopology topology;
        public Matrix4x4 matrix;
        public int vertexOffset;
        public int vertexCount;
        public int indexOffset;
        public int indexCount;
        public Material material;
        public Font font;
    };

    private Dictionary< string, Font > _fontCache;

    private List< float > _vertexBuffer;
    private List< int > _indexBuffer;

    private List< ImmediateModeRenderingCommand > _commands;
    private int _currentCommandIndex;

    private int _vertexSize;
    private int _indexSize;

    private int _layer;

    public static ImmediateModeRendering GetImmediateRenderingForLayer( int layer )
    {
        int layerMask = 1 << layer;

        ImmediateModeRendering imm = null;

        Camera[] allCams = new Camera[ Camera.allCamerasCount ];
        Camera.GetAllCameras( allCams );

        for( int i = 0, imax = allCams.Length; i < imax; i++ )
        {
            Camera c = allCams[ i ];
            if( c != null && ( c.cullingMask & layerMask ) != 0 && c.enabled )
            {
                imm = c.GetComponent< ImmediateModeRendering >();
                if( imm == null )
                {
                    imm = c.gameObject.AddComponent< ImmediateModeRendering >();
                    imm._layer = layer;
                }

                break;
            }
        }

        return imm;
    }

    protected void Awake()
    {
        _fontCache = new Dictionary<string, Font>();

        _vertexBuffer = new List<float>();
        _indexBuffer = new List<int>();

        _commands = new List<ImmediateModeRenderingCommand>();
        _currentCommandIndex = -1;

        _vertexSize = 0;
        _indexSize = 0;

        _layer = 0;
    }

    protected void Start()
    {
    }

    protected void Update()
    {
    }

    protected void LateUpdate()
    {
    }

    protected void OnPostRender()
    {
        if( _commands.Count == 0 ) return;

        GL.PushMatrix();

        for( int c = 0, cmax = _commands.Count; c < cmax; c++ )
        {
            ImmediateModeRenderingCommand cmd = _commands[ c ];

            GL.PushMatrix();

            if( cmd.isOrtho )
            {
                GL.LoadOrtho();
            }

            GL.MultMatrix( cmd.matrix );

            cmd.material.SetPass( 0 );

            switch( cmd.topology )
            {
                case ImmediateModeRenderingTopology.Line:
                    GL.Begin( GL.LINES );
                    break;

                case ImmediateModeRenderingTopology.Triangle:
                    GL.Begin( GL.TRIANGLES );
                    break;
            }

            int stride = 0;
            if( ( cmd.vertexFormat & ImmediateModeRenderingVertexFormat.Vertex ) != 0 )
            {
                stride += 3;
            }
            if( ( cmd.vertexFormat & ImmediateModeRenderingVertexFormat.Color ) != 0 )
            {
                stride += 4;
            }
            if( ( cmd.vertexFormat & ImmediateModeRenderingVertexFormat.UV ) != 0 )
            {
                stride += 2;
            }

            for( int i = cmd.indexOffset, imax = cmd.indexOffset + cmd.indexCount; i < imax; ++i )
            {
                int index = _indexBuffer[ i ];
                int vertIndex = cmd.vertexOffset + ( index * stride );

                if( ( cmd.vertexFormat & ImmediateModeRenderingVertexFormat.UV ) != 0 )
                {
                    float u = _vertexBuffer[ vertIndex++ ];
                    float v = _vertexBuffer[ vertIndex++ ];
                    GL.TexCoord2( u, v );
                }

                if( ( cmd.vertexFormat & ImmediateModeRenderingVertexFormat.Color ) != 0 )
                {
                    float r = _vertexBuffer[ vertIndex++ ];
                    float g = _vertexBuffer[ vertIndex++ ];
                    float b = _vertexBuffer[ vertIndex++ ];
                    float a = _vertexBuffer[ vertIndex++ ];
                    GL.Color( new Color( r, g, b, a ) );
                }

                if( ( cmd.vertexFormat & ImmediateModeRenderingVertexFormat.Vertex ) != 0 )
                {
                    float x = _vertexBuffer[ vertIndex++ ];
                    float y = _vertexBuffer[ vertIndex++ ];
                    float z = _vertexBuffer[ vertIndex++ ];
                    GL.Vertex3( x, y, z );
                }
            }

            GL.End();

            GL.PopMatrix();
        }

        GL.PopMatrix();

        _vertexSize = 0;
        _indexSize = 0;

        _vertexBuffer.Clear();
        _indexBuffer.Clear();
        _commands.Clear();
    }

    public void BeginImmediateDraw( ImmediateModeRenderingTopology topology, ImmediateModeRenderingVertexFormat vertexFormat, Material material = null )
    {
        Assert( _currentCommandIndex == -1, "Rendering Command already in progress" );

        _currentCommandIndex = _commands.Count;

        if( material == null )
        {
            material = GetDefaultBlitMaterial();
        }

        ImmediateModeRenderingCommand cmd = new ImmediateModeRenderingCommand();
        cmd.isOrtho = false;
        cmd.layer = _layer;
        cmd.vertexFormat = vertexFormat;
        cmd.topology = topology;
        cmd.matrix = Matrix4x4.identity;
        cmd.vertexOffset = 0;
        cmd.vertexCount = 0;
        cmd.indexOffset = 0;
        cmd.indexCount = 0;
        cmd.material = material;
        cmd.font = null;

        _commands.Add( cmd );
    }

    public void BeginImmediateOrthoDraw( ImmediateModeRenderingTopology topology, ImmediateModeRenderingVertexFormat vertexFormat, Material material = null )
    {
        Assert( _currentCommandIndex == -1, "Rendering Command already in progress" );

        _currentCommandIndex = _commands.Count;

        if( material == null )
        {
            material = GetDefaultBlitMaterial();
        }

        ImmediateModeRenderingCommand cmd = new ImmediateModeRenderingCommand();
        cmd.isOrtho = true;
        cmd.layer = _layer;
        cmd.vertexFormat = vertexFormat;
        cmd.topology = topology;
        cmd.matrix = Matrix4x4.identity;
        cmd.vertexOffset = 0;
        cmd.vertexCount = 0;
        cmd.indexOffset = 0;
        cmd.indexCount = 0;
        cmd.material = material;
        cmd.font = null;

        _commands.Add( cmd );
    }

    public void BeginImmediateText( string fontName )
    {
        Assert( _currentCommandIndex == -1, "Rendering Command already in progress" );

        _currentCommandIndex = _commands.Count;

        Font font;
        if( !_fontCache.TryGetValue( fontName, out font ) )
        {
            font = Font.CreateDynamicFontFromOSFont( fontName, 32 );
            _fontCache.Add( fontName, font );
        }

        ImmediateModeRenderingCommand cmd = new ImmediateModeRenderingCommand();
        cmd.isOrtho = false;
        cmd.layer = _layer;
        cmd.vertexFormat = ImmediateModeRenderingVertexFormat.VertexColorUV;
        cmd.topology = ImmediateModeRenderingTopology.Triangle;
        cmd.matrix = Matrix4x4.identity;
        cmd.vertexOffset = 0;
        cmd.vertexCount = 0;
        cmd.indexOffset = 0;
        cmd.indexCount = 0;
        cmd.material = font.material;
        cmd.font = font;

        _commands.Add( cmd );
    }

    public void SetMatrix( Matrix4x4 matrix )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started to set matrix" );
        _commands[ _currentCommandIndex ].matrix = matrix;
    }

    public void SetTRS( Vector3 pos, Quaternion rot, Vector3 scale )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started to set matrix" );
        _commands[ _currentCommandIndex ].matrix = Matrix4x4.TRS( pos, rot, scale );
    }

    public void GetTextSize( string text, int fontSize, out Vector2 size )
    {
        size.x = 0f;
        size.y = 0f;

        if( string.IsNullOrEmpty( text ) ) return;

        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];

        Font font = cmd.font;
        Assert( font != null, "Missing Font" );

        font.RequestCharactersInTexture( text );

        Vector3 cursor = Vector3.zero;
        float fontScale = ( (float)fontSize / 2048f );

        for( int i = 0, imax = text.Length; i < imax; i++ )
        {
            char c = text[ i ];

            switch( c )
            {
                case '\n':
                case '\r':
                    cursor.x = 0;
                    cursor.y += (float)font.lineHeight * fontScale;
                    continue;
            }

            CharacterInfo info;
            if( font.GetCharacterInfo( c, out info ) )
            {
                cursor.x += (float)info.advance * fontScale;
            }
        }

        size.x = cursor.x;
        size.y = cursor.y + (float)font.lineHeight * fontScale;
    }

    public void AddText( Vector3 pos, string text, int fontSize, Color color )
    {
        AddText( pos, text, fontSize, color, color );
    }

    public void AddText( Vector3 pos, string text, int fontSize, Color colorTop, Color colorBottom )
    {
        if( string.IsNullOrEmpty( text ) ) return;

        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColorUV, "Incorrect vertex format" );
        Assert( cmd.topology == ImmediateModeRenderingTopology.Triangle, "Incorrect Topology" );

        Font font = cmd.font;
        Assert( font != null, "Missing Font" );

        font.RequestCharactersInTexture( text );

        Vector3 cursor = pos;
        float baseline = 0;
        float fontScale = ( (float)fontSize / 2048f );

        Color c0 = colorBottom;
        Color c1 = colorTop;
        Color c2 = colorTop;
        Color c3 = colorBottom;

        for( int i = 0, imax = text.Length; i < imax; i++ )
        {
            char c = text[ i ];

            switch( c )
            {
                case '\n':
                case '\r':
                    cursor.x = 0;
                    cursor.y += (float)font.lineHeight * fontScale;
                    continue;
            }

            CharacterInfo info;
            if( font.GetCharacterInfo( c, out info ) )
            {
                float xMin = cursor.x + ( fontScale * info.minX );
                float xMax = cursor.x + ( fontScale * info.maxX );
                float yMin = cursor.y + ( fontScale * info.minY ) - baseline;
                float yMax = cursor.y + ( fontScale * info.maxY ) - baseline;

                Vector3 v0 = new Vector3( xMin, yMin, cursor.z );
                Vector3 v1 = new Vector3( xMin, yMax, cursor.z );
                Vector3 v2 = new Vector3( xMax, yMax, cursor.z );
                Vector3 v3 = new Vector3( xMax, yMin, cursor.z );

                Vector2 uv0 = info.uvBottomLeft;
                Vector2 uv1 = info.uvTopLeft;
                Vector2 uv2 = info.uvTopRight;
                Vector2 uv3 = info.uvBottomRight;

                AddQuad(
                    v0, c0, uv0,
                    v1, c1, uv1,
                    v2, c2, uv2,
                    v3, c3, uv3
                );

                cursor.x += (float)info.advance * fontScale;
            }
        }
    }

    public void AddVertex( Vector3 vert, Color color )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColor, "Incorrect vertex format" );

        AddVertexData( color );
        AddVertexData( vert );

        cmd.vertexCount += 1;
    }

    public void AddVertex( Vector3 vert, Color color, Vector2 uv )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColorUV, "Incorrect vertex format" );

        AddVertexData( uv );
        AddVertexData( color );
        AddVertexData( vert );

        cmd.vertexCount += 1;
    }

    public void AddLine( Vector3 vert0, Vector3 vert1, Color color )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColor, "Incorrect vertex format" );
        Assert( cmd.topology == ImmediateModeRenderingTopology.Line, "Incorrect Topology" );

        AddVertexData( color );
        AddVertexData( vert0 );

        AddVertexData( color );
        AddVertexData( vert1 );

        _indexBuffer.Add( cmd.vertexCount + 0 );
        _indexBuffer.Add( cmd.vertexCount + 1 );

        cmd.vertexCount += 2;
        cmd.indexCount += 2;
    }

    public void AddLine( Vector3 vert0, Color color0, Vector3 vert1, Color color1 )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColor, "Incorrect vertex format" );
        Assert( cmd.topology == ImmediateModeRenderingTopology.Line, "Incorrect Topology" );

        AddVertexData( color0 );
        AddVertexData( vert0 );

        AddVertexData( color1 );
        AddVertexData( vert1 );

        _indexBuffer.Add( cmd.vertexCount + 0 );
        _indexBuffer.Add( cmd.vertexCount + 1 );

        cmd.vertexCount += 2;
        cmd.indexCount += 2;
    }

    public void AddLineIndex( int index0, int index1 )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.topology == ImmediateModeRenderingTopology.Line, "Incorrect Topology" );

        _indexBuffer.Add( index0 );
        _indexBuffer.Add( index1 );

        cmd.indexCount += 2;
    }

    public void AddTriangleIndex( int index0, int index1, int index2 )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.topology == ImmediateModeRenderingTopology.Triangle, "Incorrect Topology" );

        _indexBuffer.Add( index0 );
        _indexBuffer.Add( index1 );
        _indexBuffer.Add( index2 );

        cmd.indexCount += 3;
    }

    public void AddQuadIndex( int index0, int index1, int index2, int index3 )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.topology == ImmediateModeRenderingTopology.Triangle, "Incorrect Topology" );

        _indexBuffer.Add( index0 );
        _indexBuffer.Add( index1 );
        _indexBuffer.Add( index2 );

        _indexBuffer.Add( index2 );
        _indexBuffer.Add( index3 );
        _indexBuffer.Add( index0 );

        cmd.indexCount += 6;
    }

    public void AddQuad( Vector3 vert0, Vector3 vert1, Vector3 vert2, Vector3 vert3, Color color )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColor, "Incorrect vertex format" );
        Assert( cmd.topology == ImmediateModeRenderingTopology.Triangle, "Incorrect Topology" );

        AddVertexData( color );
        AddVertexData( vert0 );

        AddVertexData( color );
        AddVertexData( vert1 );

        AddVertexData( color );
        AddVertexData( vert2 );

        AddVertexData( color );
        AddVertexData( vert3 );

        _indexBuffer.Add( cmd.vertexCount + 0 );
        _indexBuffer.Add( cmd.vertexCount + 1 );
        _indexBuffer.Add( cmd.vertexCount + 2 );

        _indexBuffer.Add( cmd.vertexCount + 2 );
        _indexBuffer.Add( cmd.vertexCount + 3 );
        _indexBuffer.Add( cmd.vertexCount + 0 );

        cmd.vertexCount += 4;
        cmd.indexCount += 6;
    }

    public void AddQuad( Vector3 vert0, Color color0, Vector3 vert1, Color color1, Vector3 vert2, Color color2, Vector3 vert3, Color color3 )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColor, "Incorrect vertex format" );
        Assert( cmd.topology == ImmediateModeRenderingTopology.Triangle, "Incorrect Topology" );

        AddVertexData( color0 );
        AddVertexData( vert0 );

        AddVertexData( color1 );
        AddVertexData( vert1 );

        AddVertexData( color2 );
        AddVertexData( vert2 );

        AddVertexData( color3 );
        AddVertexData( vert3 );

        _indexBuffer.Add( cmd.vertexCount + 0 );
        _indexBuffer.Add( cmd.vertexCount + 1 );
        _indexBuffer.Add( cmd.vertexCount + 2 );

        _indexBuffer.Add( cmd.vertexCount + 2 );
        _indexBuffer.Add( cmd.vertexCount + 3 );
        _indexBuffer.Add( cmd.vertexCount + 0 );

        cmd.vertexCount += 4;
        cmd.indexCount += 6;
    }

    public void AddQuad( Vector3 vert0, Color color0, Vector2 uv0, Vector3 vert1, Color color1, Vector2 uv1, Vector3 vert2, Color color2, Vector2 uv2, Vector3 vert3, Color color3, Vector2 uv3 )
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        Assert( cmd.vertexFormat == ImmediateModeRenderingVertexFormat.VertexColorUV, "Incorrect vertex format" );
        Assert( cmd.topology == ImmediateModeRenderingTopology.Triangle, "Incorrect Topology" );

        AddVertexData( uv0 );
        AddVertexData( color0 );
        AddVertexData( vert0 );

        AddVertexData( uv1 );
        AddVertexData( color1 );
        AddVertexData( vert1 );

        AddVertexData( uv2 );
        AddVertexData( color2 );
        AddVertexData( vert2 );

        AddVertexData( uv3 );
        AddVertexData( color3 );
        AddVertexData( vert3 );

        _indexBuffer.Add( cmd.vertexCount + 0 );
        _indexBuffer.Add( cmd.vertexCount + 1 );
        _indexBuffer.Add( cmd.vertexCount + 2 );

        _indexBuffer.Add( cmd.vertexCount + 2 );
        _indexBuffer.Add( cmd.vertexCount + 3 );
        _indexBuffer.Add( cmd.vertexCount + 0 );

        cmd.vertexCount += 4;
        cmd.indexCount += 6;
    }

    public void EndImmediate()
    {
        Assert( _currentCommandIndex != -1, "Rendering Command not started" );

        ImmediateModeRenderingCommand cmd = _commands[ _currentCommandIndex ];
        cmd.vertexOffset = _vertexSize;
        cmd.indexOffset = _indexSize;

        _currentCommandIndex = -1;
        
        _vertexSize = _vertexBuffer.Count;
        _indexSize = _indexBuffer.Count;
    }

    private void AddVertexData( Vector2 v )
    {
        _vertexBuffer.Add( v.x );
        _vertexBuffer.Add( v.y );
    }

    private void AddVertexData( Vector3 v )
    {
        _vertexBuffer.Add( v.x );
        _vertexBuffer.Add( v.y );
        _vertexBuffer.Add( v.z );
    }

    private void AddVertexData( Color c )
    {
        _vertexBuffer.Add( c.r );
        _vertexBuffer.Add( c.g );
        _vertexBuffer.Add( c.b );
        _vertexBuffer.Add( c.a );
    }

    private static Material GetDefaultBlitMaterial()
    {
        if( DefaultBlitMaterial == null )
        {
            DefaultBlitMaterial = new Material(Shader.Find("Unlit/Transparent Colored" ) );
            DefaultBlitMaterial.hideFlags = HideFlags.HideAndDontSave;
            DefaultBlitMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }

        return DefaultBlitMaterial;
    }

    private void Assert( bool test, string msg )
    {

    }
}
