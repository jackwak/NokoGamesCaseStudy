using System.Collections;

public class MovingToCollectState : IAIState
{
    public void Enter(AICharacterController controller)
    {
        controller.SetWalkingState(true);
    }

    public IEnumerator Execute(AICharacterController controller)
    {
        yield return controller.MoveToArea(controller.CollectArea);
        controller.ChangeState(new CollectingState());
    }

    public void Exit(AICharacterController controller)
    {
        controller.SetWalkingState(false);
    }
}