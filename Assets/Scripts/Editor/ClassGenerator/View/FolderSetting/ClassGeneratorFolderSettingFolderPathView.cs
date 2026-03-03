using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingFolderPathView
    {
        private const int FOLDER_INDENT_SIZE = 20;
        private const int FOLDER_ROW_HEIGHT = 25;
        private readonly Color TREE_COLOR = new(0.4f, 0.4f, 0.4f, 1f);
        
        private Vector2 _scrollPosition;

        internal void Draw(IReadOnlyDictionary<string, int> folderPathDict)
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
    }
}