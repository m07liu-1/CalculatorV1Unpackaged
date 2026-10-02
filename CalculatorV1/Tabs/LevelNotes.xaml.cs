namespace CalculatorV1.Tabs;

public partial class LevelNotes : ContentPage
{
	
	public LevelNotes()
	{
		InitializeComponent();
	}

	internal void receiveData(string s)
	{
		receiver.Text = "The average of the numbers is: " + s;
	}
}