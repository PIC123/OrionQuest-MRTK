using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ObjLoader : MonoBehaviour
{

    public string modelLocationUriString = @"https://gltfmodels.blob.core.windows.net/models/monkey.obj?sp=r&st=2020-04-02T16:24:26Z&se=2020-12-16T01:24:26Z&spr=https&sv=2019-02-02&sr=b&sig=1kWI5JGs5QrvyPb8BI1nAe7hXsUN0bycPFDHlHHEyK4%3D";
    //public string modelLocationUri = @"https://gltfmodels.blob.core.windows.net/models/mesh.obj?sp=r&st=2020-04-01T09:48:03Z&se=2020-07-01T17:48:03Z&spr=https&sv=2019-02-02&sr=b&sig=v4dCpZokG5CUw4A7FKGnPk9h%2BSs68o9NZepX3k8BsrE%3D";
    //public string modelLocationUri = @"https://gltfmodels.blob.core.windows.net/models/greencube.obj?sp=r&st=2020-04-01T11:37:26Z&se=2020-09-28T19:37:26Z&spr=https&sv=2019-02-02&sr=b&sig=mvp4jxWZ%2BSoHnovyC0IcjiPXCODGAXMkCAt6nrUwONI%3D";
    private ObjFile _objFile = null;
    private Mesh _mesh = null;
    private bool _loaded = false;

    public enum ShaderType { Lit, Unlit };
    public ShaderType shading;

    // Start is called before the first frame update
    void Start()
    {
        var modelLocationUri = new Uri(modelLocationUriString);
        DownloadModelAsync(modelLocationUri).ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                foreach (var ex in t.Exception.InnerExceptions)
                {
                    Debug.LogException(ex);
                }
                Debug.LogException(t.Exception);
            }
        });

        _mesh = CreateUnityMeshObject(modelLocationUri.Segments.Last());
    }

    async Task DownloadModelAsync(Uri modelUri)
    {
        HttpResponseMessage response = null;
        using (var http = new HttpClient())
        {
            response = await http.GetAsync(modelUri);
            response.EnsureSuccessStatusCode();
        }

        await Task.Run(async () =>
        {
            // This part could be implemented as a native dll to optimise the speed of 
            // the loading. It would be a bit awkward as probably wouldn't want to write
            // the whole file into memory but instead stream it across the managed to native boundary.
            // You could write a class inheriting from stream_buffer that wraps a .NET stream object. 
            // That would be the most efficient way because no memory would be needlessly copied.
            // Deferring that until this is too slow..
            //
            var modelStream = await response.Content.ReadAsStreamAsync();
            _objFile = new ObjFile(modelStream);
            _objFile.Init();
            _objFile.GenerateBuffers();
            _objFile.Dispose();
        });
    }

    Mesh CreateUnityMeshObject(string modelName)
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        Mesh mesh;
        meshFilter.mesh = mesh = new Mesh();
        mesh.name = modelName;

        var shaderId = (shading == ShaderType.Lit) ?
            "Unlit/VertexColourShaderOneLight" : "Unlit/VertexColourShader";

        var mat = new Material(Shader.Find(shaderId));
        meshRenderer.material = mat;

        return mesh;
    }

    void GenerateMesh(Mesh mesh, ObjFile objFile)
    {
        // This function needs to be called from the main render thread
        //
        if (objFile.Vertices.Length > 65534)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        mesh.vertices = objFile.Vertices;
        mesh.triangles = objFile.Triangles;
        mesh.colors = objFile.Colours;
        mesh.normals = objFile.Normals;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_loaded &&_mesh && (_objFile != null) && _objFile.IsLoaded)
        {
            GenerateMesh(_mesh, _objFile);
            _loaded = true;
        }
    }
}

static class ObjParser
{
    private static void defaultAction(string s) { }

    public static void Parse(ObjFile model, Action<string> OnVertex, Action<string> OnVertexNormal = null, 
        Action<string> OnComment = null, Action<string> OnFace = null)
    {
        if (OnVertex == null)
            OnVertex = defaultAction;
        if (OnComment == null)
            OnComment = defaultAction;
        if (OnVertexNormal == null)
            OnVertexNormal = defaultAction;
        if (OnFace == null)
            OnFace = defaultAction;

        StreamReader sr = new StreamReader(model.ModelStream);
        while (sr.Peek() >= 0)
        {
            var line = sr.ReadLine();
            if (line.StartsWith("#"))
            {
                OnComment(line);
            }
            else if (line.StartsWith("v "))
            {
                OnVertex(line);
            }
            else if (line.StartsWith("vn"))
            {
                OnVertexNormal(line);
            }
            else if (line.StartsWith("f"))
            {
                OnFace(line);
            }
        }
    }
}
