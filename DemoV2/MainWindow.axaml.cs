using Avalonia.Controls;
using Avalonia.Media;

namespace DemoV2;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // existing button handlers...
        StartDemoButton.Click += (_, _) =>
        {
            StartScreen.IsVisible = false;
            ExampleScreen.IsVisible = true;
        };

        SubmitExampleButton.Click += (_, _) =>
        {
            // does nothing for now
        };

        // set up placeholders for the two text boxes
        SetupPlaceholder(InputBefore, "input text");
        SetupPlaceholder(InputAfter, "transformed input text");
    }

    private void SetupPlaceholder(TextBox box, string placeholder)
    {
        var grey = new SolidColorBrush(Color.Parse("#999999"));
        var darkGrey = new SolidColorBrush(Color.Parse("#181818"));

        box.Text = placeholder;
        box.Foreground = grey;

        box.GotFocus += (_, _) =>
        {
            if (box.Text == placeholder)
            {
                box.Text = "";
                box.Foreground = darkGrey;
            }
        };

        box.LostFocus += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                box.Text = placeholder;
                box.Foreground = grey;
            }
        };
    }
}