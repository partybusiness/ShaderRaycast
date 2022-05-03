using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Monster
{
    public MonsterClass monsterClass;
    public Vector2 position;
    public float rotation;
    public int animationFrame;
    public float distance; //distance to player, used for visual sorting
    public float viewAngle; //angle to position on-screen

}
