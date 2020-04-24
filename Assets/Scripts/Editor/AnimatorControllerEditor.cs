using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR    
namespace Editor
{
    [CustomEditor(typeof(Animator))]
    public class AnimatorControllerEditor : UnityEditor.Editor
    {
        private bool _foldout;
    
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        
            var animator = target as Animator;
            if (animator == null)
            {
                return;
            }

            EditorGUILayout.Separator();
            _foldout = EditorGUILayout.Foldout(_foldout, "Parameters");
            if (!_foldout)
            {
                return;
            }
        
            foreach (var parameter in animator.parameters)
            {
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        var f = EditorGUILayout.FloatField(parameter.name, animator.GetFloat(parameter.name));
                        animator.SetFloat(parameter.name, f);
                        break;
                    case AnimatorControllerParameterType.Int:
                        var i = EditorGUILayout.IntField(parameter.name, animator.GetInteger(parameter.name));
                        animator.SetInteger(parameter.name, i);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        var b = EditorGUILayout.Toggle(parameter.name, animator.GetBool(parameter.name));
                        animator.SetBool(parameter.name, b);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(parameter.name))
                        {
                            animator.SetTrigger(parameter.name);
                        }
                        if (GUILayout.Button("Reset " + parameter.name))
                        {
                            animator.ResetTrigger(parameter.name);
                        }
                        GUILayout.EndHorizontal();
                        break;
                }
            }
        }
    }
}
#endif
