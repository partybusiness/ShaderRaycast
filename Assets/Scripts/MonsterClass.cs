using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterClass", menuName = "ScriptableObjects/MonsterClass", order = 1)]
public class MonsterClass : ScriptableObject
{
    public Mesh mesh;
    public float rotationSpeed = 40f;
    public float movementSpeed = 5f;

}
