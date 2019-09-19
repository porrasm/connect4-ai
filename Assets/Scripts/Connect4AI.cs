using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect4AI {

    private int maxTreeCount;
    private int maxDepth = 3;
    private int count;

    private byte color;
    private TreeNode game;

    public Connect4AI(byte color, int treeCount) {
        this.color = color;
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

            Debug.Log("i: " + i + "; child column: " + game.Children[i].Turn.Column + ", value: " + game.Children[i].Turn.Value);

            if (game.Children[i].Turn.Value > maxValue) {
                maxIndex = i;
                maxValue = game.Children[i].Turn.Value;
            }
        }

        Debug.Log("End count: " + count);
        Debug.Log("Child count: " + game.Children.Count);

        return game.Children[maxIndex].Turn.Column;
    }


    private float Max(TreeNode node, int depth) {

        count++;

        if (count >= maxTreeCount || depth > maxDepth) {
           // Debug.Log("Reached max depth/count");
            return HeurMax(node);
        }

        int gameOverValue = int.MinValue;

        if (GameOver(node.Turn, out gameOverValue)) {
          //  Debug.Log("GAME OVER: " + gameOverValue);
           // PrintField(node.Turn.Field);
            return gameOverValue;
        }

        float value = float.MinValue;

        AddPossibleTurns(ref node);

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
            //Debug.Log("Reached max depth/count");
            return HeurMin(node);
        }

        int gameOverValue = int.MinValue;

        if (GameOver(node.Turn, out gameOverValue)) {
           // Debug.Log("GAME OVER: " + gameOverValue);
           // PrintField(node.Turn.Field);
            return gameOverValue;
        }

        float value = float.MaxValue;

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

        int checkHeight = yLen - 1;
        checkHeight = 0;
        //checkHeight = 0;

        for (int x = 0; x < xLen; x++) {
            if (node.Turn.Field[x, checkHeight] == 0) {

                Debug.Log("Possible turn: " + x);

                Turn turn = new Turn(NewField(CopyField(node.Turn.Field), x), 0, (sbyte)x);

                PrintField(turn.Field);

                node.Children.Add(new TreeNode(turn));
            } else {
                Debug.Log("Check height: " + checkHeight);
                Debug.Log("Not possible turn " + x + "\nval (" + x + ", " + checkHeight + "): " + node.Turn.Field[x, checkHeight]);
                PrintField(node.Turn.Field);  
            }
        }

        Debug.Log("Added possible turns: " + node.Children.Count);
        PrintField(node.Turn.Field);
    }
    private byte[,] NewField(byte[,] field, int x) {

        int yLen = field.GetLength(1);

        for (int y = yLen - 1; y >= 0; y--) {
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

        for (int x = 0; x <xLen; x++) {

            string row = "";

            for (int y = yLen - 1; y >= 0; y--) {
                int val = field[x, y];

                switch (val) {
                    case 0:
                        row += "0 ";
                        break;
                    case 1:
                        row += "B ";
                        break;
                    case 2:
                        row += "R ";
                        break;
                }
            }

            rows += row + "\n";
        }

        Debug.Log(rows);
    }

    private float HeurMax(TreeNode node) {
        return UnityEngine.Random.value;
        return 0;
        return (int)(UnityEngine.Random.value * 2 - 1);

        Debug.Log("Heur max");
        return -1;
    }
    private float HeurMin(TreeNode node) {
        return UnityEngine.Random.value;
        return 0;
        return (int)(UnityEngine.Random.value * 2 - 1);
        Debug.Log("Heru min");
        return -1;
    }


    private bool GameOver(Turn turn, out int endValue) {

        bool noEmpties = true;

        if (turn.Field == null) {
            Debug.Log("Turn field null");
        }

        for (int x = 0; x < turn.Field.GetLength(0); x++) {
            for (int y = 0; y < turn.Field.GetLength(1); y++) {

                if (turn.Field[x, y] == 0) {
                    noEmpties = false;
                } else {
                    if (GameOver(x, y, turn.Field, out endValue)) {
                        return true;
                    }
                }
            }
        }

        endValue = int.MinValue;
        return noEmpties;
    }
    private bool GameOver(int x, int y, byte[,] field, out int endValue) {

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

            endValue = reference == color ? 1 : -1;

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

            endValue = reference == color ? 1 : -1;

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

        endValue = reference == color ? 1 : -1;

        return wins == 1;
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