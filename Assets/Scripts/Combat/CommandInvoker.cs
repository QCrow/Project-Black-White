using System.Collections.Generic;

public class CommandInvoker
{
    private Stack<ICommand> _commandStack = new Stack<ICommand>();

    public void SetCommand(ICommand command)
    {
        _commandStack.Push(command);
    }

    public void ExecuteCommand()
    {
        if (_commandStack.Count > 0)
        {
            ICommand command = _commandStack.Pop();
            command.Execute();
        }
    }

    public void UndoCommand()
    {
        if (_commandStack.Count > 0)
        {
            ICommand command = _commandStack.Pop();
            command.Undo();
        }
    }
}