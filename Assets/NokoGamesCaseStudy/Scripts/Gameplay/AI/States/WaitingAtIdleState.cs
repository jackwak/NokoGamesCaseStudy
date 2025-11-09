using System.Collections;
using UnityEngine;

public class WaitingAtIdleState : IAIState
{
    public void Enter(AICharacterController controller) { }

    public IEnumerator Execute(AICharacterController controller)
    {
        if (Vector3.Distance(controller.transform.position, controller.InitialPosition) > 0.5f)
        {
            yield return controller.ReturnToIdlePosition();
        }

        while (true)
        {
            yield return new WaitForSeconds(controller.CheckInterval);

            if (controller.CanDropToArea())
            {
                controller.ChangeState(new IdleState());
                yield break;
            }
        }
    }

    public void Exit(AICharacterController controller) { }
}