namespace MasterMES.CLASSES
{
    public interface IButtonAction
    {
        void InitButtonClick();
        void ViewButtonClick();
        void NewButtonClick();
        void CopyButtonClick();
        void SaveButtonClick();
        void DeleteButtonClick();
        void PrintButtonClick(int reportIndex);
    }
}
