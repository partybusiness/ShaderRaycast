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

    public float screenHeight = 0.7f;

    Vector4[] distances = new Vector4[512];

    MonsterRenderer monsterRenderer;

    private void Start()
    {
        findDistances.SetTexture(0, "map", map);
        findDistances.SetFloat("mapSize", map.width);
        Shader.SetGlobalFloat("fov", fov);
        renderMaterial.SetTexture("_MapTex", map);        
        renderMaterial.SetFloat("_screenHeight", screenHeight);
        GetComponent<Camera>().rect = new Rect(0, 1- screenHeight, 1, screenHeight);
        //create alternate camera for bottom UI?

        //     https://docs.unity3d.com/540/Documentation/ScriptReference/Shader.SetGlobalFloatArray.html
        monsterRenderer = GetComponent<MonsterRenderer>();
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
        Shader.SetGlobalVector("forward", forward);
        Shader.SetGlobalVector("position", position);
        findDistances.Dispatch(0, distances.Length, 1, 1);
        distanceBuffer.GetData(distances);

        
        Shader.SetGlobalVectorArray("stripDistances", distances);
        //renderMaterial.SetVectorArray("stripDistances", distances);
        Graphics.Blit(source, destination, renderMaterial);

        monsterRenderer.RenderMonsters(destination);
    }
}
