#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using anogame.framework.UI;

namespace anogame.framework.Editor
{
    /// <summary>
    /// UIViewBaseのカスタムエディター
    /// </summary>
    [CustomEditor(typeof(UIViewBase), true)]
    public class UIViewBaseEditor : UnityEditor.Editor
    {
        private SerializedProperty isVisibleProp;
        private SerializedProperty showAnimationSettingsProp;
        private SerializedProperty useAnimatorProp;
        private SerializedProperty openTriggerProp;
        private SerializedProperty closeTriggerProp;
        private SerializedProperty isOpenBoolProp;
        private SerializedProperty animationTimeoutProp;

        private void OnEnable()
        {
            // SerializedPropertyを取得
            isVisibleProp = serializedObject.FindProperty("isVisible");
            showAnimationSettingsProp = serializedObject.FindProperty("showAnimationSettings");
            useAnimatorProp = serializedObject.FindProperty("useAnimator");
            openTriggerProp = serializedObject.FindProperty("openTrigger");
            closeTriggerProp = serializedObject.FindProperty("closeTrigger");
            isOpenBoolProp = serializedObject.FindProperty("isOpenBool");
            animationTimeoutProp = serializedObject.FindProperty("animationTimeout");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 基本設定
            EditorGUILayout.PropertyField(isVisibleProp);

            EditorGUILayout.Space();

            // アニメーション設定ヘッダー
            EditorGUILayout.LabelField("アニメーション設定", EditorStyles.boldLabel);

            // useAnimatorは常に表示
            EditorGUILayout.PropertyField(useAnimatorProp, new GUIContent("Animatorを使用"));

            // useAnimatorが有効な場合のみ詳細設定を表示
            if (useAnimatorProp.boolValue)
            {
                // アニメーション設定の表示/非表示トグル
                EditorGUI.indentLevel++;
                showAnimationSettingsProp.boolValue = EditorGUILayout.Foldout(
                    showAnimationSettingsProp.boolValue,
                    "詳細設定",
                    true
                );
                EditorGUI.indentLevel--;
            }

            // 詳細設定は条件付きで表示
            if (useAnimatorProp.boolValue && showAnimationSettingsProp.boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Animatorパラメータ", EditorStyles.miniBoldLabel);

                EditorGUILayout.PropertyField(openTriggerProp, new GUIContent("開くトリガー"));
                EditorGUILayout.PropertyField(closeTriggerProp, new GUIContent("閉じるトリガー"));
                EditorGUILayout.PropertyField(isOpenBoolProp, new GUIContent("開いているかのBool"));

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("タイムアウト設定", EditorStyles.miniBoldLabel);

                EditorGUILayout.PropertyField(animationTimeoutProp, new GUIContent("アニメーションタイムアウト"));

                // ヘルプボックスを表示
                if (useAnimatorProp.boolValue)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.HelpBox(
                        "Animatorに以下のパラメータを設定してください：\n" +
                        $"• Trigger: {openTriggerProp.stringValue}\n" +
                        $"• Trigger: {closeTriggerProp.stringValue}\n" +
                        $"• Bool: {isOpenBoolProp.stringValue}",
                        MessageType.Info
                    );
                }

                EditorGUI.indentLevel--;
            }

            // Animatorが見つからない場合の警告
            if (useAnimatorProp.boolValue && Application.isPlaying)
            {
                var uiView = target as UIViewBase;
                if (uiView != null && uiView.GetComponent<Animator>() == null)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.HelpBox(
                        "Animatorコンポーネントが見つかりません。\nAnimatorを追加するか、「Animatorを使用」を無効にしてください。",
                        MessageType.Warning
                    );
                }
            }

            serializedObject.ApplyModifiedProperties();

            // その他のプロパティがあれば表示（継承先のカスタムフィールドなど）
            DrawPropertiesExcluding(serializedObject,
                "m_Script",
                "isVisible",
                "showAnimationSettings",
                "useAnimator",
                "openTrigger",
                "closeTrigger",
                "isOpenBool",
                "animationTimeout"
            );
        }
    }
}
#endif