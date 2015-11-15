using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Burn.Model
{
    class Tabelle_dreh
    {
        double nocke_nwelle, pic_welle, kurbel_welle, ein_grad, mult , periode;
        int dez_erg;
        String hex_erg;
        public Tabelle_dreh()
        {
            periode = 0;
            nocke_nwelle = 0;
            pic_welle = 0;
            ein_grad = 0;
            mult = 0;
            hex_erg = "0x0000";
        }

        public double Periode
        {
            get { return periode; }
            set { periode = value; OnPropertyChanged("Periode"); }
        }
        
        public double Kurbel_welle
        {
            get { return kurbel_welle; }
            set { kurbel_welle = value; OnPropertyChanged("Kurbel_welle"); }
        }

        public String Hex_erg
        {
            get { return hex_erg; }
            set { hex_erg = value; OnPropertyChanged("Hex_erg"); }
        }
        public int Dez_erg
        {
            get { return dez_erg; }
            set { dez_erg = value; OnPropertyChanged("Dez_erg"); }
        }

        public double Nocke_nwelle
        {
            get { return nocke_nwelle; }
            set { nocke_nwelle = value; OnPropertyChanged("Nocke_nwelle"); }
        }

        public double Pic_welle
        {
            get { return pic_welle; }
            set { pic_welle = value; OnPropertyChanged("Pic_welle"); }
        }

        public double Ein_grad
        {
            get { return ein_grad; }
            set { ein_grad = value; OnPropertyChanged("Ein_grad"); }
        }

        public double Mult
        {
            get { return mult; }
            set { mult = value; OnPropertyChanged("Mult"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [Burn.Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
