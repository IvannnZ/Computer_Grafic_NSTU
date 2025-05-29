using Photo3DEditor.Services;
using Photo3DEditor.Views;
using System.Windows;

namespace Photo3DEditor
{
    public partial class App : Application
    {
        public static DataService DataService { get; } = new DataService();
        public static int weigth = 1000;
        public static int height = 1000;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new ImageWindow().Show();
            new DepthEditorWindow().Show();
            new Viewer3DWindow().Show(); 
        }
    }

}