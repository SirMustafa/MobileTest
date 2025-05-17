using System;
using UnityEngine;

[Serializable]
public class Blocks
{
    [SerializeField] public GameObject slotObject;
    [SerializeField] public Vector2 index;
    [SerializeField] public bool isBreakable;
}