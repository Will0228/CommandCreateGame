using System;
using System.Collections.Generic;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingView : IDisposable
    {
        private const int FOLDER_INDENT_SIZE = 20;
        private const int FOLDER_ROW_HEIGHT = 25;
        private Color32 TREE_COLOR = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        private Vector2 _scrollPosition;
        private AppLayerType _selectedLayer = AppLayerType.None;
        
        private readonly Subject<(ComponentRoleType, string)> _onSetFolderPathSubject = new();
        public Observable<(ComponentRoleType, string)> OnSetFolderPathAsObservable => _onSetFolderPathSubject;
        
        internal void Draw(Rect windowPosition,
            IReadOnlyDictionary<AppLayerType, string> layerPathDict,
            IReadOnlyDictionary<string, int> folderPathDict)
        {
            var halfWidth = windowPosition.width / 2f - 2f;
            
            EditorGUILayout.BeginHorizontal();
            DrawFolderSettingAreaByAppLayer(layerPathDict, halfWidth);
            DrawFolderPath(folderPathDict);
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 層ごとのフォルダ設定項目を列挙
        /// </summary>
        private void DrawFolderSettingAreaByAppLayer(IReadOnlyDictionary<AppLayerType, string> layerPathDict,
            float sectionRectWidth)
        {
            EditorGUILayout.BeginVertical();
            
            foreach (var dict in layerPathDict)
            {
                if (dict.Key == AppLayerType.None)
                {
                    continue;
                }
                
                var sectionRect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(sectionRectWidth));
                {
                    // 選択されている場合の枠の強調表示
                    if (_selectedLayer == dict.Key)
                    {
                        var originalColor = GUI.color;
                        GUI.color = Color.yellow;
                        GUI.Box(sectionRect, GUIContent.none, EditorStyles.selectionRect);
                        GUI.color = originalColor;
                    }
                    
                    EditorGUILayout.LabelField($"{dict.Key} Layer", GUILayout.Width(120));
                    var buttonLabel = dict.Value;
                    if (GUILayout.Button(buttonLabel, GUILayout.Width(80)))
                    {
                        _selectedLayer = dict.Key;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawFolderPath(IReadOnlyDictionary<string, int> folderPathDict)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                if (folderPathDict == null || folderPathDict.Count == 0)
                {
                    EditorGUILayout.HelpBox("No data available.", MessageType.Info);
                    return;
                }

                EditorGUILayout.BeginVertical();
                {
                    var parentFolderCount = new Dictionary<int, int>();
                    var currentDepth = 1;
                    
                    foreach (var kvp in folderPathDict)
                    {
                        var path = kvp.Key;
                        var depth = kvp.Value;

                        // フォルダの階層が上に戻ったときに、今より下の階層のフォルダ個数をリセットする
                        if (currentDepth > depth)
                        {
                            for (int i = depth + 1; i <= currentDepth; i++)
                            {
                                parentFolderCount[i] = 0;
                            }
                        }
                        currentDepth = depth;
                        
                        // 親のフォルダまでのいくつフォルダが存在するかを計算してツリー構造を描画するときに使用
                        for (int i = 2; i <= depth; i++)
                        {
                            // 該当のkeyが未初期化であれば初期化
                            if (!parentFolderCount.TryGetValue(i, out var value))
                            {
                                parentFolderCount[i] = 0;
                            }
                            parentFolderCount[i]++;
                        }

                        EditorGUILayout.BeginHorizontal();
                        {
                            if (depth > 1)
                            {
                                DrawTree(depth, parentFolderCount[depth]);
                            }
                            
                            // ボタン本体。クリックされたら true を返す
                            if (GUILayout.Button(path, GUILayout.Height(FOLDER_ROW_HEIGHT), GUILayout.ExpandWidth(false)))
                            {
                                // OnFolderButtonClicked(path);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
                
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawTree(int depth, int count)
        {
            // ツリー線の描画エリア計算
            var prefixWidth = (depth - 1) * FOLDER_INDENT_SIZE;
            var lineRect = GUILayoutUtility.GetRect(prefixWidth, FOLDER_ROW_HEIGHT, GUILayout.ExpandWidth(false));
            
            var halfIndent = FOLDER_INDENT_SIZE / 2f;
            var centerX = lineRect.x + (depth - 1) * FOLDER_INDENT_SIZE - halfIndent;
            var centerY = lineRect.y + (FOLDER_ROW_HEIGHT / 2f);

            // 横線
            EditorGUI.DrawRect(new Rect(centerX, centerY, halfIndent, 1f), TREE_COLOR);
            // 縦線
            var height = (FOLDER_ROW_HEIGHT - 10) + (FOLDER_ROW_HEIGHT * (count - 1));
            EditorGUI.DrawRect(new Rect(centerX, centerY, 1f, -height), TREE_COLOR);
        }
        
        void IDisposable.Dispose()
        {
            
        }
    }
}