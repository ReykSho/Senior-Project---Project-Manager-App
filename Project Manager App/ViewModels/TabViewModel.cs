using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using Project_Manager_App.Contracts.Services;
using Project_Manager_App.Services;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.Storage;
using Project_Manager_App.Views;

namespace Project_Manager_App.ViewModels;


public sealed partial class TabViewModel : ObservableRecipient
{

    public TabViewModel()
    {
       
    }

}
