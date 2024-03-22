public class Usable : InteractableItem
{
    public override void InteractWith()
    {
        base.InteractWith();
        UIController.Instance.AddPickedUp(Data);
    }
}
