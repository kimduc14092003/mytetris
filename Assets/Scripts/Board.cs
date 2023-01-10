using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominoes;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Vector3Int spawnPos;
    public Vector2Int boardSize = new Vector2Int(7, 18);
    public TMP_Text scoreText;
    public TMP_Text levelText;

    public int score;
    private bool isGameOver;
    private int level;
    private bool firstPiece;
    public RectInt Bounds
    {
        get
        {
            Vector2Int pos = new Vector2Int(-3-this.boardSize.x/2, -2-this.boardSize.y/2);
            return new RectInt(pos, this.boardSize);
        }
    }
    void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        score = 0;
        level = 1;
        firstPiece = true;
        isGameOver = false;
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random=Random.Range(0, tetrominoes.Length);
        if (firstPiece)
        {
            int randomFirst = Random.Range(0, tetrominoes.Length);
            TetrominoData tetrominoFirst = this.tetrominoes[randomFirst];
            this.activePiece.Initialize(this, this.spawnPos, tetrominoFirst);
            firstPiece = false;
        }
        else
        {
            TetrominoData tetromino = this.tetrominoes[random];
        
            this.activePiece.Initialize(this, this.spawnPos, tetromino);
        }

        if (IsValidPos(this.activePiece, this.spawnPos))
        {
         Set(this.activePiece);
        }
        else if(!isGameOver)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        SceneManager.LoadScene(2,LoadSceneMode.Additive);
        Time.timeScale = 0;
        isGameOver = true;
    }
    public void RestartGame()
    {
        isGameOver=false;   
        this.tilemap.ClearAllTiles();
        Time.timeScale = 1;
        this.score = 0;
        this.level = 1;
        scoreText.text = "Score\n" + this.score;
        levelText.text = "Level\n" + this.level;
        this.activePiece.stepDelay = 1f;
    }


    public void Set(Piece piece)
    {
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos=piece.cells[i]+piece.pos;
            this.tilemap.SetTile(tilePos, piece.data.tile);
        }
    }    
    public void Clear(Piece piece)
    {
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos=piece.cells[i]+piece.pos;
            this.tilemap.SetTile(tilePos, null);
        }
    }

    public bool IsValidPos(Piece piece,Vector3Int pos)
    {
        RectInt bounds = this.Bounds;
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + pos;

            if (!bounds.Contains((Vector2Int)tilePos))
            {
                return false;
            }

            if (this.tilemap.HasTile(tilePos))
            {
                return false;
            }
        }
        return true;
    }
    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row=bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                SetScore();
            }
            else
            {
                row++;
            }
        }
    }

    private void SetScore()
    {
        this.score += 100;
        scoreText.text="Score\n"+this.score;
        Debug.Log(score%500);
        if(this.score % 500 == 0)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        this.activePiece.stepDelay -= 0.15f*this.activePiece.stepDelay;
        levelText.text = "Level\n" + ++this.level;
    }
    

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;
        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int pos=new Vector3Int(col, row,0);
            if (!this.tilemap.HasTile(pos))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;
        for(int col=bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(pos, null);
        }
        while (row < bounds.yMax)
        {
            for(int col=bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int pos=new Vector3Int(col, row+1, 0);
                TileBase above =this.tilemap.GetTile(pos);
                pos = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(pos, above);
            }
            row++;
        }
    }

}
