using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generates distances and saves them into rows of a texture
public class Generate360Strips : MonoBehaviour
{
    public string fileName = "speedTexture";

    [SerializeField]
    Vector2 velocity;

    [SerializeField]
    Vector2 position;

    [SerializeField]
    Texture2D map;

    ComputeBuffer distanceBuffer;

    [SerializeField]
    ComputeShader findDistances;

    [SerializeField]
    Material blitMaterial; //used to blit strips

    Vector4[] distances = new Vector4[512];

    RenderTexture goalTexture;

    Texture2D sourceTexture;

    void Start()
    {

    }

    private void OnEnable()
    {
        distanceBuffer = new ComputeBuffer(distances.Length, 4 * sizeof(float));
        findDistances.SetBuffer(0, "distances", distanceBuffer);
        findDistances.SetTexture(0, "map", map);
        findDistances.SetFloat("mapSize", map.width);
        Shader.SetGlobalVector("forward", new Vector2(0, 1f));

        goalTexture = new RenderTexture(512, 512, 0);
        sourceTexture = new Texture2D(512, 512); //contents don't matter we just need it to blit

        position = new Vector2(position.x, 0.45f);
        while (position.y < 512.5f)
        {
            RenderStrip();
            position += Vector2.up;
        }

        SaveRenderImage(goalTexture);
    }

    private void OnDisable()
    {
        distanceBuffer.Release();
        distanceBuffer = null;
    }

    private void RenderStrip ()
    {        
        Shader.SetGlobalVector("position", position);
        findDistances.Dispatch(0, distances.Length, 1, 1);
        distanceBuffer.GetData(distances);

        blitMaterial.SetVectorArray("stripDistances", distances);
        Graphics.Blit(sourceTexture, goalTexture, blitMaterial, -1);
        //newTextureB
        //strip material
        //render to rendertexture?
    }


    private void SaveRenderImage(RenderTexture goalRender)
    {
        //save goalRender texture and blitMaterial as resources
        var saveTex = new Texture2D(goalRender.width, goalRender.height, TextureFormat.RGB24, false);
        RenderTexture.active = goalRender;
        saveTex.ReadPixels(new Rect(0, 0, goalRender.width, goalRender.height), 0, 0);
        RenderTexture.active = null;
        byte[] bytes;
        bytes = saveTex.EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets/SpeedRacer/" + fileName + ".png", bytes);
    }
}
