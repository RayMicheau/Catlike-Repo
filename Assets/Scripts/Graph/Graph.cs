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
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, MultiSineFunctionMorph
    };

    

    const float pi = Mathf.PI;

    /// <summary>
    /// Function to represent the math func f(x, t) = sin(pi(x + t))
    /// </summary>
    /// <param name="x">X Coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="t">Time</param>
    /// <returns>Y Coordinate</returns>
    static float SineFunction(float x, float z, float t)
    {
        return Mathf.Sin(Mathf.PI * (x + t));
    }

    /// <summary>
    /// Function to represent the math func f(x, t) = sin(pi(x + t)) + (sin(2*pi(x + t)) / 2)
    /// </summary>
    /// <param name="x">X Coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="t">Time</param>
    /// <returns>Y Coordinate</returns>
    static float MultiSineFunction(float x, float z, float t)
    {
        float y = Mathf.Sin(Mathf.PI * (x + t));
        y += Mathf.Sin(2f * Mathf.PI * (x + t)) / 2f;
        y *= 0.66f;
        return y;
    }

    static float MultiSineFunctionMorph(float x, float z, float t)
    {
        float y = Mathf.Sin(Mathf.PI * (x + t));
        y += Mathf.Sin(2f * Mathf.PI * (x + 2f * t)) / 2f;
        y *= 0.66f;
        return y;
    }


    /// <summary>
    /// Function to represent the math func f(x, z, t) = (sin(pi(x + t)) + sin(pi(z + t))) / 2
    /// </summary>
    /// <param name="x">X Coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="t">Time</param>
    /// <returns>Y Coordinate</returns>
    static float Sine2DFunction(float x, float z, float t) {
        float y = Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(pi * (z + t));
        y *= 0.5f;
        return y;
    }

    /// <summary>
    /// Function to represent the math func f(x, z, t) = M + Sx + Sz
    /// Where M is the main wave, Sx represents the secondary wave based on x
    /// and Sz representes the secondary wave based on z
    /// </summary>
    /// <param name="x">X Coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="t">Time</param>
    /// <returns>Y Coordinate</returns>
    static float MultiSine2DFunction(float x, float z, float t) {
        //Main wave: M = sin(pi(x + z + t/2)) multiplied by 4 to make it BIG
        float y = 4f * Mathf.Sin(pi * (x + z + t * .5f));
        //Secondary wave: Sx = sin(pi(x + t))
        y += Mathf.Sin(pi * (x + t));
        //Third wave: Sz = sin(2pi(z + 2t))
        y += Mathf.Sin(2f * pi * (z + 2f * t)) * 0.5f;
        //Use this to keep it in the -1 - 1 range
        y *= 1f / 5.5f;
        //returns the function f(x, z, t) = 4M + Sx + Sz/2
        return y;
    }

    /// <summary>
    /// Function to represent the equation f(x, z, t) = sin(pi * distance)
    /// To decrease the amplitude, we use 1/10D as the amplitude, ensuring it decreases
    /// as the distance away increases
    /// </summary>
    /// <param name="x">X Coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="t">Time</param>
    /// <returns>Y Coordinate</returns>
    static float Ripple(float x, float z, float t)    {
        float d = Mathf.Sqrt(x * x + z * z);
        float y = Mathf.Sin(4f * pi * d - t);
        y /= 1f + 10f * d;
        return y;
    }

    private void Awake()
    {
        //Sets the scale of cubes, based on the given resolution
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;

        Vector3 position;
        position.y = 0f;
        position.z = 0f;

        points = new Transform[resolution * resolution];
        
        for (int i = 0, z = 0; z < resolution; z++)
        {
            position.z = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                Transform point = Instantiate(pointPrefab);
                position.x = (x + 0.5f) * step - 1f;

                point.localPosition = position;
                point.localScale = scale;

                //sets each spawned graph node to be a child of this object
                point.SetParent(transform, false);
                points[i] = point;
            }
        }
    }

    void Update()
    {
        float t = Time.time;
        GraphFunction f = functions[(int)function];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;

            position.y = f(position.x, position.z, t);
            point.localPosition = position;
        }

    }
}