using System.Collections;

public interface IAIState
{
    void Enter(AICharacterController controller);
    IEnumerator Execute(AICharacterController controller);
    void Exit(AICharacterController controller);
}