using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRenderer : MonoBehaviour
{
    [SerializeField]
    Monster[] monsters;

    [SerializeField]
    Material monsterMaterial;

    [SerializeField]
    float monsterHeight = 0.7f;


    [SerializeField]
    Vector2 fixedScreenPos;

    [SerializeField]
    float fixedDistance;

    //private Camera monsterCamera;

    RenderStrips strips;

    RenderTexture visibleMap; //displays squares that monsters can see

    private void Start()
    {
        var distantCamera = new GameObject();
        //monsterCamera = distantCamera.AddComponent<Camera>();
        //what should be the setting?
        //monsterCamera.enabled = false;
        strips = GetComponent<RenderStrips>();
        
        monsterMaterial.SetFloat("_ratio", Screen.width / Screen.height);
    }


    public void RenderMonsters(RenderTexture destination)
    {
        monsterMaterial.SetFloat("_ratio", Camera.main.aspect);
        //Debug.Log("Size "+)

        var visibleMonsters = new List<Monster>();
        //monsterCamera.targetTexture = destination;
        foreach (var monster in monsters)
        {
            var diff = monster.position + Vector2.one*0.5f - strips.position;
            monster.distance = diff.magnitude;
            diff = diff.normalized;
            monster.viewAngle = (Mathf.Atan2(diff.y, diff.x) - Mathf.Atan2(strips.forward.y, strips.forward.x));
            //TODO do I need to wrap this around to make sure it doesn't disappear at wrong time?
            if (monster.viewAngle > Mathf.PI)
                monster.viewAngle -= Mathf.PI * 2f;
            if (monster.viewAngle < -Mathf.PI)
                monster.viewAngle += Mathf.PI * 2f;

            var halfWidth = 0.5f / monster.distance;

            if (monster.viewAngle > -strips.fov - halfWidth && monster.viewAngle < strips.fov + halfWidth) //frustum culling // how to pad with monster width?
                visibleMonsters.Add(monster);

            
            //return;
        }

        //sort by distance
        visibleMonsters.Sort(delegate (Monster x, Monster y)
        {
            return y.distance.CompareTo(x.distance);
        });


        for (var i=0;i< visibleMonsters.Count;i++)
        {
            var monster = visibleMonsters[i];
            var screenPos = new Vector2(((-monster.viewAngle / strips.fov) + 1.0f) / 2.0f, monsterHeight); //0.35 //strips.fov
            monsterMaterial.SetFloat("monsterDist", monster.distance);
            monsterMaterial.SetVector("monsterPos", screenPos);
            Graphics.Blit(monster.monsterClass.texture, destination, monsterMaterial);
        }
    }

    internal RenderTexture GenerateVisibleMap(Texture2D map)
    {
        visibleMap = new RenderTexture(map.width, map.height, 16, RenderTextureFormat.RGB111110Float, RenderTextureReadWrite.Linear);
        return visibleMap;
    }
}
