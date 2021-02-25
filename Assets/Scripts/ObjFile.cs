using System;
using System.IO;
using UnityEngine;

class ObjFile : IDisposable
{
    public bool IsLoaded { get; set; } = false;
    public int NumVertices { get; private set; }
    public int NumFaces { get; private set; }

    public Vector3[] Vertices { get; private set; }
    public Vector3[] Normals { get; private set; }
    public Color[] Colours { get; private set; }
    
    public int[] Triangles { get; private set; }

    public ObjFile(Stream fileStr)
    {
        ModelStream = fileStr;
    }

    public Stream ModelStream { get; }

    public void Init()
    {
        try
        {
            NumVertices = 0;
            NumFaces = 0;
            ObjParser.Parse(this, s => NumVertices++, null, null, s => NumFaces++);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    Tuple<Vector3, Color> ReadVertex(string vertexStr)
    {
        var splitStr = vertexStr.Split(' ');
        return Tuple.Create(new Vector3(float.Parse(splitStr[1]), float.Parse(splitStr[2]), float.Parse(splitStr[3])),
            new Color(float.Parse(splitStr[4]), float.Parse(splitStr[5]), float.Parse(splitStr[6])));
    }
    Vector3 ReadNormal(string normalStr)
    {
        var splitStr = normalStr.Split(' ');
        return new Vector3(float.Parse(splitStr[1]), float.Parse(splitStr[2]), float.Parse(splitStr[3]));
    }

    Tuple<int, int>[] ReadFace(string normalStr)
    {
        var splitStr = normalStr.Split(' ');
        var result = new Tuple<int, int>[3];
        for (int i=1;i<4;i++)
        {
            var indices = splitStr[i].Split('/');
            result[i-1] = Tuple.Create(int.Parse(indices[0])-1, int.Parse(indices[2])-1);
        }
        return result;
    }

    internal void GenerateBuffers()
    {
        ModelStream.Position = 0;
        Vertices = new Vector3[NumVertices];
        Normals = new Vector3[NumVertices];
        Colours = new Color[NumVertices];

        Triangles = new int[NumFaces*3];

        int vertexIdx = 0;
        int normalIdx = 0;
        int triangleIdx = 0;
        ObjParser.Parse(this, fileLine =>
        {
            var vertex = ReadVertex(fileLine);
            var index = vertexIdx++;
            Vertices[index] = vertex.Item1;
            Colours[index] = vertex.Item2;
        },
        fileLine =>
        {
            // Not sure if we need normals, lighting, etc.
            Normals[normalIdx++] = ReadNormal(fileLine);
        },
        null,
        fileLine =>
        {
            // We only expect vertex and vertex normal data to be defined in the face.
            var face = ReadFace(fileLine);
            for (int faceIfx = 0; faceIfx < 3; faceIfx++)
            {
                Triangles[triangleIdx++] = face[faceIfx].Item1;
            }
        });

        IsLoaded = true;
    }

    #region IDisposable Support
    
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ModelStream?.Dispose();
            }
            disposedValue = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }
    #endregion
}
