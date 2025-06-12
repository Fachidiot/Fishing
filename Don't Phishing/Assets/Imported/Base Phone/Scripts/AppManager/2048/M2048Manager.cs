using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class M2048Manager : BaseAppManager
{
    [SerializeField] private TMP_Text m_scoreTMP;
    [SerializeField] private int m_width = 4;
    [SerializeField] private int m_height = 4;
    [SerializeField] private float m_scale = 300;

    [SerializeField] private Node m_nodePrefab;
    [SerializeField] private Block m_blockPrefab;

    [SerializeField] private GameObject m_nodeParent;
    [SerializeField] private List<BlockType> typeList;
    //[SerializeField] private float travelTime = 0.2f;
    [SerializeField] private int m_windCondition = 2048;

    [SerializeField] private GameObject m_winScreen, m_loseScreen;

    private List<Node> m_nodeList = new List<Node>();
    private List<Block> m_blockList = new List<Block>();
    private Game2048State m_state;
    private int m_round;
    private int m_score = 0;

    private BlockType GetBlockTypeByValue(int _value) => typeList.First(t=>t.value == _value);

    void Start()
    {
        ChangeState(Game2048State.GenerateLevel);
    }

    private void ChangeState(Game2048State _state)
    {
        m_state = _state;
        m_scoreTMP.text = m_score.ToString();

        switch(m_state)
        {
            case Game2048State.GenerateLevel:
                GenerateGrid();
                break;
            case Game2048State.SpawningBlocks:
                SpawnBlocks(m_round++ == 0 ? 2 : 1);
                break;
            case Game2048State.WaitingInput:
                break;
            case Game2048State.Moving:
                break;
            case Game2048State.Win:
                m_winScreen.SetActive(true);
                break;
            case Game2048State.Lose:
                m_loseScreen.SetActive(true);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state != Game2048State.WaitingInput)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Shift(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Shift(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Shift(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            Shift(Vector2.down);
    }

    void GenerateGrid()
    {
        m_round = 0;
        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                Node node = Instantiate(m_nodePrefab, m_nodeParent.transform);
                node.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * m_scale, y * m_scale);
                m_nodeList.Add(node);
            }
        }

        ChangeState(Game2048State.SpawningBlocks);
    }

    void SpawnBlocks(int _amount)
    {
        List<Node> freeNodes = m_nodeList.Where(n => n.OccupiedBlock == null).OrderBy(b => UnityEngine.Random.value).ToList();

        foreach (Node node in freeNodes.Take(_amount))
        {
            SpawnBlock(node, UnityEngine.Random.value > 0.8f ? 4 : 2);
        }

        if (freeNodes.Count() == 1 && !CanMerge())
        {   // Lose the game.
            ChangeState(Game2048State.Lose);
            return; 
        }

        ChangeState(m_blockList.Any(b => b.value == m_windCondition) ? Game2048State.Win : Game2048State.WaitingInput);
    }

    void SpawnBlock(Node _node, int _value)
    {
        if (_value > m_score)
            m_score = _value;
        Block block = Instantiate(m_blockPrefab, new Vector3(_node.Pos.x, _node.Pos.y, 0), Quaternion.identity);
        block.transform.SetParent(m_nodeParent.transform, false);
        block.Init(GetBlockTypeByValue(_value));
        block.SetBlock(_node);
        m_blockList.Add(block);
    }

    void Shift(Vector2 _direction)
    {
        ChangeState(Game2048State.Moving);

        List<Block> orderedBlocks = m_blockList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (_direction == Vector2.right || _direction == Vector2.up) orderedBlocks.Reverse();

        foreach (Block block in orderedBlocks)
        {
            Node next = block.node;
            do
            {
                block.SetBlock(next);

                Node possibleNode = GetNodeAtPosition(next.Pos + _direction * m_scale);
                if (possibleNode != null)
                {
                    // We know a node is present.
                    // If it's possible to merge, set merge.
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.value))
                    {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                    }
                    // Otherwise, can we move to this spot?
                    else if (possibleNode.OccupiedBlock == null)
                        next = possibleNode;

                    // None hit? End do while loop.
                }

            }
            while (next != block.node);
        }

        var sequence = DOTween.Sequence();

        foreach (Block block in orderedBlocks)
        {
            Vector3 movePoint = block.mergingBlock != null ? block.mergingBlock.node.Pos : block.node.Pos;

            //sequence.Insert(0, block.transform.DOMove(movePoint, travelTime));
            block.GetComponent<RectTransform>().anchoredPosition = movePoint;
        }

        sequence.OnComplete(() =>
        {
            foreach (Block block in orderedBlocks.Where(b => b.mergingBlock != null))
            {
                MergeBlocks(block.mergingBlock, block);
            }

            ChangeState(Game2048State.SpawningBlocks);
        });
    }

    bool CanMerge()
    {
        List<Block> orderedBlocks = m_blockList.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();

        return CheckDirection(Vector2.left, orderedBlocks) || CheckDirection(Vector2.right, orderedBlocks)
            || CheckDirection(Vector2.up, orderedBlocks) || CheckDirection(Vector2.down, orderedBlocks); 
    }

    bool CheckDirection(Vector2 _direction, List<Block> _orderedBlocks)
    {
        if (_direction == Vector2.right || _direction == Vector2.up) _orderedBlocks.Reverse();

        foreach (Block block in _orderedBlocks)
        {
            Node next = block.node;
            do
            {
                block.SetBlock(next);

                Node possibleNode = GetNodeAtPosition(next.Pos + _direction * m_scale);
                if (possibleNode != null)
                {
                    // If it's possible to merge
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.value))
                        return true;
                    // Otherwise, can we move to this spot?
                    else if (possibleNode.OccupiedBlock == null)
                        next = possibleNode;

                    // None hit? End do while loop.
                }

            }
            while (next != block.node);
        }

        return false;
    }

    void MergeBlocks(Block baseBlock, Block mergingBlock)
    {
        SpawnBlock(baseBlock.node, baseBlock.value * 2);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block block)
    {
        m_blockList.Remove(block);
        Destroy(block.gameObject);
    }

    Node GetNodeAtPosition(Vector2 _pos)
    {
        return m_nodeList.FirstOrDefault(n=>n.Pos == _pos);
    }

    public override void ResetApp()
    {
        m_blockList.Clear();
        m_nodeList.Clear();
        ChangeState(Game2048State.GenerateLevel);

        return;
    }

    public override void SetText()
    {
        return;
    }
}

[Serializable]
public struct BlockType
{
    public int value;
    public Color color;
}

public enum Game2048State
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}