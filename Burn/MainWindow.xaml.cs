using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Burn
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model.Model model;
        bool versenden_erfolgreich;
        public MainWindow()
        {
            versenden_erfolgreich = true;
            model = new Model.Model();
            DataContext = model;
            InitializeComponent();
        }
        public delegate void LongTimeTask_Delegate();
        LongTimeTask_Delegate senden_delegate = null; 
        IAsyncResult ruckruf_delegate = null;

        public void senden_an_pic()
        {
            if(model.senden_an_picmikrokontroller() == 0)
                versenden_erfolgreich = true;
            else
                versenden_erfolgreich = false;
        }
                  
        public void TaskCompleted(IAsyncResult R)
         {
             Dispatcher.Invoke(new Action(delegate()
                    {
                        if (versenden_erfolgreich == true)
                            Info_txt_box.Text = "Versenden erfolgreich abgeschlossen";
                        else
                            Info_txt_box.Text = "Beim Senden ist ein Fehler aufgetreten";
                    }));
         }
            

        private void berechnen_bt_Click(object sender, RoutedEventArgs e)
        {
            model.berechnen();
            Info_txt_box.Text = "";
        }

        private void add_bt_Click(object sender, RoutedEventArgs e)
        {
                model.AddPointbool = true;
                model.RemovePointbool = false;
                Remove_bt.IsEnabled = true;
                add_bt.IsEnabled = false;
                Move_bt.IsEnabled = true;
                Info_txt_box.Text = "";
        }

        private void Move_bt_Click(object sender, RoutedEventArgs e)
        {
                model.AddPointbool = false;
                model.RemovePointbool = false;
                Remove_bt.IsEnabled = true;
                add_bt.IsEnabled = true;
                Move_bt.IsEnabled = false;
                Info_txt_box.Text = "";
        }

        private void Remove_bt_Click(object sender, RoutedEventArgs e)
        {
            Remove_bt.IsEnabled = false;
            add_bt.IsEnabled = true;
            Move_bt.IsEnabled = true;
            model.AddPointbool = false;
            model.RemovePointbool = true;
            Info_txt_box.Text = "";
        }

        private void run_bt_Click(object sender, RoutedEventArgs e)
        {
            model.berechnen();
            COMListbox.ItemsSource = model.getPortNames();
            sendengrid.Visibility = Visibility.Visible;
            Plot1.Visibility = Visibility.Hidden;
            Info_txt_box.Text = "Wählen Sie Ihres COM Port aus";
        }

        private void ok_bt_Click(object sender, RoutedEventArgs e)
        {
            model.Comname = (string)COMListbox.SelectedValue;
            if (model.Comname != null)
            {
                sendengrid.Visibility = Visibility.Hidden;
                Plot1.Visibility = Visibility.Visible;
                senden_delegate = new LongTimeTask_Delegate(senden_an_pic);
                ruckruf_delegate = senden_delegate.BeginInvoke(new AsyncCallback(TaskCompleted), null);
                senden_delegate.EndInvoke(ruckruf_delegate);
                //model.versenden();
            }
            else
            {
                Info_txt_box.Text = "Das COM Port wurde nicht ausgewählt";
            }
        }

        private void cancel_bt_Click(object sender, RoutedEventArgs e)
        {
            sendengrid.Visibility = Visibility.Hidden;
            Plot1.Visibility = Visibility.Visible;
            Info_txt_box.Text = "";
        }
    }
}
