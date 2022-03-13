using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderStrips : MonoBehaviour
{
    [SerializeField]
    Material renderMaterial;

    [SerializeField]
    ComputeShader findDistances;

    [SerializeField]
    Texture2D map;

    ComputeBuffer distanceBuffer;

    public Vector2 position;

    public Vector2 forward;

    public float fov = 1.2f;

    Vector4[] distances = new Vector4[512];

    private void Start()
    {
        findDistances.SetTexture(0, "map", map);
        findDistances.SetFloat("mapSize", map.width);
        findDistances.SetFloat("fov", fov);
        renderMaterial.SetFloat("fov", fov);

        //     https://docs.unity3d.com/540/Documentation/ScriptReference/Shader.SetGlobalFloatArray.html

    }

    private void OnEnable()
    {
        distanceBuffer = new ComputeBuffer(distances.Length, 4 * sizeof(float));
        findDistances.SetBuffer(0, "distances", distanceBuffer);

    }

    private void OnDisable()
    {
        distanceBuffer.Release();
        distanceBuffer = null;
    }

    private void CalcPos ()
    {
        position.x = transform.position.x;
        position.y = transform.position.z;

        forward.x = transform.forward.x;
        forward.y = transform.forward.z;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        CalcPos();
        findDistances.SetVector("forward", forward);
        findDistances.SetVector("position", position);
        findDistances.Dispatch(0, distances.Length, 1, 1);
        distanceBuffer.GetData(distances);

        renderMaterial.SetVector("forward", forward);
        renderMaterial.SetVector("position", position);
        renderMaterial.SetVectorArray("stripDistances", distances);
        Graphics.Blit(source, destination, renderMaterial);
    }
}
