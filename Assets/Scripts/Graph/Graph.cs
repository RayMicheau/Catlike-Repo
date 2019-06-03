using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform pointPrefab;

    //Resolution for the graph to use
    [Range(10, 100)]
    public int resolution;

    public GraphFunctionName function;

    //Array of points to track and edit positions of graph nodes
    Transform[] points;

    static readonly GraphFunction[] functions = {
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, Cylinder, Sphere, Torus
    };

    const float pi = Mathf.PI;

    /// <summary>
    /// Math func: f(u, v, t) = [u, sin(pi(u + t)), v]
    /// </summary>

    static Vector3 SineFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.z = z;
        return p;
    }


    static Vector3 MultiSineFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(Mathf.PI * (x + t));
        p.y += Mathf.Sin(2f * Mathf.PI * (z + t)) / 2f;
        p.y *= 0.66f;
        p.z = z;
        return p;
    }


    static Vector3 Sine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(pi * (z + t));
        p.y *= 0.5f;
        p.z = z;
        return p;
    }


    static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        //Main wave: M = sin(pi(x + z + t/2)) multiplied by 4 to make it BIG
        p.y = 4f * Mathf.Sin(pi * (x + z + t * .5f));
        //Secondary wave: Sx = sin(pi(x + t))
        p.y += Mathf.Sin(pi * (x + t));
        //Third wave: Sz = sin(2pi(z + 2t))
        p.y += Mathf.Sin(2f * pi * (z + 2f * t)) * 0.5f;
        //Use this to keep it in the -1 - 1 range
        p.y *= 1f / 5.5f;
        p.z = z;

        //returns the function f(x, z, t) = 4M + Sx + Sz/2
        return p;
    }


    static Vector3 Ripple(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        float d = Mathf.Sqrt(x * x + z * z);
        p.y = Mathf.Sin(pi * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = z;
        return p;
    }


    //Starting with just a circle, all points on a circle can be defined via [sin(theta), cos(theta)] with theta going from 0 -> 2pi
    //We can use u instead, which in this instance goes from -1 to 1. To create an XZ circle, we use the function f(u) = [sin(pi*u), 0, cos(pi*u)]
    static Vector3 Cylinder(float u, float v, float t)
    {
        float r = 0.8f + Mathf.Sin(pi * (6f * u + 2f * v + t)) * 0.2f;
        Vector3 p;
        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 Sphere(float u, float v, float t)
    {
        float r = 0.8f + (Mathf.Sin(pi * (6f * u + t)) * 0.1f) + (Mathf.Sin(pi * (4f * v + t)) * 0.1f);
        float s = r * Mathf.Cos(pi * v * 0.5f);
        Vector3 p;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r *Mathf.Sin(pi * v * .5f);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 Torus(float u, float v, float t) { 
        Vector3 p;
        //Making r1 greater than 1 will open a hole in the middle of the torus
        float r1 = 0.65f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        //using r2 determines the circles wrapping around the ring
        float r2 = 0.2f + Mathf.Sin(pi * (4f * v + t)) * 0.05f;
        float s = r2 * Mathf.Cos(pi * v) + r1;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }


    private void Awake()
    {
        //Sets the scale of cubes, based on the given resolution
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;

        points = new Transform[resolution * resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;

            //sets each spawned graph node to be a child of this object
            point.SetParent(transform, false);
            points[i] = point;
        }

    }

    void Update()
    {
        float t = Time.time;
        GraphFunction f = functions[(int)function];
        float step = 2f / resolution;
        for (int i = 0, z = 0; z < resolution; z++)
        {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = f(u, v, t);
            }
        }
    }
}