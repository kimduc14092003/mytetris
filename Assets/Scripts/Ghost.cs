using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; } 
    public Vector3Int pos { get; private set; }

    private void Awake()
    {
        this.tilemap=GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }
    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePos = this.cells[i] + this.pos;
            this.tilemap.SetTile(tilePos, null);
        }
    }   
    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }   
    private void Drop()
    {
        Vector3Int pos = this.trackingPiece.pos;
        int current = pos.y;
        int bottom = -this.board.boardSize.y / 2 - 2;
        this.board.Clear(this.trackingPiece);
        for(int row = current; row >= bottom; row--)
        {
            pos.y = row;
            if (this.board.IsValidPos(this.trackingPiece, pos))
            {
                this.pos = pos;
            }else
            {
                break;
            }
        }
        this.board.Set(this.trackingPiece);
    }   
    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePos = this.cells[i] + this.pos;
            this.tilemap.SetTile(tilePos, this.tile);
        }
    }
}
