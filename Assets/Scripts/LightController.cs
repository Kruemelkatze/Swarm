using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private CameraMovement _cm;
    [HideInInspector] public AnimationCurve lightIntensityCurve;
    [SerializeField] private Light2D[] lights;

    // Start is called before the first frame update
    void Start()
    {
        _cm = Hub.Get<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        var depth = _cm.GetDepthPercentage();
        var value = lightIntensityCurve.Evaluate(depth);
        
        foreach (var light in lights)
        {
            light.intensity = value;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LightController))]
    public class LightControllerEditor : Editor
    {
        private static readonly Rect Ranges = new Rect(0,0,1,1);
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var t = target as LightController;
            t.lightIntensityCurve = UnityEditor.EditorGUILayout.CurveField("Light Intensity Curve", t.lightIntensityCurve, Color.yellow, Ranges);
        }
    }
#endif
}