using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int pos { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int rotationIndex { get; private set; }
    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;
    public void Initialize(Board board,Vector3Int pos,TetrominoData data)
    {
        this.board = board;
        this.pos = pos;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if(cells == null)
        {
            this.cells=new Vector3Int[data.cells.Length];
        }
        for(int i = 0; i < cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    void Update()
    {
        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if(Time.time >= this.stepTime)
        {
            Step();
        }
        this.board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);
        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }
    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    private void Rotate()
    {
        int originalRotation=this.rotationIndex;
        this.rotationIndex=Wrap(this.rotationIndex++,0,4);

        ApplyRotationMatrix(1);
        if (!TestWallKicks(this.rotationIndex,1))
        {
            this.rotationIndex=originalRotation;
            ApplyRotationMatrix(-1);
        }
    }
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            int x, y;
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0]*direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

            }
            this.cells[i] = new Vector3Int(x, y, 0);
        }

    }

    private bool TestWallKicks(int rotationIndex,int rotationDerection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDerection);
        for(int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }
    private int GetWallKickIndex(int rotationIndex, int rotationDerection)
    {
        int wallKickIndex = rotationIndex * 2;
        if(rotationIndex < 0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input,int min,int max)
    {
        if(input < min)
        {
            return max - (min - input) % (max - min);
        }else
        {
            return min+(input-min)%(max-min);
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPos = this.pos;
        newPos.x += translation.x;
        newPos.y += translation.y;

        bool valid = this.board.IsValidPos(this,newPos);
        if (valid)
        {
            this.pos = newPos;
            this.lockTime = 0f;
        }

        return valid;
    }

}