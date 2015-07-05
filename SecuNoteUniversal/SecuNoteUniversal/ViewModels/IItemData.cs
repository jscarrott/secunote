using System.Threading.Tasks;

namespace SecuNoteUniversal.ViewModels
{
    public interface IItemData
    {
        string SaveItem(AbstractItemViewModel item);
        string DeleteItem(int id);
         Task<AbstractItemViewModel> GetItem(string name);
    }
}