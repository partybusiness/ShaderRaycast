using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate360Strips : MonoBehaviour
{

    [SerializeField]
    Vector2 velocity;

    [SerializeField]
    Vector2 position;

    [SerializeField]
    Material carMaterial;

    [SerializeField]
    Texture2D map;

    ComputeBuffer distanceBuffer;

    [SerializeField]
    ComputeShader findDistances;

    Vector4[] distances = new Vector4[512];

    void Start()
    {
        
    }

    private void OnEnable()
    {
        distanceBuffer = new ComputeBuffer(distances.Length, 4 * sizeof(float));
        findDistances.SetBuffer(0, "distances", distanceBuffer);
        findDistances.SetTexture(0, "map", map);
        findDistances.SetFloat("mapSize", map.width);
        Shader.SetGlobalVector("forward", new Vector2(0,1f));
        Shader.SetGlobalFloat("fov", 5f);
        
        //renderMaterial.SetTexture("_MapTex", map);
        //renderMaterial.SetFloat("_screenHeight", screenHeight);
    }

    private void OnDisable()
    {
        distanceBuffer.Release();
        distanceBuffer = null;
    }

    void Update()
    {
        position += velocity * Time.deltaTime;
        Shader.SetGlobalVector("position", position);
        findDistances.Dispatch(0, distances.Length, 1, 1);
        distanceBuffer.GetData(distances);
        carMaterial.SetVectorArray("stripDistances", distances);
        //Shader.SetGlobalVectorArray("stripDistances", distances);


    }
}
