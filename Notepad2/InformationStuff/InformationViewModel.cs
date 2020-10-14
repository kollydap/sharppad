using Notepad2.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Notepad2.InformationStuff
{
    public class InformationViewModel : BaseViewModel
    {
        public ObservableCollection<InformationModel> InformationItems { get; set; }

        public ICommand ClearInfoItemsCommand { get; }

        public InformationViewModel()
        {
            InformationItems = new ObservableCollection<InformationModel>();

            ClearInfoItemsCommand = new Command(Clear);
        }

        public void AddInformation(InformationModel information)
        {
            InformationItems.Insert(0, information);
        }

        public void RemoveInformation(InformationModel information)
        {
            InformationItems.Remove(information);
        }

        public void Clear()
        {
            InformationItems.Clear();
        }
    }
}
