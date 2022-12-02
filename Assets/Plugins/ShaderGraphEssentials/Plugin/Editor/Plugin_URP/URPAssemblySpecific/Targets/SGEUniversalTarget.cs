//
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
using System.Collections.Generic;
using System.Linq;
using ShaderGraphEssentials.Legacy;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Legacy;
using UnityEditor.ShaderGraph.Serialization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using CullMode = UnityEngine.Rendering.CullMode;
using RenderQueue = UnityEditor.ShaderGraph.RenderQueue;

namespace ShaderGraphEssentials
{
    enum BlendMode
    {
        Off = 0,
        Alpha = 1,
        Premultiply = 2,
        Additive = 3,
        Multiply = 4
    }

    sealed class SGEUniversalTarget : Target, ILegacyTarget
    {
        // Constants
        static readonly GUID kSourceCodeGuid = new GUID("f9a02d7a49622a44bb0e1e7f11ade6bd"); // SGEUniversalTarget.cs
        public const string kPipelineTag = "UniversalPipeline";

        // SubTarget
        List<SubTarget> m_SubTargets;
        List<string> m_SubTargetNames;
        int activeSubTargetIndex => m_SubTargets.IndexOf(m_ActiveSubTarget);

        // View
        PopupField<string> m_SubTargetField;
        TextField m_CustomGUIField;

        [SerializeField]
        JsonData<SubTarget> m_ActiveSubTarget;

        [SerializeField] 
        private RenderType m_RenderType = RenderType.Opaque;

        [SerializeField] 
        private RenderQueue m_RenderQueue = RenderQueue.Geometry;

        [SerializeField]
        private BlendMode m_BlendMode = BlendMode.Off;
        
        [SerializeField]
        private bool m_AlphaClip = false;

        [SerializeField]
        private CullMode m_CullMode = CullMode.Back;

        [SerializeField] 
        private ZWrite m_ZWrite = ZWrite.On;

        [SerializeField] 
        private ZTest m_ZTest = ZTest.Less;
        
        [SerializeField]
        string m_CustomEditorGUI;

        public SGEUniversalTarget()
        {
            displayName = "SGE Universal";
            m_SubTargets = TargetUtils.GetSubTargets(this);
            m_SubTargetNames = m_SubTargets.Select(x => x.displayName).ToList();
            TargetUtils.ProcessSubTargetList(ref m_ActiveSubTarget, ref m_SubTargets);
        }

        public SubTarget activeSubTarget
        {
            get => m_ActiveSubTarget;
            set => m_ActiveSubTarget = value;
        }

        public RenderType renderType
        {
            get => m_RenderType;
            set => m_RenderType = value;
        }
        
        public RenderQueue renderQueue
        {
            get => m_RenderQueue;
            set => m_RenderQueue = value;
        }

        public BlendMode blendMode
        {
            get => m_BlendMode;
            set => m_BlendMode = value;
        }
        
        public bool alphaClip
        {
            get => m_AlphaClip;
            set => m_AlphaClip = value;
        }

        public CullMode cullMode
        {
            get => m_CullMode;
            set => m_CullMode = value;
        }
        
        public ZWrite zWrite
        {
            get => m_ZWrite;
            set => m_ZWrite = value;
        }
        
        public ZTest zTest
        {
            get => m_ZTest;
            set => m_ZTest = value;
        }

        public string customEditorGUI
        {
            get => m_CustomEditorGUI;
            set => m_CustomEditorGUI = value;
        }

        public override bool IsActive()
        {
            bool isUniversalRenderPipeline = GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset;
            return isUniversalRenderPipeline && activeSubTarget.IsActive();
        }

        public override bool IsNodeAllowedByTarget(Type nodeType)
        {
            SRPFilterAttribute srpFilter = NodeClassCache.GetAttributeOnNodeType<SRPFilterAttribute>(nodeType);
            bool worksWithThisSrp = srpFilter == null || srpFilter.srpTypes.Contains(typeof(UniversalRenderPipeline));
            return worksWithThisSrp && base.IsNodeAllowedByTarget(nodeType);
        }

        public override void Setup(ref TargetSetupContext context)
        {
            // Setup the Target
            context.AddAssetDependency(kSourceCodeGuid, AssetCollection.Flags.SourceDependency);

            // Setup the active SubTarget
            TargetUtils.ProcessSubTargetList(ref m_ActiveSubTarget, ref m_SubTargets);
            m_ActiveSubTarget.value.target = this;
            m_ActiveSubTarget.value.Setup(ref context);

            // Override EditorGUI
            if(!string.IsNullOrEmpty(m_CustomEditorGUI))
            {
                context.SetDefaultShaderGUI(m_CustomEditorGUI);
            }
        }

        public override void OnAfterMultiDeserialize(string json)
        {
            TargetUtils.ProcessSubTargetList(ref m_ActiveSubTarget, ref m_SubTargets);
            m_ActiveSubTarget.value.target = this;
        }

        public override void GetFields(ref TargetFieldContext context)
        {
            var descs = context.blocks.Select(x => x.descriptor);
            // Core fields
            context.AddField(Fields.GraphVertex,            descs.Contains(BlockFields.VertexDescription.Position) ||
                                                            descs.Contains(BlockFields.VertexDescription.Normal) ||
                                                            descs.Contains(BlockFields.VertexDescription.Tangent));
            context.AddField(Fields.GraphPixel);
            
            context.AddField(SGEFields.BlendModeOff, blendMode == BlendMode.Off);
            context.AddField(SGEFields.BlendModeAdditive, blendMode == BlendMode.Additive);
            context.AddField(SGEFields.BlendModeAlpha, blendMode == BlendMode.Alpha);
            context.AddField(SGEFields.BlendModeMultiply, blendMode == BlendMode.Multiply);
            context.AddField(SGEFields.BlendModePremultiply, blendMode == BlendMode.Premultiply);
            
            context.AddField(Fields.AlphaClip, alphaClip);

            context.AddField(SGEFields.CullModeOff, cullMode == CullMode.Off);
            context.AddField(SGEFields.CullModeFront, cullMode == CullMode.Front);
            context.AddField(SGEFields.CullModeBack, cullMode == CullMode.Back);
            
            context.AddField(SGEFields.ZWrite, zWrite == ZWrite.On);

            context.AddField(SGEFields.ZTestLess, zTest == ZTest.Less);
            context.AddField(SGEFields.ZTestGreater, zTest == ZTest.Greater);
            context.AddField(SGEFields.ZTestLEqual, zTest == ZTest.LEqual);
            context.AddField(SGEFields.ZTestGEqual, zTest == ZTest.GEqual);
            context.AddField(SGEFields.ZTestEqual, zTest == ZTest.Equal);
            context.AddField(SGEFields.ZTestNotEqual, zTest == ZTest.NotEqual);
            context.AddField(SGEFields.ZTestAlways, zTest == ZTest.Always);

            bool opaque = blendMode == BlendMode.Off;
            context.AddField(UniversalFields.SurfaceOpaque, opaque);
            context.AddField(UniversalFields.SurfaceTransparent,  !opaque);

            // SubTarget fields
            m_ActiveSubTarget.value.GetFields(ref context);
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context)
        {
            // Core blocks
            context.AddBlock(BlockFields.VertexDescription.Position);
            context.AddBlock(BlockFields.VertexDescription.Normal);
            context.AddBlock(BlockFields.VertexDescription.Tangent);
            context.AddBlock(BlockFields.SurfaceDescription.BaseColor);
            context.AddBlock(BlockFields.SurfaceDescription.Alpha, alphaClip || blendMode != BlendMode.Off);
            context.AddBlock(BlockFields.SurfaceDescription.AlphaClipThreshold, alphaClip);

            // SubTarget blocks
            m_ActiveSubTarget.value.GetActiveBlocks(ref context);
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            base.CollectShaderProperties(collector, generationMode);

            collector.AddShaderProperty(LightmappingShaderProperties.kLightmapsArray);
            collector.AddShaderProperty(LightmappingShaderProperties.kLightmapsIndirectionArray);
            collector.AddShaderProperty(LightmappingShaderProperties.kShadowMasksArray);
        }

        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<String> registerUndo)
        {
            // Core properties
            m_SubTargetField = new PopupField<string>(m_SubTargetNames, activeSubTargetIndex);
            context.AddProperty("Material", m_SubTargetField, (evt) =>
            {
                if (Equals(activeSubTargetIndex, m_SubTargetField.index))
                    return;

                registerUndo("Change Material");
                m_ActiveSubTarget = m_SubTargets[m_SubTargetField.index];
                onChange();
            });

            // SubTarget properties
            m_ActiveSubTarget.value.GetPropertiesGUI(ref context, onChange, registerUndo);
            
            context.AddProperty("Render Type", new EnumField(RenderType.Opaque) { value = renderType }, (evt) =>
            {
                if (Equals(renderType, evt.newValue))
                    return;

                registerUndo("Change Render Type");
                renderType = (RenderType) evt.newValue;
                onChange();
            });
            
            context.AddProperty("Render Queue", new EnumField(RenderQueue.Background) { value = renderQueue }, (evt) =>
            {
                if (Equals(renderQueue, evt.newValue))
                    return;

                registerUndo("Change Render Queue");
                renderQueue = (RenderQueue) evt.newValue;
                onChange();
            });
            
            context.AddProperty("Blend", new EnumField(BlendMode.Off) { value = blendMode }, (evt) =>
            {
                if (Equals(blendMode, evt.newValue))
                    return;

                registerUndo("Change Blend");
                blendMode = (BlendMode) evt.newValue;
                onChange();
            });
            
            context.AddProperty("Alpha Clip", new Toggle() { value = alphaClip }, (evt) =>
            {
                if (Equals(alphaClip, evt.newValue))
                    return;

                registerUndo("Change Alpha Clip");
                alphaClip = evt.newValue;
                onChange();
            });
            
            context.AddProperty("CullMode", new EnumField(CullMode.Front) { value = cullMode}, (evt) =>
            {
                if (Equals(cullMode, evt.newValue))
                    return;

                registerUndo("Change ZWrite");
                cullMode = (CullMode) evt.newValue;
                onChange();
            });

            context.AddProperty("ZWrite", new EnumField(ZWrite.On) { value = zWrite}, (evt) =>
            {
                if (Equals(zWrite, evt.newValue))
                    return;

                registerUndo("Change ZWrite");
                zWrite = (ZWrite) evt.newValue;
                onChange();
            });
            
            context.AddProperty("ZTest", new EnumField(ZTest.Less) { value = zTest}, (evt) =>
            {
                if (Equals(zTest, evt.newValue))
                    return;

                registerUndo("Change ZTest");
                zTest = (ZTest) evt.newValue;
                onChange();
            });

            // Custom Editor GUI
            // Requires FocusOutEvent
            m_CustomGUIField = new TextField("") { value = customEditorGUI };
            m_CustomGUIField.RegisterCallback<FocusOutEvent>(s =>
            {
                if (Equals(customEditorGUI, m_CustomGUIField.value))
                    return;

                registerUndo("Change Custom Editor GUI");
                customEditorGUI = m_CustomGUIField.value;
                onChange();
            });
            context.AddProperty("Custom Editor GUI", m_CustomGUIField, (evt) => {});
        }

        public bool TrySetActiveSubTarget(Type subTargetType)
        {
            if(!subTargetType.IsSubclassOf(typeof(SubTarget)))
                return false;

            foreach(var subTarget in m_SubTargets)
            {
                if(subTarget.GetType().Equals(subTargetType))
                {
                    m_ActiveSubTarget = subTarget;
                    return true;
                }
            }

            return false;
        }

        private static CullMode UpgradeCullMode(ShaderGraphEssentials.Legacy.CullMode oldCullMode)
        {
            switch (oldCullMode)
            {
                case Legacy.CullMode.Back:
                    return CullMode.Back;
                case Legacy.CullMode.Front:
                    return CullMode.Front;
                case Legacy.CullMode.Off:
                    return CullMode.Off;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldCullMode), oldCullMode, null);
            }
        }

        public bool TryUpgradeFromMasterNode(IMasterNode1 masterNode, out Dictionary<BlockFieldDescriptor, int> blockMap)
        {
            void UpgradeAlphaClip(int clipId)
            {
                var clipThresholdId = clipId;
                var node = masterNode as AbstractMaterialNode;
                var clipThresholdSlot = node.FindSlot<Vector1MaterialSlot>(clipThresholdId);
                if(clipThresholdSlot == null)
                    return;

                clipThresholdSlot.owner = node;
                if(clipThresholdSlot.isConnected || clipThresholdSlot.value > 0.0f)
                {
                    m_AlphaClip = true;
                }
            }
            
            // Upgrade Target
            switch(masterNode)
            {
                case SGEUnlitMasterNode1 unlitMasterNode:
                    m_RenderType = unlitMasterNode.m_renderType;
                    m_RenderQueue = unlitMasterNode.m_renderQueue;
                    m_BlendMode = (BlendMode) unlitMasterNode.m_blendMode;
                    m_CullMode = UpgradeCullMode(unlitMasterNode.m_cullMode);
                    m_ZWrite = unlitMasterNode.m_zwrite;
                    m_ZTest = unlitMasterNode.m_ztest;
                    m_CustomEditorGUI = unlitMasterNode.m_customEditor;
                    UpgradeAlphaClip(8);
                    break;
                case SGESimpleLitMasterNode1 simpleLitMasterNode:
                    m_RenderType = simpleLitMasterNode.m_renderType;
                    m_RenderQueue = simpleLitMasterNode.m_renderQueue;
                    m_BlendMode = (BlendMode) simpleLitMasterNode.m_blendMode;
                    m_CullMode = UpgradeCullMode(simpleLitMasterNode.m_cullMode);
                    m_ZWrite = simpleLitMasterNode.m_zwrite;
                    m_ZTest = simpleLitMasterNode.m_ztest;
                    m_CustomEditorGUI = simpleLitMasterNode.m_customEditor;
                    UpgradeAlphaClip(7);
                    break;
                case SGECustomLitMasterNode1 customLitMasterNode:
                    m_RenderType = customLitMasterNode.m_renderType;
                    m_RenderQueue = customLitMasterNode.m_renderQueue;
                    m_BlendMode = (BlendMode) customLitMasterNode.m_blendMode;
                    m_CullMode = UpgradeCullMode(customLitMasterNode.m_cullMode);
                    m_ZWrite = customLitMasterNode.m_zwrite;
                    m_ZTest = customLitMasterNode.m_ztest;
                    m_CustomEditorGUI = customLitMasterNode.m_customEditor;
                    UpgradeAlphaClip(7);
                    break;
            }

            // Upgrade SubTarget
            foreach(var subTarget in m_SubTargets)
            {
                if(!(subTarget is ILegacyTarget legacySubTarget))
                    continue;

                if(legacySubTarget.TryUpgradeFromMasterNode(masterNode, out blockMap))
                {
                    m_ActiveSubTarget = subTarget;
                    return true;
                }
            }

            blockMap = null;
            return false;
        }

        public override bool WorksWithSRP(RenderPipelineAsset scriptableRenderPipeline)
        {
            return scriptableRenderPipeline?.GetType() == typeof(UniversalRenderPipelineAsset);
        }
    }

#region Passes
    static class CorePasses
    {
        public static readonly PassDescriptor DepthOnly = new PassDescriptor()
        {
            // Definition
            displayName = "DepthOnly",
            referenceName = "SHADERPASS_DEPTHONLY",
            lightMode = "DepthOnly",
            useInPreview = true,

            // Template
            passTemplatePath = GenerationUtils.GetDefaultTemplatePath("PassMesh.template"),
            sharedTemplateDirectories = GenerationUtils.GetDefaultSharedTemplateDirectories(),

            // Port Mask
            validVertexBlocks = CoreBlockMasks.Vertex,
            validPixelBlocks = CoreBlockMasks.FragmentAlphaOnly,

            // Fields
            structs = CoreStructCollections.Default,
            fieldDependencies = CoreFieldDependencies.Default,

            // Conditional State
            renderStates = CoreRenderStates.DepthOnly,
            pragmas = CorePragmas.Instanced,
            includes = CoreIncludes.DepthOnly,
        };

        public static readonly PassDescriptor ShadowCaster = new PassDescriptor()
        {
            // Definition
            displayName = "ShadowCaster",
            referenceName = "SHADERPASS_SHADOWCASTER",
            lightMode = "ShadowCaster",

            // Template
            passTemplatePath = GenerationUtils.GetDefaultTemplatePath("PassMesh.template"),
            sharedTemplateDirectories = GenerationUtils.GetDefaultSharedTemplateDirectories(),

            // Port Mask
            validVertexBlocks = CoreBlockMasks.Vertex,
            validPixelBlocks = CoreBlockMasks.FragmentAlphaOnly,

            // Fields
            structs = CoreStructCollections.Default,
            requiredFields = CoreRequiredFields.ShadowCaster,
            fieldDependencies = CoreFieldDependencies.Default,

            // Conditional State
            renderStates = CoreRenderStates.ShadowCaster,
            pragmas = CorePragmas.Instanced,
            includes = CoreIncludes.ShadowCaster,
        };
    }
#endregion

#region PortMasks
    class CoreBlockMasks
    {
        public static readonly BlockFieldDescriptor[] Vertex = new BlockFieldDescriptor[]
        {
            BlockFields.VertexDescription.Position,
            BlockFields.VertexDescription.Normal,
            BlockFields.VertexDescription.Tangent,
        };

        public static readonly BlockFieldDescriptor[] FragmentAlphaOnly = new BlockFieldDescriptor[]
        {
            BlockFields.SurfaceDescription.Alpha,
            BlockFields.SurfaceDescription.AlphaClipThreshold,
        };

        public static readonly BlockFieldDescriptor[] FragmentColorAlpha = new BlockFieldDescriptor[]
        {
            BlockFields.SurfaceDescription.BaseColor,
            BlockFields.SurfaceDescription.Alpha,
            BlockFields.SurfaceDescription.AlphaClipThreshold,
        };
    }
#endregion

#region StructCollections
    static class CoreStructCollections
    {
        public static readonly StructCollection Default = new StructCollection
        {
            { Structs.Attributes },
            { UniversalStructs.Varyings },
            { Structs.SurfaceDescriptionInputs },
            { Structs.VertexDescriptionInputs },
        };
    }
#endregion

#region RequiredFields
    static class CoreRequiredFields
    {
        public static readonly FieldCollection ShadowCaster = new FieldCollection()
        {
            StructFields.Attributes.normalOS,
        };
    }
#endregion

#region FieldDependencies
    static class CoreFieldDependencies
    {
        public static readonly DependencyCollection Default = new DependencyCollection()
        {
            { FieldDependencies.Default },
            new FieldDependency(UniversalStructFields.Varyings.stereoTargetEyeIndexAsRTArrayIdx,    StructFields.Attributes.instanceID ),
            new FieldDependency(UniversalStructFields.Varyings.stereoTargetEyeIndexAsBlendIdx0,     StructFields.Attributes.instanceID ),
        };
    }
#endregion

#region RenderStates
    static class CoreRenderStates
    {
        public static readonly RenderStateCollection Default = new RenderStateCollection
        {
            // TODO render type
            // TODO render queue
            { RenderState.Blend(Blend.One, Blend.Zero), new FieldCondition(SGEFields.BlendModeOff, true) },
            { RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(SGEFields.BlendModeAlpha, true) },
            { RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(SGEFields.BlendModePremultiply, true) },
            { RenderState.Blend(Blend.One, Blend.One, Blend.One, Blend.One), new FieldCondition(SGEFields.BlendModeAdditive, true) },
            { RenderState.Blend(Blend.DstColor, Blend.Zero), new FieldCondition(SGEFields.BlendModeMultiply, true) },
            
            { RenderState.Cull(Cull.Off), new FieldCondition(SGEFields.CullModeOff, true) },
            { RenderState.Cull(Cull.Front), new FieldCondition(SGEFields.CullModeFront, true) },
            { RenderState.Cull(Cull.Back), new FieldCondition(SGEFields.CullModeBack, true) },
            
            { RenderState.ZWrite(ZWrite.On), new FieldCondition(SGEFields.ZWrite, true) },
            { RenderState.ZWrite(ZWrite.Off), new FieldCondition(SGEFields.ZWrite, false) },
            
            { RenderState.ZTest(ZTest.Less), new FieldCondition(SGEFields.ZTestLess, true) },
            { RenderState.ZTest(ZTest.Greater), new FieldCondition(SGEFields.ZTestGreater, true) },
            { RenderState.ZTest(ZTest.LEqual), new FieldCondition(SGEFields.ZTestLEqual, true) },
            { RenderState.ZTest(ZTest.GEqual), new FieldCondition(SGEFields.ZTestGEqual, true) },
            { RenderState.ZTest(ZTest.Equal), new FieldCondition(SGEFields.ZTestEqual, true) },
            { RenderState.ZTest(ZTest.NotEqual), new FieldCondition(SGEFields.ZTestNotEqual, true) },
            { RenderState.ZTest(ZTest.Always), new FieldCondition(SGEFields.ZTestAlways, true) },
        };

        public static readonly RenderStateCollection Meta = new RenderStateCollection
        {
            { RenderState.Cull(Cull.Off) },
        };

        public static readonly RenderStateCollection ShadowCaster = new RenderStateCollection
        {
            { RenderState.ZTest(ZTest.LEqual) },
            { RenderState.ZWrite(ZWrite.On) },
            { RenderState.Cull(Cull.Back), new FieldCondition(Fields.DoubleSided, false) },
            { RenderState.Cull(Cull.Off), new FieldCondition(Fields.DoubleSided, true) },
            { RenderState.ColorMask("ColorMask 0") },
            { RenderState.Blend(Blend.One, Blend.Zero), new FieldCondition(UniversalFields.SurfaceOpaque, true) },
            { RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(Fields.BlendAlpha, true) },
            { RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(UniversalFields.BlendPremultiply, true) },
            { RenderState.Blend(Blend.One, Blend.One, Blend.One, Blend.One), new FieldCondition(UniversalFields.BlendAdd, true) },
            { RenderState.Blend(Blend.DstColor, Blend.Zero), new FieldCondition(UniversalFields.BlendMultiply, true) },
        };

        public static readonly RenderStateCollection DepthOnly = new RenderStateCollection
        {
            { RenderState.ZTest(ZTest.LEqual) },
            { RenderState.ZWrite(ZWrite.On) },
            { RenderState.Cull(Cull.Back), new FieldCondition(Fields.DoubleSided, false) },
            { RenderState.Cull(Cull.Off), new FieldCondition(Fields.DoubleSided, true) },
            { RenderState.ColorMask("ColorMask 0") },
            { RenderState.Blend(Blend.One, Blend.Zero), new FieldCondition(UniversalFields.SurfaceOpaque, true) },
            { RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(Fields.BlendAlpha, true) },
            { RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(UniversalFields.BlendPremultiply, true) },
            { RenderState.Blend(Blend.One, Blend.One, Blend.One, Blend.One), new FieldCondition(UniversalFields.BlendAdd, true) },
            { RenderState.Blend(Blend.DstColor, Blend.Zero), new FieldCondition(UniversalFields.BlendMultiply, true) },
        };

        public static readonly RenderStateCollection DepthNormalsOnly = new RenderStateCollection
        {
            { RenderState.ZTest(ZTest.LEqual) },
            { RenderState.ZWrite(ZWrite.On) },
            { RenderState.Cull(Cull.Back), new FieldCondition(Fields.DoubleSided, false) },
            { RenderState.Cull(Cull.Off), new FieldCondition(Fields.DoubleSided, true) },
            { RenderState.Blend(Blend.One, Blend.Zero), new FieldCondition(UniversalFields.SurfaceOpaque, true) },
            { RenderState.Blend(Blend.SrcAlpha, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(Fields.BlendAlpha, true) },
            { RenderState.Blend(Blend.One, Blend.OneMinusSrcAlpha, Blend.One, Blend.OneMinusSrcAlpha), new FieldCondition(UniversalFields.BlendPremultiply, true) },
            { RenderState.Blend(Blend.One, Blend.One, Blend.One, Blend.One), new FieldCondition(UniversalFields.BlendAdd, true) },
            { RenderState.Blend(Blend.DstColor, Blend.Zero), new FieldCondition(UniversalFields.BlendMultiply, true) },
        };
    }
#endregion

#region Pragmas
    // TODO: should these be renamed and moved to UniversalPragmas/UniversalPragmas.cs ?
    // TODO: these aren't "core" as HDRP doesn't use them
    // TODO: and the same for the rest "Core" things
    static class CorePragmas
    {
        public static readonly PragmaCollection Default = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target20) },
            { Pragma.OnlyRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };

        public static readonly PragmaCollection Instanced = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target20) },
            { Pragma.OnlyRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
            { Pragma.MultiCompileInstancing },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };

        public static readonly PragmaCollection Forward = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target20) },
            { Pragma.OnlyRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
            { Pragma.MultiCompileInstancing },
            { Pragma.MultiCompileFog },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };

        public static readonly PragmaCollection _2DDefault = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target20) },
            { Pragma.ExcludeRenderers(new[]{ Platform.D3D9 }) },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };

        public static readonly PragmaCollection DOTSDefault = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target45) },
            { Pragma.ExcludeRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };

        public static readonly PragmaCollection DOTSInstanced = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target45) },
            { Pragma.ExcludeRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
            { Pragma.MultiCompileInstancing },
            { Pragma.DOTSInstancing },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };

        public static readonly PragmaCollection DOTSForward = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target45) },
            { Pragma.ExcludeRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
            { Pragma.MultiCompileInstancing },
            { Pragma.MultiCompileFog },
            { Pragma.DOTSInstancing },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };

        public static readonly PragmaCollection DOTSGBuffer = new PragmaCollection
        {
            { Pragma.Target(ShaderModel.Target45) },
            { Pragma.ExcludeRenderers(new[]{ Platform.GLES, Platform.GLES3, Platform.GLCore }) },
            { Pragma.MultiCompileInstancing },
            { Pragma.MultiCompileFog },
            { Pragma.DOTSInstancing },
            { Pragma.Vertex("vert") },
            { Pragma.Fragment("frag") },
        };
    }
#endregion

#region Includes
    static class CoreIncludes
    {
        const string kColor = "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl";
        const string kTexture = "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl";
        const string kCore = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl";
        const string kLighting = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl";
        const string kGraphFunctions = "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl";
        const string kVaryings = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl";
        const string kShaderPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl";
        const string kDepthOnlyPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl";
        internal const string kDepthNormalsOnlyPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl";
        const string kShadowCasterPass = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl";
        const string kTextureStack = "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl";

        public static readonly IncludeCollection CorePregraph = new IncludeCollection
        {
            { kColor, IncludeLocation.Pregraph },
            { kTexture, IncludeLocation.Pregraph },
            { kCore, IncludeLocation.Pregraph },
            { kLighting, IncludeLocation.Pregraph },
            { kTextureStack, IncludeLocation.Pregraph },        // TODO: put this on a conditional
        };

        public static readonly IncludeCollection ShaderGraphPregraph = new IncludeCollection
        {
            { kGraphFunctions, IncludeLocation.Pregraph },
        };

        public static readonly IncludeCollection CorePostgraph = new IncludeCollection
        {
            { kShaderPass, IncludeLocation.Postgraph },
            { kVaryings, IncludeLocation.Postgraph },
        };

        public static readonly IncludeCollection DepthOnly = new IncludeCollection
        {
            // Pre-graph
            { CorePregraph },
            { ShaderGraphPregraph },

            // Post-graph
            { CorePostgraph },
            { kDepthOnlyPass, IncludeLocation.Postgraph },
        };

        public static readonly IncludeCollection DepthNormalsOnly = new IncludeCollection
        {
            // Pre-graph
            { CorePregraph },
            { ShaderGraphPregraph },

            // Post-graph
            { CorePostgraph },
            { kDepthNormalsOnlyPass, IncludeLocation.Postgraph },
        };

        public static readonly IncludeCollection ShadowCaster = new IncludeCollection
        {
            // Pre-graph
            { CorePregraph },
            { ShaderGraphPregraph },

            // Post-graph
            { CorePostgraph },
            { kShadowCasterPass, IncludeLocation.Postgraph },
        };
    }
#endregion

#region KeywordDescriptors
    // TODO: should these be renamed and moved to UniversalKeywordDescriptors/UniversalKeywords.cs ?
    // TODO: these aren't "core" as they aren't used by HDRP
    static class CoreKeywordDescriptors
    {
        public static readonly KeywordDescriptor Lightmap = new KeywordDescriptor()
        {
            displayName = "Lightmap",
            referenceName = "LIGHTMAP_ON",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor DirectionalLightmapCombined = new KeywordDescriptor()
        {
            displayName = "Directional Lightmap Combined",
            referenceName = "DIRLIGHTMAP_COMBINED",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor SampleGI = new KeywordDescriptor()
        {
            displayName = "Sample GI",
            referenceName = "_SAMPLE_GI",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor MainLightShadows = new KeywordDescriptor()
        {
            displayName = "Main Light Shadows",
            referenceName = "_MAIN_LIGHT_SHADOWS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor MainLightShadowsCascade = new KeywordDescriptor()
        {
            displayName = "Main Light Shadows Cascade",
            referenceName = "_MAIN_LIGHT_SHADOWS_CASCADE",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor AdditionalLights = new KeywordDescriptor()
        {
            displayName = "Additional Lights",
            referenceName = "_ADDITIONAL",
            type = KeywordType.Enum,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
            entries = new KeywordEntry[]
            {
                new KeywordEntry() { displayName = "Vertex", referenceName = "LIGHTS_VERTEX" },
                new KeywordEntry() { displayName = "Fragment", referenceName = "LIGHTS" },
                new KeywordEntry() { displayName = "Off", referenceName = "OFF" },
            }
        };

        public static readonly KeywordDescriptor AdditionalLightShadows = new KeywordDescriptor()
        {
            displayName = "Additional Light Shadows",
            referenceName = "_ADDITIONAL_LIGHT_SHADOWS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor ShadowsSoft = new KeywordDescriptor()
        {
            displayName = "Shadows Soft",
            referenceName = "_SHADOWS_SOFT",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor MixedLightingSubtractive = new KeywordDescriptor()
        {
            displayName = "Mixed Lighting Subtractive",
            referenceName = "_MIXED_LIGHTING_SUBTRACTIVE",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor LightmapShadowMixing = new KeywordDescriptor()
        {
            displayName = "Lightmap Shadow Mixing",
            referenceName = "LIGHTMAP_SHADOW_MIXING",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor ShadowsShadowmask = new KeywordDescriptor()
        {
            displayName = "Shadows Shadowmask",
            referenceName = "SHADOWS_SHADOWMASK",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor SmoothnessChannel = new KeywordDescriptor()
        {
            displayName = "Smoothness Channel",
            referenceName = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor ShapeLightType0 = new KeywordDescriptor()
        {
            displayName = "Shape Light Type 0",
            referenceName = "USE_SHAPE_LIGHT_TYPE_0",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor ShapeLightType1 = new KeywordDescriptor()
        {
            displayName = "Shape Light Type 1",
            referenceName = "USE_SHAPE_LIGHT_TYPE_1",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor ShapeLightType2 = new KeywordDescriptor()
        {
            displayName = "Shape Light Type 2",
            referenceName = "USE_SHAPE_LIGHT_TYPE_2",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor ShapeLightType3 = new KeywordDescriptor()
        {
            displayName = "Shape Light Type 3",
            referenceName = "USE_SHAPE_LIGHT_TYPE_3",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        public static readonly KeywordDescriptor UseLegacySpriteBlocks = new KeywordDescriptor()
        {
            displayName = "UseLegacySpriteBlocks",
            referenceName = "USELEGACYSPRITEBLOCKS",
            type = KeywordType.Boolean,
        };
    }
#endregion
}