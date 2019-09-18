using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect4AI {

    private int maxTreeCount;
    private int maxDepth = 5;
    private int count;

    private byte color;
    private TreeNode game;

    public Connect4AI(byte color, int treeCount) {
        this.color = color;
        this.maxTreeCount = treeCount;
    }

    public int PlayTurn(byte[,] field) {

        game = new TreeNode(new Turn(field, 0, -1));
        count = 0;

        int[] turns = new int[field.GetLength(0)];

        Max(game, 1);

        int maxIndex = int.MinValue;
        int maxValue = int.MinValue;

        for (int i = 0; i < game.Children.Count; i++) {
            
            if (game.Children[i].Turn.Value > maxValue) {
                maxIndex = i;
                maxValue = game.Children[i].Turn.Value;
            }

        }

        return game.Children[maxIndex].Turn.Column;
    }


    private int Max(TreeNode node, int depth) {

        count++;

        if (count >= maxTreeCount || depth > maxDepth) {
            return HeurMax(node);
        }

        if (GameOver(node.Turn)) {
            return node.Turn.Value;
        }

        int value = int.MinValue;

        AddPossibleTurns(ref node);

        for (int i = 0; i < node.Children.Count; i++) {

            var child = node.Children[i];
            child.SetTurnValue(Math.Max(value, Min(child, depth + 1)));
            value = child.Turn.Value;
        }

        return value;
    }
    private int Min(TreeNode node, int depth) {

        count++;

        if (count >= maxTreeCount || depth > maxDepth) {
            return HeurMin(node);
        }

        if (GameOver(node.Turn)) {
            return node.Turn.Value;
        }

        int value = int.MaxValue;

        AddPossibleTurns(ref node);

        for (int i = 0; i < node.Children.Count; i++) {

            var child = node.Children[i];
            child.SetTurnValue(Math.Min(value, Max(child, depth + 1)));
            value = child.Turn.Value;
        }

        return value;
    }

    private void AddPossibleTurns(ref TreeNode node) {

        int xLen = node.Turn.Field.GetLength(0);
        int yLen = node.Turn.Field.GetLength(1);

        for (int x = 0; x < xLen; x++) {
            if (node.Turn.Field[x, yLen - 1] == 0) {

                Turn turn = new Turn(NewField(node.Turn.Field, x), 0, (sbyte)x);

                node.Children.Add(new TreeNode(turn));
            }
        }
    }
    private byte[,] NewField(byte[,] field, int x) {

        int yLen = field.GetLength(1);

        for (int y = yLen - 1; y >= 0; y--) {
            if (field[x, y] != 0) {
                field[x, y] = color;
                return field;
            }
        }

        throw new SystemException("Could not create game field");
        return null;
    }

    private int HeurMax(TreeNode node) {
        return 0;
    }
    private int HeurMin(TreeNode node) {
        return 0;
    }


    private bool GameOver(Turn turn) {

        bool noEmpties = true;

        if (turn.Field == null) {
            Debug.Log("Turn field null");
        }

        for (int x = 0; x < turn.Field.GetLength(0); x++) {
            for (int y = 0; y < turn.Field.GetLength(1); y++) {

                if (turn.Field[x, y] == 0) {
                    noEmpties = false;
                } else {
                    if (GameOver(x, y, turn.Field)) {
                        return true;
                    }
                }
            }
        }

        return noEmpties;
    }
    private bool GameOver(int x, int y, byte[,] field) {

        byte reference = field[x, y];

        bool ValidCoords(int xV, int yV) {
            return xV < field.GetLength(0) && yV < field.GetLength(1);
        }

        byte wins = 3;

        // Check right
        for (int i = 1; i < 4; i++) {

            byte newX = (byte)(x + i);

            if (!ValidCoords(newX, y) || field[newX, y] != reference) {
                wins--;
                break;
            }
        }
        if (wins == 3) {
            return true;
        }

        // Check up
        for (int i = 1; i < 4; i++) {

            byte newY = (byte)(y + i);

            if (!ValidCoords(x, newY) || field[x, newY] != reference) {
                wins--;
                break;
            }
        }
        if (wins == 2) {
            return true;
        }

        //Check up-right
        for (int i = 1; i < 4; i++) {

            byte newX = (byte)(x + i);
            byte newY = (byte)(y + i);

            if (!ValidCoords(newX, newY) || field[newX, newY] != reference) {
                wins--;
                break;
            }
        }

        return wins == 1;
    }
}

public struct Turn {
    public byte[,] Field;
    public int Value;
    public sbyte Column;

    public Turn(byte[,] field, sbyte value, sbyte column) {
        this.Field = field;
        this.Value = value;
        this.Column = column;
    }
}