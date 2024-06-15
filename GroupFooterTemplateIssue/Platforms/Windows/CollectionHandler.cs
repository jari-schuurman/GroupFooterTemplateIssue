using GroupFooterTemplateIssue.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.UI.Xaml.Controls;
using System.Collections;
using System.Reflection;

namespace GroupFooterTemplateIssue.Platforms.Windows
{
    public partial class CollectionHandler : CollectionViewHandler
    {
        // this method causes memory leak
        // https://github.com/dotnet/maui/issues/22954
        // the internal event handler of https://github.com/dotnet/maui/blob/b182ffef2c85a5681686732a81b0f572a86591cc/src/Controls/src/Core/Platform/Windows/CollectionView/GroupedItemTemplateCollection.cs#L56
        private const string GroupsChangedMethodName = "GroupsChanged";
        protected override void UpdateItemsSource()
        {
            base.UpdateItemsSource();
        }

        protected override void DisconnectHandler(ListViewBase platformView)
        {
            Clean();
            base.DisconnectHandler(platformView);
        }

        protected override void CleanUpCollectionViewSource()
        {
            Clean();
            base.CleanUpCollectionViewSource();
        }

        private FieldInfo fieldInfo = null;

        private void Clean()
        {
            if (Element.ItemsSource is ICleanObservableCollection collection)
            {
                collection.CleanUp(GroupsChangedMethodName);
            }
            else if (CollectionViewSource is not null)
            {
                dynamic observableItemTemplateCollection = CollectionViewSource.Source;
                if (observableItemTemplateCollection is IList list)
                {
                    //Get the type of the class
                    Type type = observableItemTemplateCollection.GetType();

                    // Get the private field information
                    fieldInfo ??= type.GetField("_itemsSource", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        // Get the value of the private field
                        object fieldValue = fieldInfo.GetValue(observableItemTemplateCollection);
                        if (fieldValue is ICleanObservableCollection source)
                        {
                            source.CleanUp(GroupsChangedMethodName);
                        }
                    }
                }
            }
        }
    }
}
