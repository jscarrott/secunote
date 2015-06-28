namespace SecuNoteUniversal.ViewModels
{
    public interface IItemData
    {
        string SaveItem(AbstractItemViewModel item);
        string DeleteItem(int id);
        AbstractItemViewModel GetItem(int id);
    }
}