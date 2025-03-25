public interface ICommand
{
    public void Execute();
    public bool WillEndTurn();
}