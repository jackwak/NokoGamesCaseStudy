using System.Collections;

public class MovingToDropState : IAIState
{
    public void Enter(AICharacterController controller)
    {
        controller.SetWalkingState(true);
    }

    public IEnumerator Execute(AICharacterController controller)
    {
        yield return controller.MoveToArea(controller.DropArea);
        controller.ChangeState(new DroppingState());
    }

    public void Exit(AICharacterController controller)
    {
        controller.SetWalkingState(false);
    }
}