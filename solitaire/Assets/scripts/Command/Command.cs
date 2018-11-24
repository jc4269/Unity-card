using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This will be the base of an action to use and undo and redo manager
public interface ICommand
{
    //This function will implement the action code to take. It will need to store the variables necessary to undo the action.
    void Execute();
    //This function will implement the reverse of actions taken, using variables stored in the object being implemented from ICommand.
    void Undo();
}

