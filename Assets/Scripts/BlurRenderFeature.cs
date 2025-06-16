using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderFeature : ScriptableRendererFeature
{
    class BlurPass : ScriptableRenderPass
    {
        private Material blurMaterial;
        private RTHandle source;
        private RTHandle tempRT;
        private Vector2 blurOffset;

        public BlurPass(Material mat, Vector2 offset)
        {
            blurMaterial = mat;
            blurOffset = offset;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(
                ref tempRT,
                descriptor,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_TempBlurTexture"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (blurMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Blur Pass");

            blurMaterial.SetVector("_BlurOffset", blurOffset);

            Blit(cmd, source, tempRT, blurMaterial);
            Blit(cmd, tempRT, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // Optional cleanup
        }
    }

    [System.Serializable]
    public class BlurSettings
    {
        public Material blurMaterial;
        public Vector2 blurDirection = new Vector2(1, 0);
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public BlurSettings settings = new BlurSettings();
    private BlurPass blurPass;

    public override void Create()
    {
        blurPass = new BlurPass(settings.blurMaterial, settings.blurDirection)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(blurPass); // NO need to access cameraColorTargetHandle here!
    }
}
