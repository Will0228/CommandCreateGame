using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Animations;
using UnityEngine;

namespace Presentation.DemoViewTest
{
    internal sealed class DemoViewBaseEditorParameterStore : IDisposable
    {
        private readonly List<DemoViewBaseEditorParameterDefinition.IGuiParameter> _commonParameters = new ();
        public IReadOnlyList<DemoViewBaseEditorParameterDefinition.IGuiParameter> CommonParameters => _commonParameters;
        
        private readonly List<DemoViewBaseEditorParameterDefinition.IGuiParameter> _animatorParameters = new ();
        public IReadOnlyList<DemoViewBaseEditorParameterDefinition.IGuiParameter> AnimatorParameters => _animatorParameters;

        internal void SetupCommonParameters(ParameterInfo[]  parameters)
        {
            foreach (var parameter in parameters)
            {
                DemoViewBaseEditorParameterDefinition.IGuiParameter param = null;
                if(parameter.ParameterType == typeof(int))
                {
                    param = new DemoViewBaseEditorParameterDefinition.IntParameter();
                    param.Initialize(parameter.Name, "0");
                }
                else if(parameter.ParameterType == typeof(Sprite))
                {
                    param = new DemoViewBaseEditorParameterDefinition.SpriteParameter();
                    param.Initialize(parameter.Name, null);
                }
                else if(parameter.ParameterType == typeof(AudioClip))
                {
                    param = new DemoViewBaseEditorParameterDefinition.AudioParameter();
                    param.Initialize(parameter.Name, null);
                }
                _commonParameters.Add(param);
            }
        }

        internal void SetupAnimatorParameters(ParameterInfo[] parameters)
        {
            foreach (var parameter in parameters)
            {
                DemoViewBaseEditorParameterDefinition.IGuiParameter param = null;
                if(parameter.ParameterType == typeof(AnimatorController))
                {
                    param = new DemoViewBaseEditorParameterDefinition.AnimatorParameter();
                    param.Initialize(parameter.Name, string.Empty);
                }
                _animatorParameters.Add(param);
            }
        }

        public void Dispose()
        {
            _commonParameters.Clear();
        }
    }
}