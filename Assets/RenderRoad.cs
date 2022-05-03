using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderRoad : MonoBehaviour
{
    [SerializeField]
    Material renderMaterial;

    [SerializeField]
    ComputeShader findDistances;

    [SerializeField]
    float speed = 8f;

    ComputeBuffer distanceBuffer;

    Vector4[] distances = new Vector4[256]; //distance, xoffset, texture uv, 

    void Start()
    {

    }

    void Update()
    {

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

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //CalcPos();
        //findDistances.SetVector("forward", forward);
        //findDistances.SetVector("position", position);
        findDistances.SetVector("_Time", Shader.GetGlobalVector("_Time"));
        findDistances.SetFloat("speed", speed);
        
        findDistances.Dispatch(0, distances.Length, 1, 1);
        distanceBuffer.GetData(distances);

        //renderMaterial.SetVector("forward", forward);
        //renderMaterial.SetVector("position", position);
        renderMaterial.SetFloat("skyAngle", 0.5f);
        
        //Debug.Log(distances[4] + ", " + distances[67]);
        renderMaterial.SetVectorArray("stripDistances", distances);
        Graphics.Blit(source, destination, renderMaterial);
    }
}
