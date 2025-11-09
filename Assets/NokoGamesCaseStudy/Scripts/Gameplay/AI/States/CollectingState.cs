using System.Collections;

public class CollectingState : IAIState
{
    public void Enter(AICharacterController controller) { }

    public IEnumerator Execute(AICharacterController controller)
    {
        yield return controller.WorkAtArea(controller.CollectArea, controller.WorkDuration);
        controller.ChangeState(new IdleState());
    }

    public void Exit(AICharacterController controller) { }
}