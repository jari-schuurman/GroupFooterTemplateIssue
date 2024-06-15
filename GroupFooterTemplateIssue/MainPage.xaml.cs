using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using GroupFooterTemplateIssue.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GroupFooterTemplateIssue
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public CleanObservableCollection<AnimalGroup> Animals { get; private set; } = new CleanObservableCollection<AnimalGroup>();
        public MainPage()
        {
            InitializeComponent();
            Animals.Add(new AnimalGroup("Bears", new ObservableCollection<IItem>
            {
                new Animal
                {
                    Name = "American Black Bear",
                    Location = "North America",
                    Details = "Details about the bear go here.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/0/08/01_Schwarzbär.jpg"
                },
                new Animal
                {
                    Name = "Asian Black Bear",
                    Location = "Asia",
                    Details = "Details about the bear go here.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b7/Ursus_thibetanus_3_%28Wroclaw_zoo%29.JPG/180px-Ursus_thibetanus_3_%28Wroclaw_zoo%29.JPG"
                },
                // ...
            }));
            Animals.Add(new AnimalGroup("Monkeys", new ObservableCollection<IItem>
            {
                new Animal
                {
                    Name = "Baboon",
                    Location = "Africa & Asia",
                    Details = "Details about the monkey go here.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg"
                },
                new Animal
                {
                    Name = "Capuchin Monkey",
                    Location = "Central & South America",
                    Details = "Details about the monkey go here.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg"
                },
                new Animal
                {
                    Name = "Blue Monkey",
                    Location = "Central and East Africa",
                    Details = "Details about the monkey go here.",
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/83/BlueMonkey.jpg/220px-BlueMonkey.jpg"
                },
                // ...
            }));
            collection.ItemsSource = Animals;
        }

        private void OpenPopup(object sender, EventArgs e)
        {
            var popup = new Popup()
            {
                Content = new CollectionView
                {
                    ItemsSource = Animals,
                    IsGrouped = true,
                    GroupHeaderTemplate = new DataTemplate(() =>
                    {
                        var label = new Label();
                        label.SetBinding(Label.TextProperty, "Name");
                        return label;
                    }),
                    ItemTemplate = new DataTemplate(() =>
                    {
                        var label = new Label();
                        label.SetBinding(Label.TextProperty, "Name");
                        return label;
                    }),
                },
                Size = new Size(500),
            };

            popup.Closed += (s, e) =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                memory.Text = Process.GetCurrentProcess().WorkingSet64.ToString();
            };

            this.ShowPopup(popup);
        }

        private void OpenCustom(object sender, EventArgs e)
        {
            var popup = new Popup()
            {
                Content = new GroupedCollection
                {
                    ItemsSource = Animals,
                    IsGrouped = true,
                    GroupHeaderTemplate = new DataTemplate(() =>
                    {
                        var label = new Label();
                        label.SetBinding(Label.TextProperty, "Name");
                        return label;
                    }),
                    ItemTemplate = new DataTemplate(() =>
                    {
                        var label = new Label();
                        label.SetBinding(Label.TextProperty, "Name");
                        return label;
                    }),
                },
                Size = new Size(500),
            };

            popup.Closed += (s, e) =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                memory.Text = Process.GetCurrentProcess().WorkingSet64.ToString();
            };

            this.ShowPopup(popup);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            // this will never trigger
            if (sender is Button button)
            {
                button.BackgroundColor = Colors.Red;
            }
        }
    }

    // 1st workaround
    // This interface is needed because of https://github.com/dotnet/maui/blob/c8ce71e7730e01498756f2764a3f9f32e2c21236/src/Controls/src/Core/Platform/Windows/CollectionView/GroupTemplateContext.cs#L25
    // It will try to add a fake item to the collection. However, since it is an observablecollection, the item that is being added to the list will also be added to the source:
    // https://github.com/dotnet/maui/blob/c8ce71e7730e01498756f2764a3f9f32e2c21236/src/Controls/src/Core/Platform/Windows/CollectionView/ObservableItemTemplateCollection.cs#L81
    // Without a shared interface, youll get an exception that you cannod add object of type T to the collection.
    public interface IItem
    {

    }

    public class AnimalGroup : ObservableCollection<IItem>, IItem
    {
        public string Name { get; private set; }

        public AnimalGroup(string name, ObservableCollection<IItem> animals) : base(animals)
        {
            Name = name;
        }

        // 2nd workaround:
        // The `AddToSource` mentioned above is still triggered, so the item still gets added to its own collection
        // This results in a loop where it is adding itself all the time
        // Override the InsertItem method to ignore itself
        // Comment this out to see a funny result
        protected override void InsertItem(int index, IItem item)
        {
            if (item != this)
                base.InsertItem(index, item);
        }
    }

    public partial class Animal : ObservableObject, IItem
    {
        [ObservableProperty]
        string? name;

        [ObservableProperty]
        string? location;

        [ObservableProperty]
        string? details;

        [ObservableProperty]
        string? imageUrl;
    }
}
