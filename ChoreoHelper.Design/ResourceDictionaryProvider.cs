using System.Windows;

namespace ChoreoHelper.Design;

public class ResourceDictionaryProvider : ResourceDictionary
{
    public ResourceDictionaryProvider()
    {
        MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/materialdesigntheme.light.xaml")
        });
        MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/materialdesign3.defaults.xaml")
        });
        MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/primary/materialdesigncolor.bluegrey.xaml")
        });
        MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/secondary/materialdesigncolor.deeppurple.xaml")
        });
    }
}