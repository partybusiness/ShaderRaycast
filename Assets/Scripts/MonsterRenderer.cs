using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRenderer : MonoBehaviour
{
    [SerializeField]
    Monster[] monsters;

    [SerializeField]
    Material monsterMaterial;

    private Camera monsterCamera;

    RenderStrips strips;

    private void Start()
    {
        var distantCamera = new GameObject();
        monsterCamera = distantCamera.AddComponent<Camera>();
        //what should be the setting?
        monsterCamera.enabled = false;
        strips = GetComponent<RenderStrips>();
    }


    public void RenderMonsters(RenderTexture destination)
    {
        monsterCamera.targetTexture = destination;
        foreach (var monster in monsters)
        {
            monster.monsterClass.material.SetFloat("monsterDist", 1f);
            //find angle between 

            //float angle = id.x / 256.0 - 1.0; //angle offset of this strip ranging from -1.0 to 1.0
            //float B = -angle * fov; //convert to radians, decide how wide view is?
            //float2 newForward = float2(forward.x * cos(B) - forward.y * sin(B), forward.x * sin(B) + forward.y * cos(B)); //rotate vector by B?
            var diff = monster.position - strips.position;
            var angle = (Mathf.Atan2(diff.y, diff.x) - Mathf.Atan2(strips.forward.y, strips.forward.x)) ;

            var screenPos = new Vector2(angle / strips.fov, 0.35f); //0.35
            //var renderPos = monsterCamera.ScreenToWorldPoint(screenPos);
            var distance = Vector2.Distance(monster.position, strips.position);
            //var renderMatrix = Matrix4x4.TRS(renderPos, Quaternion.identity, Vector3.one);
            //monster.monsterClass.material, 1

            // Graphics.SetRenderTarget(destination);
            // Graphics.DrawMeshNow(monster.monsterClass.mesh, renderMatrix);
            //Graphics.CopyTexture();

            Debug.Log(distance);

            monsterMaterial.SetFloat("monsterDist", distance);
            monsterMaterial.SetVector("monsterPos", screenPos);
            Graphics.Blit(monster.monsterClass.texture, destination, monsterMaterial);
            //Graphics.BlitMultiTap(monster.monsterClass.texture, destination, monsterMaterial, Vector2.zero);
            return;
        }
    }
}
