using LocalBuddies.Mobile.UI.Views;
using Prism.Unity;

namespace LocalBuddies.Mobile.UI
{
    public class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            NavigationService.NavigateAsync("Navigation/List");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<List>();
            Container.RegisterTypeForNavigation<Call>();
            Container.RegisterTypeForNavigation<Settings>();
            Container.RegisterTypeForNavigation<Navigation>();
        }
    }
}
