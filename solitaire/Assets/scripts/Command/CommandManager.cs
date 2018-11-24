using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour{
    List<ICommand> commands = new List<ICommand>(); // This is the commands stack to be able to undo. using a list incase we need to access specifics.

    public void ExecuteCommand(ICommand command){
        //Debug.Log("ExecuteCommand before execute commands.count=" + commands.Count);
        command.Execute();
        commands.Add(command);
        //Debug.Log("ExecuteCommand after execute commands.count=" + commands.Count);
    }

    public void UndoCommand(){
        //Debug.Log("ExecuteCommand before undo commands.count=" + commands.Count);
        if (commands.Count > 0)
        {
            Debug.Log("Something to undo");
            commands[commands.Count - 1].Undo();
            commands.RemoveAt(commands.Count - 1);
        }
        //Debug.Log("ExecuteCommand after undo commands.count=" + commands.Count);
    }

    public void Clear(){
        commands.Clear();
    }
}
