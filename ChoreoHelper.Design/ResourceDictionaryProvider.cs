using System.Windows;

namespace ChoreoHelper.Design;

public sealed class ResourceDictionaryProvider : ResourceDictionary
{
    private static readonly Lazy<ResourceDictionary[]> ResourceDictionaryFactory = new(() =>
    [
        new()
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/materialdesigntheme.light.xaml")
        },
        new()
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/materialdesign3.defaults.xaml")
        },
        new()
        {
            Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/primary/materialdesigncolor.bluegrey.xaml")
        },
        new()
        {
            Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/secondary/materialdesigncolor.deeppurple.xaml")
        }
    ]);

    public ResourceDictionaryProvider()
    {
        foreach (var dict in ResourceDictionaryFactory.Value)
        {
            MergedDictionaries.Add(dict);
        }
    }
}