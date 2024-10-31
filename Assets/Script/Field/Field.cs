using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Field : MonoBehaviour
{
    private Dictionary<Vector2, Tile> _tileDict = new();
    public Dictionary<Vector2, Tile> TileDict => _tileDict;

    private FieldColorType _fieldType = FieldColorType.none;
    public FieldColorType FieldType => _fieldType;

    // 필드의 생성을 위한 필드의 위치
    private readonly Vector3 FieldPosition = new(0.05f, -1.06f, 0);
    private readonly Vector3 FieldRotation = new(24f, 0, 0);
    private readonly Vector3 FieldScale = new(14.2f, 6.1f, 1f);

    private const int MaxFieldX = 6;
    private const int MaxFieldY = 3;

    readonly List<Vector2> UDLR = new() { Vector2.right, Vector2.up, Vector2.left, Vector2.down };

    public Vector2? NowHighlightFrameOnLocation;

    public Color ColorList(FieldColorType color)
    {
        return color switch
        {
            FieldColorType.UnitSpawn => new Color32(23, 114, 102, 40),
            FieldColorType.Move => new Color32(23, 114, 102, 40),
            FieldColorType.Attack => new Color32(220, 20, 60, 40),
            FieldColorType.PlayerSkill => new Color32(220, 20, 60, 40),
            FieldColorType.EnemyPlayerSkill => new Color32(220, 20, 60, 40),
            FieldColorType.PlayerPlayerSkill => new Color32(23, 114, 102, 40),
            FieldColorType.none => new Color32(0, 0, 0, 0),
            _ => new Color32(23, 114, 102, 40),
        };
    }

    private void Awake()
    {
        for (int i = 0; i < MaxFieldY; i++)
            for (int j = 0; j < MaxFieldX; j++)
                _tileDict.Add(new Vector2(j, i), CreateTile(j, i));

        transform.position = FieldPosition;
        transform.eulerAngles = FieldRotation;
        transform.localScale = FieldScale;
    }

    private Tile CreateTile(int x, int y)
    {
        x -= MaxFieldX / 2;
        y -= MaxFieldY / 2;

        float disX = transform.localScale.x / MaxFieldX;
        float disY = transform.localScale.y / MaxFieldY;

        float locX = (disX * x) + (disX * 0.5f);
        float locY = disY * y + 1.5f;

        Vector3 tilePos = new(locX, transform.position.y + locY);
        GameObject tileObject = GameManager.Resource.Instantiate("Tile", transform);

        return tileObject.GetComponent<Tile>().Init(tilePos);
    }

    // 타일의 좌표값을 리턴한다.
    public Vector2 GetCoordByTile(Tile tile)
    {
        // 타일이 없으면 -1, -1을 반환
        if(tile == null)
        {
            Debug.Log("Tile Parameter is Null");
            return new Vector2(-1, -1);
        }

        foreach (KeyValuePair<Vector2, Tile> items in TileDict)
            if (items.Value == tile)
                return items.Key;

        Debug.Log("Can't find target tile");
        return new Vector2(-1, -1);
    }

    public BattleUnit GetUnit(Vector2 coord)
    {
        if (!IsInRange(coord) || !TileDict[coord].UnitExist)
            return null;

        if (TileDict[coord].Unit.IsConnectedUnit)
            return TileDict[coord].Unit.GetOriginalUnit();
        else
            return TileDict[coord].Unit;
    }

    public List<BattleUnit> GetUnitsInRange(Vector2 originCoord, List<Vector2> areaCoords, Team? team = null)
    {
        List<BattleUnit> units = new();

        foreach (Vector2 coord in areaCoords)
        {
            BattleUnit unit = GetUnit(originCoord + coord);
            if (unit != null)
            {
                if (team == null || unit.Team == team)
                {
                    units.Add(unit);
                }
            }
        }

        return units;
    }

    public List<BattleUnit> GetArroundUnits(Vector2 unitCoord) => GetUnitsInRange(unitCoord, UDLR);
    public List<BattleUnit> GetArroundUnits(Vector2 unitCoord, Team team) => GetUnitsInRange(unitCoord, UDLR, team);

    public List<Vector2> GetFieldAllCoord()
    {
        List<Vector2> Coords = new();

        for (int i = 0; i < MaxFieldY; i++)
            for (int j = 0; j < MaxFieldX; j++)
                Coords.Add(new Vector2(j, i));

        return Coords;
    }

    public List<Vector2> GetUnitAllCoord()
    {
        List<Vector2> Coords = new();

        for (int i = -MaxFieldY; i <= MaxFieldY; i++)
            for (int j = -MaxFieldX; j <= MaxFieldX; j++)
                Coords.Add(new Vector2(j, i));

        return Coords;
    }

    //십자가 범위 유닛
    public List<Vector2> GetCrossCoord(Vector2 unitCoord)
    {
        List<Vector2> Coords = new();
        Coords.Add(unitCoord);

        for(int i=1; i < MaxFieldX; i++)
        {
            foreach(Vector2 udlr in UDLR)
            {
                Vector2 vec = unitCoord + udlr * i;
                if (IsInRange(vec))
                    Coords.Add(vec);
            }
        }

        return Coords;
    }

    // 타일이 최대 범위를 벗어났는지 확인
    public bool IsInRange(Vector2 coord) => TileDict.ContainsKey(coord);

    // 지정한 위치에 있는 타일의 좌표를 반환
    public Vector3 GetTilePosition(Vector2 coord)
    {
        if (IsInRange(coord))
        {
            Vector3 position = TileDict[coord].transform.position;

            float sizeY = TileDict[coord].transform.lossyScale.y * 0.2f;
            position.y -= sizeY;

            return position;
        }
        else
        {
            return new(0, 0, 0);
        }
    }

    public void SetNextActionTileColor(BattleUnit unit, FieldColorType fieldType)
    {
        List<Vector2> rangeList = new();

        if (fieldType == FieldColorType.Move)
            rangeList = unit.GetMoveRange();
        else if (fieldType == FieldColorType.Attack)
            rangeList = unit.GetAttackRange();

        foreach (Vector2 vec in rangeList)
        {
            Vector2 range = unit.Location + vec;
            if (IsInRange(range))
            {
                TileDict[range].SetColor(ColorList(fieldType));
            }
        }
    }

    public void SetEffectTile(List<Vector2> vectorList, EffectTileType effectType)
    {
        foreach (Vector2 vec in vectorList)
        {
            if (IsInRange(vec))
                _tileDict[vec].SetEffect(effectType);
        }
    }

    public void ClearEffectTile(List<Vector2> vectorList, EffectTileType effectType)
    {
        foreach (Tile tile in TileDict.Values)
        {
            tile.ClearEffect(effectType);
        }
    }

    public void SetSpawnTileColor(FieldColorType fieldType, DeckUnit deckUnit)
    {
        foreach (Vector2 spawnTile in TileDict.Keys)
        {
            if (CheckSpawnTile(deckUnit, spawnTile))
                continue;
            if (TileDict[spawnTile].UnitExist)
                continue;

            List<Vector2> tempList = new();

            if (UnitSizeCheck(spawnTile, deckUnit))
            {
                foreach (Vector2 size in deckUnit.GetUnitSizeRange())
                {
                    TileDict[spawnTile + size].SetColor(ColorList(fieldType));
                }
            }
        }

        _fieldType = fieldType;
    }

    public void SetUnitTileColor(FieldColorType fieldType)
    {
        SetEnemyUnitTileColor(fieldType);
        SetFriendlyUnitTileColor(fieldType);
    }

    public void SetEnemyUnitTileColor(FieldColorType fieldType)
    {
        foreach (Tile tile in TileDict.Values)
        {
            if (tile.UnitExist && tile.Unit.Team == Team.Enemy)
            {
                tile.SetColor(ColorList(FieldColorType.EnemyPlayerSkill));
            }
        }

        _fieldType = fieldType;
    }
    
    public void SetFriendlyUnitTileColor(FieldColorType fieldType)
    {
        foreach (Tile tile in TileDict.Values)
        {
            if (tile.UnitExist && tile.Unit.Team == Team.Player)
            {
                tile.SetColor(ColorList(FieldColorType.PlayerPlayerSkill));
            }
        }

        _fieldType = fieldType;
    }

    public void SetAllTileColor(FieldColorType fieldType)
    {
        foreach (Tile tile in TileDict.Values)
        {
            tile.SetColor(ColorList(fieldType));
        }

        _fieldType = fieldType;
    }

    public void SetNotBattleOnlyUnitTileColor(FieldColorType fieldType)
    {
        foreach (Tile tile in TileDict.Values)
        {
            if (tile.UnitExist && tile.Unit.Team == Team.Player && !tile.Unit.Data.IsBattleOnly)
            {
                tile.SetColor(ColorList(FieldColorType.PlayerPlayerSkill));
            }
        }

        _fieldType = fieldType;
    }

    public void ClearAllColor()
    {
        foreach (Tile tile in TileDict.Values)
        {
            tile.SetColor(Color.white);
        }

        FieldCloseSplashRange();
    }

    public void SetTileHighlightFrame(Vector2? location, bool highlightOn)
    {
        if (location.Equals(new Vector2(-1, -1)))
            return;

        if (NowHighlightFrameOnLocation != null)
        {
            TileDict[(Vector2)NowHighlightFrameOnLocation].SetHightlightFrame(false);
        }

        NowHighlightFrameOnLocation = location;

        if (location != null)
        {
            TileDict[(Vector2)location].SetHightlightFrame(highlightOn);
        }
    }

    public bool UnitSizeCheck(Vector2 spawnLocation, DeckUnit deckUnit)
    {
        foreach (Vector2 size in deckUnit.GetUnitSizeRange())
        {
            Vector2 tempVec = spawnLocation + size;

            if (CheckSpawnTile(deckUnit, tempVec) || TileDict[tempVec].UnitExist)
            {
                return false;
            }
        }

        return true;
    }

    public void EnterTile(BattleUnit unit, Vector2 coord) => TileDict[coord].EnterTile(unit);

    public void ExitTile(Vector2 coord) => TileDict[coord].ExitTile();

    private bool CheckSpawnTile(DeckUnit deckUnit, Vector2 spawnTile) => !deckUnit.CanSpawnInEnemyField && !IsPlayerRange(spawnTile);

    public void SetActiveAllTiles(bool isActive)
    {
        foreach (Tile tile in TileDict.Values)
            tile.SetActiveCollider(isActive);
    }

    private bool IsPlayerRange(Vector2 coord) => ((int)coord.x < MaxFieldX / 2 && IsInRange(coord));

    private UI_Info _hoverInfo;

    public void MouseEnterTile(Tile tile)
    {
        FieldShowInfo(tile);

        if (tile.IsColored)
            FieldShowSplashRange(tile);
    }

    public void MouseExitTile(Tile tile)
    {
        FieldCloseInfo(tile);
        FieldCloseSplashRange();
    }

    public void FieldShowInfo(Tile tile)
    {
        if (tile.UnitExist)
        {
            BattleUnit unit = tile.Unit;

            if (unit.IsConnectedUnit)
            {
                unit = unit.GetOriginalUnit();
            }

            FieldCloseInfo(tile);
            _hoverInfo = BattleManager.BattleUI.ShowInfo();
            _hoverInfo.SetInfo(unit);
        }
    }

    public void FieldCloseInfo(Tile tile)
    {
        if (_hoverInfo != null)
        {
            BattleManager.BattleUI.CloseInfo(_hoverInfo);
        }
    }

    public void FieldShowSplashRange(Tile tile)
    {
        if (!BattleManager.Phase.CurrentPhaseCheck(BattleManager.Phase.Action))
        {
            FieldCloseSplashRange();
            return;
        }

        BattleUnit currentUnit = BattleManager.Data.GetNowUnit();
        if (currentUnit == null || currentUnit.Team != Team.Player)
        {
            return;
        }

        foreach (Vector2 range in currentUnit.Action.GetSplashRangeForField(currentUnit, tile, currentUnit.Location))
        {
            if (BattleManager.Field.IsInRange(range))
                BattleManager.Field.TileDict[range].SetRangeHightlight(true);
        }
    }

    public void FieldCloseSplashRange()
    {
        foreach (Tile tile in TileDict.Values)
        {
            tile.SetRangeHightlight(false);
        }
    }
}