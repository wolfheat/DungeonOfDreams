public class Usable : Interactable
{
    public override void InteractWith()
    {
        base.InteractWith();
        UIController.Instance.AddPickedUp(Data);
    }
}
