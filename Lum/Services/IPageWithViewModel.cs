namespace Lum.Services
{
    internal interface IPageWithViewModel<TViewModel>
    {
        TViewModel ViewModel { get; set; }
        void UpdateBindings();
    }
}