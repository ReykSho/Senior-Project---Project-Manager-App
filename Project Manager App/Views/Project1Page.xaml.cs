using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Project_Manager_App.ViewModels;
using CommunityToolkit.WinUI.UI.Controls;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.Storage;

namespace Project_Manager_App.Views;

public sealed partial class Project1Page : Page
{
    public Project1ViewModel ViewModel { get;}
    public Project1Page()
    {
        ViewModel = App.GetService<Project1ViewModel>();
        this.InitializeComponent();
    }

    private void TabView_Loaded(object sender, RoutedEventArgs e)
    {
        var tabView = sender as TabView;
        if (tabView != null && tabView.TabItems.Count < 3)
        {
            for (var i = tabView.TabItems.Count; i < 3; i++)
            {
                tabView.TabItems.Add(CreateNewTab(i));
            }
        }
    }

    private void TabView_AddButtonClick(TabView sender, object args)
    {
        sender.TabItems.Add(CreateNewTab(sender.TabItems.Count));
    }

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
    }

    private TabViewItem CreateNewTab(int index)
    {
        TabViewItem newItem = new TabViewItem();

        newItem.Header = $"Document {index}";
        newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };

        // The content of the tab is often a frame that contains a page, though it could be any UIElement.
        Frame frame = new Frame();
        {
            frame.Navigate(typeof(ProjectTabView));
        }

        newItem.Content = frame;

        return newItem;
    }
}
