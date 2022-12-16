using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMenuItem : MonoBehaviour
{
    public void LoadMap()
    {
        UiManager.Instance.ToggleMapMenu();
        CubeGenerator.Instance.SpawnMapFromIndex(this.transform.GetSiblingIndex());
    }
}
