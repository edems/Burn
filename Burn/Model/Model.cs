using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Ports;

namespace Burn.Model
{
    class Model : INotifyPropertyChanged
    {    //6.59189745937285E-07;
        //5,00495918211643E-07
        const  double PIC_timer_dauer = 5.00495918211643E-07;
        private PlotModel plotModel;
        private List<Tabelle_dreh> werte_tabelle;
        private bool addPointbool = false;
        private string comname = "";
        private bool removePointbool = false;
        public string Comname
        {
            get { return comname; }
            set { comname = value; OnPropertyChanged("Comname"); }
        }
        public bool AddPointbool
        {
            get { return addPointbool;}
            set { addPointbool = value; OnPropertyChanged("AddPointbool");}
        }
        public bool RemovePointbool
        {
            get { return removePointbool; }
            set { removePointbool = value; OnPropertyChanged("RemovePointbool"); }
        }
        private LinearAxis linearAxisX;
        public LinearAxis LinearAxisX
        {
            get { return linearAxisX;}
            set { linearAxisX = value; OnPropertyChanged("LinearAxisX");}
        }
        private LinearAxis linearAxisY;
        public LinearAxis LinearAxisY
        {
            get { return linearAxisY; }
            set { linearAxisY = value; OnPropertyChanged("LinearAxisY"); }
        }
        private LineSeries kurbelwelle;
        public LineSeries Kurbelwelle
        {
            get { return kurbelwelle; }
            set { kurbelwelle = value; OnPropertyChanged("Kurbelwelle"); }
        }
        private int indexOfPointToMove = -1;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }
        

        public Model()
        {            
            PlotModel = new PlotModel();            
            linearAxisX = new LinearAxis(AxisPosition.Bottom, 500, 8000,500,100);
            linearAxisX.Title = "\nKurbelwelle U/min";
            linearAxisX.TitleFontSize = 16;
            linearAxisX.TitleFontWeight = 500;
            linearAxisX.IsZoomEnabled = false;
            linearAxisX.IsPanEnabled = false;
            linearAxisY = new LinearAxis(AxisPosition.Left, 0, 20, 5, 1);
            linearAxisY.Title = "Verzögerungs Grad\n\n";
            linearAxisY.TitleFontWeight = 500;
            linearAxisY.TitleFontSize = 16;
            linearAxisY.IsZoomEnabled = false;
            linearAxisY.IsPanEnabled = false;
            PlotModel.MouseDown += new EventHandler<OxyPlot.OxyMouseEventArgs>(plotModel_MouseDown);
            PlotModel.MouseMove += new EventHandler<OxyPlot.OxyMouseEventArgs>(plotModel_MouseMove);
            PlotModel.MouseUp += new EventHandler<OxyPlot.OxyMouseEventArgs>(plotModel_Mouseup);
            kurbelwelle = new LineSeries();
            werte_tabelle = new List<Tabelle_dreh>();
            SetUpModel();        
            LoadData();            
        }

        private void plotModel_MouseMove(object sender, OxyMouseEventArgs e)
        {
            try
            {
                if (addPointbool == false)
                {
                    if (indexOfPointToMove >= 0)
                    {
                        // Move the point being edited.
                        kurbelwelle.Points[indexOfPointToMove] = kurbelwelle.InverseTransform(e.Position);
                        PlotModel.RefreshPlot(true);
                    }
                }
            }
            catch { }
        }

        private void plotModel_Mouseup(object sender, OxyMouseEventArgs e)
        {
            if (addPointbool == false)
            {
                indexOfPointToMove = -1;
                kurbelwelle.LineStyle = LineStyle.Solid;
            }
        }

        void plotModel_MouseDown(object sender, OxyPlot.OxyMouseEventArgs e)
        {
            if (addPointbool == false && removePointbool == false)
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    Series series = PlotModel.GetSeriesFromPoint(e.Position, 10);
                    if (series != null)
                    {
                        OxyPlot.TrackerHitResult result = series.GetNearestPoint(e.Position, true);
                        if (result != null && result.DataPoint != null)
                        {
                            // data point nearest to the click
                            indexOfPointToMove = (int)Math.Round(result.Index);
                            kurbelwelle.LineStyle = LineStyle.DashDot;
                        }
                    }
                }

            }
            else if( addPointbool == true)
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                        kurbelwelle.Points.Add(Axis.InverseTransform(e.Position, linearAxisX, linearAxisY));
                        if (kurbelwelle.Points.Count != 1)
                        {
                            if(kurbelwelle.Points[kurbelwelle.Points.Count-1].X < kurbelwelle.Points[kurbelwelle.Points.Count-2].X)
                            {
                                kurbelwelle.Points[kurbelwelle.Points.Count - 1].X = kurbelwelle.Points[kurbelwelle.Points.Count - 2].X + 1;
                            }
                        }
                }
                PlotModel.RefreshPlot(true);
            }
            else if (removePointbool == true)
            {
                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    Series series = PlotModel.GetSeriesFromPoint(e.Position, 10);
                    if (series != null)
                    {
                        OxyPlot.TrackerHitResult result = series.GetNearestPoint(e.Position, true);
                        if (result != null && result.DataPoint != null)
                        {
                            // data point nearest to the click
                            indexOfPointToMove = (int)Math.Round(result.Index);
                            kurbelwelle.Points.RemoveAt(indexOfPointToMove);
                            PlotModel.RefreshPlot(true);

                        }
                    }
                }
            }
            
        }

        private void SetUpModel()
        {
            plotModel.LegendSymbolLength = 40;
            plotModel.Subtitle = "Left click to edit points";
            plotModel.Title = "Zündsteurungs Control Panel";
            OxyPlot.ScreenPoint ScreenPoint = new OxyPlot.ScreenPoint();
            ScreenPoint.X=1000;
            plotModel.Axes.Add(linearAxisX);
            plotModel.Axes.Add(linearAxisY);
            kurbelwelle.Color = OxyColors.SkyBlue;
            kurbelwelle.LineStyle = LineStyle.Solid;
            kurbelwelle.MarkerFill = OxyColors.SkyBlue;
            kurbelwelle.MarkerSize = 6;
            kurbelwelle.MarkerStroke = OxyColors.White;
            kurbelwelle.MarkerStrokeThickness = 1.5;
            kurbelwelle.MarkerType = MarkerType.Circle;
            kurbelwelle.Title = "Kurbelwelle";
        }

        private void LoadData()
        {
            for (int i = 65535; i > 0; i -= 256)
            {
                Tabelle_dreh temp = new Tabelle_dreh();
                temp.Pic_welle = 60 / (i * PIC_timer_dauer);
                temp.Kurbel_welle = (temp.Pic_welle / 2);
                temp.Nocke_nwelle = (temp.Pic_welle / 4);
                temp.Periode = (60 / temp.Pic_welle);
                temp.Ein_grad = ((60 / temp.Pic_welle) / 360) * 4;
                werte_tabelle.Add(temp);
            }
            kurbelwelle.Points.Add(new DataPoint(500, 0));
            kurbelwelle.Points.Add(new DataPoint(1500, 5));
            kurbelwelle.Points.Add(new DataPoint(4000, 10));
            kurbelwelle.Points.Add(new DataPoint(6000, 15));
            plotModel.Series.Add(kurbelwelle);
            int[] werte_low = new int[8] { 2, 0, 0, 0, 0, 0, 0, 0 };
            int[] werte_high = new int[8] { 1, 0, 0, 0, 0, 0, 0, 0 }; ;

            for (int i = 7; i >= 0; --i)
            {
                if (i == 7)
                {
                    werte_low[0] = werte_high[7];

                }
                else
                {
                    werte_high[i + 1] = werte_high[i];
                    werte_low[i + 1] = werte_low[i];
                }

            }

           
        }
        public void berechnen()
        {
            if (Punkt_check() == true)
            {
                int grad_min = 0, grad_max = 0, dreh_min = 0, dreh_max = 0;
                int dreh_aktuel = 0, j = 0;
                double grad_aktuel = 0, schritweise = 0, delta_grad = 0, delta_dreh = 0;
                List<IDataPoint> temp_liste = new List<IDataPoint>();
                temp_liste = kurbelwelle.Points.ToList();
                if (temp_liste.Count != 0)
                {
                    for (int i = 0; i < temp_liste.Count; i++)
                    {
                        if (i == 0)
                        {
                            grad_min = (int)Math.Round(temp_liste.ElementAt(0).Y);
                            grad_max = (int)Math.Round(temp_liste.ElementAt(1).Y);
                            dreh_min = (int)Math.Round(temp_liste.ElementAt(0).X);
                            dreh_max = (int)Math.Round(temp_liste.ElementAt(1).X);
                            delta_grad = grad_max - grad_min;
                            delta_dreh = dreh_max - dreh_min;
                            schritweise = delta_grad / delta_dreh;
                            dreh_aktuel = dreh_min;
                            grad_aktuel = grad_min;
                        }
                        else if (i == temp_liste.Count - 1)
                        {
                            grad_min = (int)Math.Round(temp_liste.ElementAt(temp_liste.Count - 2).Y);
                            grad_max = (int)Math.Round(temp_liste.ElementAt(temp_liste.Count - 1).Y);
                            dreh_min = (int)Math.Round(temp_liste.ElementAt(temp_liste.Count - 2).X);
                            dreh_max = (int)Math.Round(temp_liste.ElementAt(temp_liste.Count - 1).X);
                            delta_grad = grad_max - grad_min;
                            delta_dreh = dreh_max - dreh_min;
                            schritweise = delta_grad / delta_dreh;
                            dreh_aktuel = dreh_min;
                            grad_aktuel = grad_min;
                        }
                        else
                        {
                            grad_min = (int)Math.Round(temp_liste.ElementAt(i).Y);
                            grad_max = (int)Math.Round(temp_liste.ElementAt(i + 1).Y);
                            dreh_min = (int)Math.Round(temp_liste.ElementAt(i).X);
                            dreh_max = (int)Math.Round(temp_liste.ElementAt(i + 1).X);
                            delta_grad = grad_max - grad_min;
                            delta_dreh = dreh_max - dreh_min;
                            dreh_aktuel = dreh_min;
                            schritweise = delta_grad / delta_dreh;
                            grad_aktuel = grad_min;
                        }
                        if (dreh_aktuel < dreh_max)
                        {
                            while (dreh_aktuel < dreh_max)
                            {

                                if (werte_tabelle.ElementAt(j).Kurbel_welle <= dreh_aktuel)
                                {
                                    werte_tabelle.ElementAt(j).Mult = (int)Math.Round(grad_aktuel);
                                    j++;
                                }
                                else
                                {
                                    dreh_aktuel++;
                                    grad_aktuel = grad_aktuel + schritweise;
                                }
                            }
                        }

                    }
                    if (j < 255)
                    {
                        while (j < 255)
                        {
                            if (werte_tabelle.ElementAt(j).Kurbel_welle <= dreh_aktuel)
                            {
                                if (grad_aktuel < 45)
                                    werte_tabelle.ElementAt(j).Mult = (int)Math.Round(grad_aktuel);
                                else
                                    werte_tabelle.ElementAt(j).Mult = (int)Math.Round(45.0);
                                j++;
                            }
                            else
                            {
                                dreh_aktuel++;
                                grad_aktuel += schritweise;
                            }
                        }
                    }
                }

                for (int k = 0; k < 255; k++)
                {
                    werte_tabelle.ElementAt(k).Dez_erg = (int)Math.Round(((werte_tabelle.ElementAt(k).Periode - (werte_tabelle.ElementAt(k).Ein_grad * werte_tabelle.ElementAt(k).Mult))) / PIC_timer_dauer);
                    werte_tabelle.ElementAt(k).Hex_erg = werte_tabelle.ElementAt(k).Dez_erg.ToString("X4");
                }
            }
        }

        private bool Punkt_check()
        {
            if (kurbelwelle.Points.Count > 1)
            {
                for (int i = 0; i < kurbelwelle.Points.Count; i++)
                {
                    if( i != kurbelwelle.Points.Count -1)
                    if (kurbelwelle.Points[i].X > kurbelwelle.Points[i + 1].X)
                    {
                        kurbelwelle.Points[i + 1].X = kurbelwelle.Points[i].X + 1;
                    }
                }
                PlotModel.RefreshPlot(true);
                return true;
            }
            else
            {
                PlotModel.RefreshPlot(true);
                return false;
            }
        }
        public void versenden()
        {
            senden_an_picmikrokontroller();
            write_hex(); 
        }
        public string[] getPortNames()
        {
            return SerialPort.GetPortNames();
        }
        void write_hex()
        {
            StringBuilder header = new StringBuilder();
            header.AppendLine("#ifndef __LOOKUP_H__");
            header.AppendLine("#define __LOOKUP_H__ ");
            header.AppendLine("#include <xc.h>");
            header.AppendLine("static volatile unsigned int lookup[256] = {");
            for (int k = 255; k >= 0 ; k--)
            {
                if (k != 0)
                {
                    if(werte_tabelle.ElementAt(k).Hex_erg == "0x0000")
                        header.AppendLine(werte_tabelle.ElementAt(k).Hex_erg + ",");
                    else
                        header.AppendLine("0x" + werte_tabelle.ElementAt(k).Hex_erg + ",");
                }
                else
                {
                    header.AppendLine("0x" + werte_tabelle.ElementAt(k).Hex_erg);
                }
            }
            header.AppendLine("};");
            header.AppendLine("#endif");
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\burnzeit.h", String.Empty);
            using (StreamWriter outfile = new StreamWriter(Directory.GetCurrentDirectory() + @"\burnzeit.h", true))
            {
                outfile.Write(header.ToString());
            }
        }
        public int senden_an_picmikrokontroller()
        {
            string str="";
            int zahl = 0,zahl2=0;            
            SerialPort port = new SerialPort(Comname, 9600, Parity.None, 8, StopBits.One); // Open the port for communications 
            port.Open();
            for (int k = 255; k >= 0; k--)
            {
                zahl2 = 0;
                port.Write("g"+werte_tabelle.ElementAt(k).Dez_erg.ToString() + "\r");
                while(true)
                {
                    zahl2++;
                    str = port.ReadExisting();
                    if (str != "")
                    {
                        break;
                    }
                    zahl++;
                    if (zahl == 10000)
                    {
                        port.Write("g" + werte_tabelle.ElementAt(k).Dez_erg.ToString() + "\r");
                        zahl = 0;
                    }
                    if (zahl2 == 1000000)
                    {
                        port.Close();
                        return -1;
                    }
                }
                str = "";
                zahl = 0;
            }
            
            port.Close();
            return 0;
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
