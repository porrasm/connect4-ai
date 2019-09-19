using System.Collections.Generic;

public class TreeNode {

    private Turn turn;
    public Turn Turn { get => turn; }
    public List<TreeNode> Children { get; }

    public TreeNode(Turn turn) {
        this.turn = turn;
        Children = new List<TreeNode>();
    }
    
    public void Add(Turn value) {
        Add(new TreeNode(value));
    }
    public void Add(TreeNode node) {
        Children.Add(node);
    }
    public void SetTurnValue(float value) {
        turn.Value = value;
    }
}