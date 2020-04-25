using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class DepthAudioFilterController : MonoBehaviour
{

    [SerializeField] private AudioLowPassFilter audioLowPassFilter;
    [Tooltip("Uses")]
    [HideInInspector] public AnimationCurve lowPassCurve;
    

    // Update is called once per frame
    void Update()
    {
        if (!audioLowPassFilter || lowPassCurve == null)
        {
            return;
        }

        var p = Hub.Get<CameraMovement>().GetDepthPercentage();
        audioLowPassFilter.cutoffFrequency = lowPassCurve.Evaluate(p);
    }
}

#if  UNITY_EDITOR
[CustomEditor(typeof(DepthAudioFilterController))]
public class DepthAudioFilterControllerEditor : Editor
{
    private static readonly Rect Ranges = new Rect(0,0,1,22000);
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = target as DepthAudioFilterController;
        t.lowPassCurve = EditorGUILayout.CurveField("Low Pass Curve 123", t.lowPassCurve, Color.green, Ranges);

    }
}
#endif
