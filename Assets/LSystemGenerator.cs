using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using System.Linq;

public class LSystemGenerator : MonoBehaviour {

    [SerializeField] private float angle = 25.0f;
    [SerializeField] private string axiom = "F";
    [SerializeField] private int loopingAmount;

    [SerializeField] private char ruleWhatToReplace = 'F';
    [SerializeField] private string ruleReplace = "FF+[+F-F-F]-[-F+F+F]";


    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;

    [SerializeField] private float length = 10.0f;
    private string currentString = string.Empty;

    private void Start() {
        transformStack = new Stack<TransformInfo>();
        rules = new Dictionary<char, string> {
            { ruleWhatToReplace, ruleReplace }
        };

        transform.Rotate(Vector3.right * -90.0f);

        currentString = axiom;

        for(int i = 0; i < loopingAmount; i++)
        {
            GenerateSystem();
        }

        CreateResult();
        Debug.Log(currentString);
    }

    private void GenerateSystem() {
        StringBuilder sb = new StringBuilder();

        foreach (char c in currentString) {
            sb.Append(
                rules.ContainsKey(c) ? rules[c] : c.ToString()
                );
        }

        currentString = sb.ToString();

        length /= 3.0f;
    }

    private void CreateResult()
    {
        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.forward * length);
                    Debug.DrawLine(initialPosition, transform.position, Color.white, 1000000f, false);
                    break;
                case '+':
                    transform.Rotate(Vector3.up * angle);
                    break;
                case '-':
                    transform.Rotate(Vector3.up * -angle);
                    break;
                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });

                    break;
                case ']':
                    {
                        TransformInfo ti = transformStack.Pop();
                        transform.position = ti.position;
                        transform.rotation = ti.rotation;
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }
    }



    private MeshFilter meshFilter;
    private PolygonCollider2D polygonCollider;

    void CreateShape()
    {
        meshFilter = GetComponent<MeshFilter>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        // Let's assume you have a FooBar method that returns your array of points
        Vector2[] myArrayOfPoints = FooBar();

        // We need to convert those Vector2 into Vector3
        Vector3[] vertices3D = System.Array.ConvertAll<Vector2, Vector3>(vertices2D, v => v);

        // Then, we need to calculate the indices of each vertex.
        // For that, you can use the Triangulator class available in:
        // http://wiki.unity3d.com/index.php?title=Triangulator
        Triangulator triangulator = new Triangulator(vertices2D);
        int[] indices = triangulator.Triangulate();

        // Now we need to create a color for each vertex. I decided to put them all white but you
        // can adjust it and change it according to your needs
        Color[] colors = Enumerable.Range(0, vertices3D.Length)
            .Select(i => Color.white)
            .ToArray();

        // Finally, create a Mesh and set vertices, indices and colors
        Mesh mesh = new Mesh();
        mesh.vertices = vertices3D;
        mesh.triangles = indices;
        mesh.colors = colors;

        // Recalculate the shape of the mesh
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // And add the mesh to your MeshFilter
        meshFilter.mesh = mesh;

        // For the collisions, basically you need to add the vertices to your PolygonCollider2D            
        polygonCollider.points = vertices2D;
    }
}
