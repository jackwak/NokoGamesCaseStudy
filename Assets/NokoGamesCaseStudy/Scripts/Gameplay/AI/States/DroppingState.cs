using System.Collections;

public class DroppingState : IAIState
{
    public void Enter(AICharacterController controller) { }

    public IEnumerator Execute(AICharacterController controller)
    {
        yield return controller.WorkAtArea(controller.DropArea, controller.WorkDuration);
        controller.ChangeState(new IdleState());
    }

    public void Exit(AICharacterController controller) { }
}