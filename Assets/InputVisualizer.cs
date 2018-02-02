using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputVisualizer : MonoBehaviour
{
    public struct Inputs
    {
        public float horizontal, vertical, thrust, brake, shoot;
    }

    const float MAX_WIDTH = 200;
    const float MIN_WIDTH = 50;

    public Inputs current;
    
    public void SetCurrent(Inputs current)
    {
        this.current = current;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        // Negative values are valid for these two
        // We need a percentage from [-1, 1]
        //
        // https://stackoverflow.com/a/11107254
        // range = top_value - bottom_value
        // percent = (value - bottom_value) / range
        float h_range = 1 - -1;
        float h_percent = (current.horizontal - -1) / h_range;

        float v_range = 1 - -1;
        float v_percent = (current.vertical - -1) / v_range;

        GUILayout.Box("Horizontal", GUILayout.Width(Mathf.Lerp(MIN_WIDTH, MAX_WIDTH, h_percent)));
        GUILayout.Box("Vertical", GUILayout.Width(Mathf.Lerp(MIN_WIDTH, MAX_WIDTH, v_percent)));

        GUILayout.Box("Thrust", GUILayout.Width(Mathf.Lerp(MIN_WIDTH, MAX_WIDTH, current.thrust)));
        GUILayout.Box("Brake", GUILayout.Width(Mathf.Lerp(MIN_WIDTH, MAX_WIDTH, current.brake)));
        GUILayout.Box("Shoot", GUILayout.Width(Mathf.Lerp(MIN_WIDTH, MAX_WIDTH, current.shoot)));

        GUILayout.EndVertical();
    }
}