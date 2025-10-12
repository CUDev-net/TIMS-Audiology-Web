namespace TIMS_X.Server.Models;

public class CheckedItem<T>
{
	public CheckedItem(T model)
	{
		Model = model;
	}

	public bool IsChecked { get; set; }

	public T Model { get; set; }
}