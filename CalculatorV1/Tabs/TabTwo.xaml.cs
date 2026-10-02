namespace CalculatorV1.Tabs;

public partial class TabTwo : ContentPage {
    public TabTwo()
    {
        this.InitializeComponent();
    }

    private void transferData(Object sender, EventArgs e)
    {
        if (decimal.TryParse(Data1.Text, out decimal num1) && decimal.TryParse(Data2.Text, out decimal num2))
        {
            var mainPage = this.Parent as TabbedPage;
            foreach (var pg in mainPage.Children)
            {
                if (pg is LevelNotes)
                {
                    ((LevelNotes)pg).receiveData(((num1 + num2) / 2).ToString());
                    mainPage.CurrentPage = pg;
                    return;
                }
            }
            var newPage = new LevelNotes();
            mainPage.Children.Add(newPage);
            newPage.receiveData(((num1 + num2)/2).ToString());
            mainPage.CurrentPage = newPage;
        }
        else
        {
            message.TextColor = Colors.Red;
            message.Text = "Warning: Invalid Number Entered!";
        }
    }
}