using System.Collections;

public class IdleState : IAIState
{
    public void Enter(AICharacterController controller) { }

    public IEnumerator Execute(AICharacterController controller)
    {
        while (true)
        {
            if (controller.CollectController.ItemCount > 0)
            {
                if (controller.CanDropToArea())
                {
                    controller.ChangeState(new MovingToDropState());
                    yield break;
                }
                else
                {
                    controller.ChangeState(new WaitingAtIdleState());
                    yield break;
                }
            }

            if (controller.CanCollectFromArea())
            {
                controller.ChangeState(new MovingToCollectState());
                yield break;
            }

            yield return null;
        }
    }

    public void Exit(AICharacterController controller) { }
}