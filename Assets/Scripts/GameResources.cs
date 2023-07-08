using System.Collections;
using System.Collections.Generic;
using BulletCircle.GoViews;
using UnityEngine;

[CreateAssetMenu(fileName = "GameResources", menuName = "GameResources", order = -100)]
public class GameResources : ScriptableObject
{
    private static GameResources m_Instance = null;

    private Dictionary<string, Material> m_Materials = new();
    
    public Mesh MagazineMesh;
    public Material MagazineMaterial;
    public ShooterGOView shooterGoViewPrefab;

    

    public static GameResources Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = Resources.Load<GameResources>("GameResources");
                m_Instance.Init();
            }

            return m_Instance;
        }
    }

    
    private void Init()
    {
        
    }
}