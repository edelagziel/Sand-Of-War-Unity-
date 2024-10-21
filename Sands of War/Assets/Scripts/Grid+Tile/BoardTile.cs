using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// One Slote of the balfield 
/// </summary>
public class BoardTile : MonoBehaviour
{
    #region varibles
    public int Id_x;
    public int Id_y;
    public GameObject []Highlights;
    public static Action<TileCoordinates> Mouse_Clike;
    public static Action<TileCoordinates> MouseEnter;
    public static Action<TileCoordinates> MouseExit;

    public TileCoordinates Coordinates;
    public bool TileFull=false;
    public bool IsHighlights = false;
    /// <summary>
    /// struct that contains tile position
    /// </summary>
    public struct TileCoordinates
    {
        public int Idx;
        public int Idy;
        public Vector3 TilePos;
        public TileCoordinates(int idx, int idy, Vector3 tilePos)
        {
            Idx = idx;
            Idy = idy;
            TilePos = tilePos;
        }
    };
    #endregion
    #region Logic
    private void OnMouseEnter()
    {
        if (Highlights == null) return;
        IsHighlights=true;
        if (TileFull == false)
            Highlights[0].SetActive(IsHighlights);      //the default Highlights
        else
        {
            MouseEnter?.Invoke(Coordinates);
        }


    }
    private void OnMouseExit()
    {
        if (Highlights == null) return;
        IsHighlights=false;
        if (TileFull == false)
            Highlights[0].SetActive(IsHighlights);      //the default Highlights
        else
        {
            MouseExit?.Invoke(Coordinates);
        }
    }
    private void OnMouseDown()
    {
        Coordinates = new TileCoordinates(Id_x, Id_y, transform.position);
        Mouse_Clike?.Invoke(Coordinates);
    }


    public void ResertTile()
    {
        for (int i = 0;i< Highlights.Length;i++)
            Highlights[i].SetActive(false);
        TileFull = false;
    }
    #endregion
    #region MonoBehaviour
    private void OnEnable()
    {
        GameManager.ReasetGame += ResertTile;
    }
    private void OnDisable()
    {
        GameManager.ReasetGame -= ResertTile;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {

    }
    #endregion 
}
