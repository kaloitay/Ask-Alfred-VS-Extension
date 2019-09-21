using System;
using System.Collections.ObjectModel;

namespace Ask_Alfred.UI
{
    internal class ComboBoxViewModel
    {
        public ObservableCollection<string> HistorySearches { get; set; }
        public ComboBoxViewModel()
        {
            HistorySearches = new ObservableCollection<string>();
        }
    }
}