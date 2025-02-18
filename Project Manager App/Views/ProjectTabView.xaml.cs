using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.UI.Text;
using Windows.UI;
using System.Runtime.InteropServices;
using WinRT;
using Project_Manager_App.Helpers;
using Project_Manager_App.ViewModels;
using Project_Manager_App.Contracts.Services;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Project_Manager_App.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProjectTabView : Page
{
    private Color currentColor = Colors.White;
    public TabViewModel ViewModel
    {
        get;
    }

    public ProjectTabView()
    {   
        this.InitializeComponent();
        ViewModel = App.GetService<TabViewModel>();
    }
    
    public async void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new Window();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Open a text file.
        FileOpenPicker open = new FileOpenPicker();
        open.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        open.FileTypeFilter.Add(".rtf");

        WinRT.Interop.InitializeWithWindow.Initialize(open, hwnd);

        StorageFile file = await open.PickSingleFileAsync();

        if (file != null)
        {
            IRandomAccessStream randAccStream =
                await file.OpenAsync(FileAccessMode.Read);
            {
                // Load the file into the Document property of the RichEditBox.
                editor.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream);
            }
        }
    }

    public async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new Window();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        FileSavePicker savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

        // Dropdown of file types the user can save the file as
        savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

        // Default file name if the user does not type one in or select a file to replace
        savePicker.SuggestedFileName = "New Document";

        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

        var file = await savePicker.PickSaveFileAsync();

        if (file != null)
        {
            // Prevent updates to the remote version of the file until we
            // finish making changes and call CompleteUpdatesAsync.
            CachedFileManager.DeferUpdates(file);
            // write to file
            IRandomAccessStream randAccStream =
                await file.OpenAsync(FileAccessMode.ReadWrite);
            {
                editor.Document.SaveToStream(TextGetOptions.FormatRtf, randAccStream);
            }

            // Let Windows know that we're finished changing the file so the
            // other app can update the remote version of the file.
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
            if (status != FileUpdateStatus.Complete)
            {
                Windows.UI.Popups.MessageDialog errorBox =
                    new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                await errorBox.ShowAsync();
            }
        }
    }

    private void BoldButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
    }

    private void ItalicButton_Click(object sender, RoutedEventArgs e)
    {
        editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
    }

    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        // Extract the color of the button that was clicked.
        Button clickedColor = (Button)sender;
        var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
        var color = ((SolidColorBrush)rectangle.Fill).Color;

        editor.Document.Selection.CharacterFormat.ForegroundColor = color;

        fontColorButton.Flyout.Hide();
        editor.Focus(FocusState.Keyboard);
        currentColor = color;
    }

    private void FindBoxHighlightMatches()
    {
        FindBoxRemoveHighlights();

        Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
        Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

        string textToFind = findBox.Text;
        if (textToFind != null)
        {
            ITextRange searchRange = editor.Document.GetRange(0, 0);
            while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
            }
        }
    }

    private void FindBoxRemoveHighlights()
    {
        ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush? defaultBackground = editor.Background as SolidColorBrush;
        SolidColorBrush? defaultForeground = editor.Foreground as SolidColorBrush;

        documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
        documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
    }

    private void Editor_GotFocus(object sender, RoutedEventArgs e)
    {
        editor.Document.GetText(TextGetOptions.UseCrlf, out _);

        // reset colors to correct defaults for Focused state
        ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

        if (background != null)
        {
            documentRange.CharacterFormat.BackgroundColor = background.Color;
        }
    }

    private void Editor_TextChanged(object sender, RoutedEventArgs e)
    {
        if (editor.Document.Selection.CharacterFormat.ForegroundColor != currentColor)
        {
            editor.Document.Selection.CharacterFormat.ForegroundColor = currentColor;
        }
    }
}
