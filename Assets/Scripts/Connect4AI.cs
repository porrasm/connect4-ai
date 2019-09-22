using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect4AI {

    private int maxTreeCount;
    private int maxDepth = 6;
    private int count;

    private byte colorMax;
    private byte colorMin;
    private TreeNode game;

    public Connect4AI(byte color, byte otherColor, int treeCount) {
        this.colorMax = color;
        this.colorMin = otherColor;
        this.maxTreeCount = treeCount + 100000000;
    }

    public int PlayTurn(byte[,] field) {

        game = new TreeNode(new Turn(field, 0, -1));
        count = 0;

        int[] turns = new int[field.GetLength(0)];

        Max(game, 1);

        int maxIndex = int.MinValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < game.Children.Count; i++) {

            if (game.Children[i].Turn.Value > maxValue) {
                maxIndex = i;
                maxValue = game.Children[i].Turn.Value;
            }
        }

        return game.Children[maxIndex].Turn.Column;
    }


    private float Max(TreeNode node, int depth) {

        count++;

        if (count >= maxTreeCount || depth > maxDepth) {
            // asd("Reached max depth/count");
            return HeurMax(node);
        }

        int gameOverValue = int.MinValue;

        if (GameOver(node.Turn, colorMax, out gameOverValue)) {

            if (gameOverValue == -1) {
                Logger.Log("Found losing turn MAX: " + node.Turn.Field);
            }

            return gameOverValue;
        }

        float value = float.MinValue;

        AddPossibleTurns(colorMax, ref node);

        for (int i = 0; i < node.Children.Count; i++) {

            var child = node.Children[i];
            child.SetTurnValue(Math.Max(value, Min(child, depth + 1)));
            value = child.Turn.Value;
        }

        return value;
    }
    private float Min(TreeNode node, int depth) {

        count++;

        if (count >= maxTreeCount || depth > maxDepth) {
            //asd("Reached max depth/count");
            return HeurMin(node);
        }

        int gameOverValue = int.MinValue;

        if (GameOver(node.Turn, colorMin, out gameOverValue)) {
            
            if (gameOverValue == -1) {
                Logger.Log("Found losing turn MIN: " + node.Turn.Field);
            }

            return gameOverValue;
        }

        float value = float.MaxValue;

        AddPossibleTurns(colorMin, ref node);

        for (int i = 0; i < node.Children.Count; i++) {

            var child = node.Children[i];
            child.SetTurnValue(Math.Min(value, Max(child, depth + 1)));
            value = child.Turn.Value;
        }

        return value;
    }

    private void AddPossibleTurns(byte color, ref TreeNode node) {

        int xLen = node.Turn.Field.GetLength(0);
        int yLen = node.Turn.Field.GetLength(1);

        int checkHeight = yLen - 1;
        //checkHeight = 0;

        for (int x = 0; x < xLen; x++) {
            if (node.Turn.Field[x, checkHeight] == 0) {

                Turn turn = new Turn(NewField(CopyField(node.Turn.Field), x, color), 0, (sbyte)x);

                node.Children.Add(new TreeNode(turn));
            } else {
            }
        }
    }
    private byte[,] NewField(byte[,] field, int x, byte color) {

        int yLen = field.GetLength(1);

        for (int y = 0; y < yLen; y++) {
            if (field[x, y] == 0) {
                field[x, y] = color;
                return field;
            }
        }

        // PrintField(field);

        throw new SystemException("Could not create game field");
        return null;
    }
    public void PrintField(byte[,] field) {

        int xLen = field.GetLength(0);
        int yLen = field.GetLength(1);

        string rows = "";

        for (int y = yLen - 1; y >= 0; y--) {
            rows += RowString(field, y) + "\n";
        }

        Logger.Log(rows);
    }
    private string RowString(byte[,] field, int row) {

        string rowString = row + ": ";

        for (int x = 0; x < field.GetLength(0); x++) {

            int val = field[x, row];

            switch (val) {
                case 0:
                    rowString += "0 ";
                    break;
                case 1:
                    rowString += "Y ";
                    break;
                case 2:
                    rowString += "R ";
                    break;
            }
        }

        return rowString;
    }

    private float HeurMax(TreeNode node) {
        return UnityEngine.Random.value;
    }
    private float HeurMin(TreeNode node) {
        return UnityEngine.Random.value;
    }


    private bool GameOver(Turn turn, byte color, out int endValue) {

        bool noEmpties = true;

        if (turn.Field == null) {
            Logger.Log("Turn field null");
        }

        for (int x = 0; x < turn.Field.GetLength(0); x++) {
            for (int y = 0; y < turn.Field.GetLength(1); y++) {

                if (turn.Field[x, y] == 0) {
                    noEmpties = false;
                } else {

                    int end = GameOver(x, y, turn.Field, color);

                    if (end != 0) {
                        endValue = end;
                        return true;
                    }
                }
            }
        }

        endValue = int.MinValue;
        return noEmpties;
    }
    private int GameOver(int x, int y, byte[,] field, byte color) {

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

        // Check up
        for (int i = 1; i < 4; i++) {

            byte newY = (byte)(y + i);

            if (!ValidCoords(x, newY) || field[x, newY] != reference) {
                wins--;
                break;
            }
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

        int refValue = reference == color ? 1 : -1;

        int endValue = wins * refValue;

        if (endValue < 0) {
            Logger.Log("Found end value with negative: " + refValue);
            PrintField(field);
        } else if (endValue > 0) {
            Logger.Log("Found end value with positive: " + refValue);
            PrintField(field);
        }

        return endValue;
    }

    private byte[,] CopyField(byte[,] field) {
        byte[,] fieldCopy = new byte[field.GetLength(0), field.GetLength(1)];

        for (int x = 0; x < field.GetLength(0); x++) {
            for (int y = 0; y < field.GetLength(1); y++) {
                fieldCopy[x, y] = field[x, y];
            }
        }

        return fieldCopy;
    }
}

public struct Turn {
    public byte[,] Field;
    public float Value;
    public sbyte Column;

    public Turn(byte[,] field, float value, sbyte column) {
        this.Field = field;
        this.Value = value;
        this.Column = column;
    }
}