using System;
using System.Collections.Generic;
using System.IO;
using R3;
using UnityEditor;
using UnityEngine;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorFolderSettingFolderPathView : IDisposable
    {
        private const int FOLDER_INDENT_SIZE = 20;
        private const int FOLDER_ROW_HEIGHT = 25;
        private readonly Color TREE_COLOR = new(0.4f, 0.4f, 0.4f, 1f);
        
        private Vector2 _scrollPosition;
        
        private readonly Subject<int> _onFolderButtonClickedSubject = new();
        public Observable<int> OnFolderButtonClickedAsObservable => _onFolderButtonClickedSubject;

        internal void Draw(IReadOnlyList<ClassGeneratorFolderSettingPathDto> dtos)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                if (dtos == null || dtos.Count == 0)
                {
                    EditorGUILayout.HelpBox("No data available.", MessageType.Info);
                    return;
                }

                EditorGUILayout.BeginVertical();
                {
                    var parentFolderCount = new Dictionary<int, int>();
                    var currentDepth = 1;
                    
                    for(int i = 0; i < dtos.Count; i++)
                    {
                        var path = dtos[i].Info.Path;
                        var depth = dtos[i].Info.Depth;

                        // フォルダの階層が上に戻ったときに、今より下の階層のフォルダ個数をリセットする
                        if (currentDepth > depth)
                        {
                            for (int j = depth + 1; j <= currentDepth; j++)
                            {
                                parentFolderCount[j] = 0;
                            }
                        }
                        currentDepth = depth;
                        
                        // 親のフォルダまでのいくつフォルダが存在するかを計算してツリー構造を描画するときに使用
                        for (int j = 2; j <= depth; j++)
                        {
                            // 該当のkeyが未初期化であれば初期化
                            if (!parentFolderCount.TryGetValue(j, out var value))
                            {
                                parentFolderCount[j] = 0;
                            }
                            parentFolderCount[j]++;
                        }

                        EditorGUILayout.BeginHorizontal();
                        {
                            if (depth > 1)
                            {
                                DrawTree(depth, parentFolderCount[depth]);
                            }

                            var cleanPath = path.TrimEnd('/', '\\');
                            var lastSegment = Path.GetFileName(cleanPath);
                            
                            // ボタン本体。クリックされたら true を返す
                            if (GUILayout.Button(lastSegment, GUILayout.Height(FOLDER_ROW_HEIGHT), GUILayout.ExpandWidth(false)))
                            {
                                _onFolderButtonClickedSubject.OnNext(currentDepth);
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
            _onFolderButtonClickedSubject?.Dispose();
        }
    }
}