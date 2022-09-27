using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
partial class CameraRenderer 
{
    // Start is called before the first frame update
    private ScriptableRenderContext _context;
    private Camera _camera;
    private CommandBuffer _commandBuffer;
    private const string bufferName = "Camera Render";
    private CullingResults _cullingResult;
    private static readonly List<ShaderTagId> drawingShaderTagIds = new List<ShaderTagId>{new ShaderTagId("SRPDefaultUnlit"),};
    private Lighting lighting = new Lighting();
    
    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _camera = camera;
        _context = context;
       
        if (!Cull(out var parameters))
        {
            return;
        }
        PrepareForSceneWindow();
        Settings(parameters);
        lighting.Setup(context, _cullingResult);
        DrawVisible();
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }

    private void Submit()
    {
        _commandBuffer.EndSample(bufferName);
        ExecuteCommandBuffer();
        _context.Submit();
    }



    private void DrawVisible()
    {
        var drawingSettings =
            CreateDrawingSettings(drawingShaderTagIds, SortingCriteria.CommonOpaque, out var sortingSettings);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

        _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);

        _context.DrawSkybox(_camera);
        
        drawingSettings.enableDynamicBatching = true;
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;

        _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);
    }


    private void Settings(ScriptableCullingParameters parameters)
    {
        _commandBuffer = new CommandBuffer
        { name = _camera.tag };
        _cullingResult = _context.Cull(ref parameters);
        _context.SetupCameraProperties(_camera);
        _commandBuffer.ClearRenderTarget(true, true, Color.clear);
        _commandBuffer.BeginSample(bufferName);
        ExecuteCommandBuffer();
    }
    private DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTags, SortingCriteria sortingCriteria, out SortingSettings sortingSettings)
    {
        sortingSettings = new SortingSettings(_camera)
        {
            criteria = sortingCriteria,
        };

        var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
        for (var i = 1; i < shaderTags.Count; i++)
        {
            drawingSettings.SetShaderPassName(i, shaderTags[i]);
        }

        return drawingSettings;
    }


    private bool Cull(out ScriptableCullingParameters parameters)
    {
        return _camera.TryGetCullingParameters(out parameters);
    }

    private void ExecuteCommandBuffer()
    {
        _context.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
    }


   

  

}

