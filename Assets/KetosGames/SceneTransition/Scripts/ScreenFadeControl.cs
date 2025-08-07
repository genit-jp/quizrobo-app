using UnityEngine;
#if UNITY_PIPELINE_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#endif

public class ScreenFadeControl : MonoBehaviour
{
    public Material fadeMaterial = null;

    private Camera Camera;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    void OnPostRender()
    {
        if (fadeMaterial != null)
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(fadeMaterial.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
        }
    }

    /*
     * When using URP, we need to update from an endCameraRendering event and
     * we need to use the CommandBuffer instead of GL.
     * Full screen quad code from https://gist.github.com/phi-lira/46c98fc67640cda47dcd27e9b3765b85
     */
#if UNITY_PIPELINE_URP
    public void RenderFade(ScriptableRenderContext context)
    {
        if (fadeMaterial != null)
        {
            var cmd = CommandBufferPool.Get("DrawFullScreenPass");
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(UnityEngine.Rendering.Universal.RenderingUtils.fullscreenMesh, Matrix4x4.identity, fadeMaterial);
            cmd.SetViewProjectionMatrices(Camera.worldToCameraMatrix, Camera.projectionMatrix);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            context.Submit();
        }
    }
#endif


}
