﻿//
// ShaderGraphEssentials for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System;
using UnityEditor;
using UnityEditor.ShaderGraph;

namespace ShaderGraphEssentials
{
    static class CreateSGESimpleLitShaderGraph
    {
        [MenuItem("Assets/Create/Shader/ShaderGraphEssentials/SGE SimpleLit Shader Graph", false, 302)]
        public static void CreateSGESimpleLitGraph()
        {
            var target = (SGEUniversalTarget)Activator.CreateInstance(typeof(SGEUniversalTarget));
            target.TrySetActiveSubTarget(typeof(SGEUniversalSimpleLitSubTarget));

            var blockDescriptors = new [] 
            { 
                BlockFields.VertexDescription.Position,
                BlockFields.VertexDescription.Normal,
                BlockFields.VertexDescription.Tangent,
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.NormalTS,
                BlockFields.SurfaceDescription.Specular,
                BlockFields.SurfaceDescription.Smoothness,
                BlockFields.SurfaceDescription.Emission,
            };

            GraphUtil.CreateNewGraphWithOutputs(new [] {target}, blockDescriptors);
        }
    }
}
