using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OmicronFSM.Editor
{
    public class GraphWindow : EditorWindow
    {
        private readonly Dictionary<Color, Texture2D> _textures = new Dictionary<Color, Texture2D>();
        private string[] _stateNames = new string[0];
        private StateMachine _machine;
        private string _name;
        private int _selectedStateIndex;
        private bool _displayCurrent = true;
        private GraphInfo _graphInfo;
        private GUIStyle _default;
        private GUIStyle _active;
        private GUIStyle _previous;
        private GUIStyle _empty;

        public static void Display(StateMachine machine, string name)
        {
            var window = GetWindow<GraphWindow>(false);
            window.titleContent = new GUIContent($"FSM Display");
            window.Init(machine, name);
        }

        private void OnDestroy()
        {
            foreach (var texture in _textures.Values)
            {
                if (texture == null)
                    continue;

                if (Application.isPlaying)
                    Destroy(texture);
                else
                    DestroyImmediate(texture);
            }

            _textures.Clear();
            _default = null;
            _active = null;
            _previous = null;
            _empty = null;
        }

        private void Update()
        {
            if (Application.isPlaying)
                Repaint();
        }

        private void OnGUI()
        {
            if (Application.isPlaying == false)
            {
                Clear();
                EditorGUILayout.HelpBox($"FSM Display works only when application is playing.", MessageType.Info);
                return;
            }

            if (_machine == null)
            {
                GUILayout.Label($"Nothing to display", EditorStyles.miniBoldLabel);
                EditorGUILayout.HelpBox($"Select GameObject with \"[SerializeField] StateMachine\" field and click \"Display Graph\" button.", MessageType.Info);
                return;
            }

            InitStyles();

            GUILayout.Label($"FSM: {_name}", EditorStyles.boldLabel);
            GUILayout.Label($"Total states: {_machine.GetStatesCount()}", EditorStyles.miniBoldLabel);

            if (_machine.GetStatesCount() == 0)
            {
                EditorGUILayout.HelpBox($"No states in {_name}, nothing to display.", MessageType.Warning);
                return;
            }

            int? currentStateIndex = _machine.GetCurrentStateIndex();

            DrawAllStates();

            GUILayout.BeginHorizontal("box");
            float columnWidth = position.width / 3f;
            var width = GUILayout.Width(columnWidth);

            StateInfo? previousState = _graphInfo.PreviousStateIndex.HasValue ? _machine.GetStateInfo(_graphInfo.PreviousStateIndex.Value) : null;
            StateInfo? currentState = currentStateIndex.HasValue ? _machine.GetStateInfo(currentStateIndex.Value) : null;

            if (GUILayout.Button($"Previous state: {(previousState.HasValue ? previousState.Value.Info : "null")}", _previous, width) && previousState.HasValue)
                SelectState(previousState.Value);

            if (GUILayout.Button($"Current state: {(currentState.HasValue ? currentState.Value.Info : "null")}", _active, width) && currentState.HasValue)
                    SelectState(currentState.Value);

            _displayCurrent = GUILayout.Toggle(_displayCurrent, "Follow Current State", width);

            if (currentStateIndex.HasValue)
            {
                if (_displayCurrent)
                    _selectedStateIndex = currentStateIndex.Value;
            }

            _machine.GetGraphInfo(ref _graphInfo, _selectedStateIndex);

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            DrawInfo(_graphInfo);
        }

        private void DrawAllStates()
        {
            if (_stateNames.Length != _machine.GetStatesCount())
                Array.Resize(ref _stateNames, _machine.GetStatesCount());

            for (int i = 0; i < _stateNames.Length; i++)
                _stateNames[i] = _machine.GetStateInfo(i).Info;

            GUILayout.BeginHorizontal("box");
            GUILayout.Label("All states:", EditorStyles.miniBoldLabel);
            int selected = EditorGUILayout.Popup(_selectedStateIndex, _stateNames);
            int? currentStateIndex = _machine.GetCurrentStateIndex();

            if (_displayCurrent == false)
                _selectedStateIndex = selected;

            GUILayout.EndHorizontal();
        }

        private void DrawInfo(GraphInfo info)
        {
            float columnWidth = (position.width - (4f * 5)) / 5f;
            int previousCount = _graphInfo.Previous?.Count ?? 0;
            int nextCount = _graphInfo.Next?.Count ?? 0;
            int rowCount = Mathf.Max(previousCount, nextCount, 1);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Previous", EditorStyles.miniBoldLabel, GUILayout.Width(columnWidth));
            GUILayout.Label("->", EditorStyles.miniBoldLabel, GUILayout.Width(columnWidth));
            GUILayout.Label(_displayCurrent ? "Current" : "Selected", EditorStyles.miniBoldLabel, GUILayout.Width(columnWidth));
            GUILayout.Label("->", EditorStyles.miniBoldLabel, GUILayout.Width(columnWidth));
            GUILayout.Label("Next", EditorStyles.miniBoldLabel, GUILayout.Width(columnWidth));
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < rowCount; i++)
            {
                // Тексты для столбцов

                bool previousExists = i < previousCount;
                StateInfo? previous = previousExists ? _graphInfo.Previous[i].State : null;
                GUIStyle previousStyle = ComputeStyle(info, previous);

                bool currentExists = i == 0;
                StateInfo? current = currentExists ? _graphInfo.Selected : null;
                GUIStyle currentStyle = ComputeStyle(info, current);

                bool nextExists = i < nextCount;
                StateInfo? next = nextExists ? _graphInfo.Next[i].State : null;
                GUIStyle nextStyle = ComputeStyle(info, next);

                string col1Text = previous.GetValueOrDefault().Info;
                string col2Text = previousExists ? _graphInfo.Previous[i].Condition : string.Empty;
                string col3Text = current.GetValueOrDefault().Info;
                string col4Text = nextExists ? _graphInfo.Next[i].Condition : string.Empty;
                string col5Text = next.GetValueOrDefault().Info;

                float h1 = _default.CalcHeight(new GUIContent(col1Text), columnWidth);
                float h2 = _default.CalcHeight(new GUIContent(col2Text), columnWidth);
                float h3 = _default.CalcHeight(new GUIContent(col3Text), columnWidth);
                float h4 = _default.CalcHeight(new GUIContent(col4Text), columnWidth);
                float h5 = _default.CalcHeight(new GUIContent(col5Text), columnWidth);
                float rowHeight = Mathf.Max(h1, h2, h3, h4, h5, EditorGUIUtility.singleLineHeight);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(col1Text, previousStyle, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight)))
                    SelectState(previous);

                GUILayout.Label(col2Text, previousExists ? _default : _empty, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight));
                GUILayout.Label(col3Text, currentStyle, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight));
                GUILayout.Label(col4Text, nextExists ? _default : _empty, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight));

                if (GUILayout.Button(col5Text, nextStyle, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight)))
                    SelectState(next);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);
            }
        }

        private void SelectState(StateInfo? state)
        {
            if (state == null)
                return;

            _displayCurrent = false;
            _selectedStateIndex = state.Value.Index;
        }

        private void Init(StateMachine machine, string name)
        {
            _machine = machine;
            _name = name;
            _selectedStateIndex = 0;
            _graphInfo = GraphInfo.Empty;
        }

        private void Clear()
        {
            _machine = null;
            _name = null;
            _graphInfo = default;
        }

        private GUIStyle ComputeStyle(GraphInfo info, StateInfo? state)
        {
            if (state.HasValue == false)
                return _empty;

            if (info.CurrentStateIndex.HasValue && state.Value.Index == info.CurrentStateIndex.Value)
                return _active;

            if (info.PreviousStateIndex.HasValue && state.Value.Index == info.PreviousStateIndex.Value)
                return _previous;

            return _default;
        }

        private void InitStyles()
        {
            CreateStyle(ref _default, new Color(0.298f, 0.309f, 0.321f), new Color(0.270f, 0.282f, 0.290f));
            CreateStyle(ref _active, new Color(0.137f, 0.470f, 0.223f), new Color(0f, 0.376f, 0.145f));
            CreateStyle(ref _previous, new Color(0.341f, 0.596f, 0.529f), new Color(0.231f, 0.427f, 0.4f));
            CreateStyle(ref _empty, Color.clear, Color.clear);
        }

        private void CreateStyle(ref GUIStyle style, Color color, Color edgeColor)
        {
            if (style == null)
                style = new GUIStyle(GUI.skin.button);

            style.wordWrap = true;
            style.alignment = TextAnchor.MiddleCenter;
            style.padding = new RectOffset(4, 4, 4, 4);

            Color delta = new Color(0.1f, 0.1f, 0.1f, 0f);

            style.normal.background = CreateColoredRoundedTexture(color, edgeColor);
            style.hover.background = CreateColoredRoundedTexture(color - delta, edgeColor - delta);
            style.active.background = CreateColoredRoundedTexture(color - (delta * 2f), edgeColor - (delta * 2f));

            style.border = style.normal.background.ComputeRoundOffset();
        }

        private Texture2D CreateColoredRoundedTexture(Color color, Color edgeColor, int resolution = 16)
        {
            if (_textures.TryGetValue(color, out Texture2D texture) && texture != null)
                return texture;

            texture = EditorProceduralUtility.CreateButtonRoundTexture(color, edgeColor, resolution);
            _textures[color] = texture;
            return texture;
        }
    }
}