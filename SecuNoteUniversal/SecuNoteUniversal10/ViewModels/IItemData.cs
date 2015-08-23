using System.Threading.Tasks;

namespace SecuNoteUniversal10.ViewModels
{
    public interface IItemData
    {
        string SaveItem(AbstractItemViewModel item);
        string DeleteItem(int id);
        Task<AbstractItemViewModel> GetItem(string name);
    }
}