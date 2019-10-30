using ImageFilters.Helpers;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using UserApplication.ViewModels;

namespace ImageFilters.ViewModel
{
    public class GaussianBlurParamViewModel : BaseViewModel, IDataErrorInfo
    {
        private DataTable dataTable = new DataTable();
        private int kernelSize;

        public ICommand OpenCommand { get { return new DelegateCommand(OnOpenCommand, CanExecuteOpenCommand); } }
        public ICommand SaveCommand { get { return new DelegateCommand(OnSaveCommand, CanExecuteSaveCommand); } }


        public GaussianBlurParamViewModel()
        {
            KernelSize = "3";
        }

        public double[,] Kernel
        {
            get => GetKernel(dataTable);
            
        }

        public DataView DataView
        {
            get
            {
                dataTable.DefaultView.AllowNew = false;
                return dataTable.DefaultView;
            }
            set
            {
                RaisePropertyChanged(() => DataView);
            }
        }

        public string KernelSize
        {
            get => kernelSize.ToString();
            set
            {
                int.TryParse(value, out kernelSize);
                if (kernelSize <= 10)
                {
                    if (string.IsNullOrEmpty(dataTable.TableName))
                        dataTable = CreateDataTable(kernelSize, null);
                    else
                        dataTable.TableName = string.Empty;
                    RaisePropertyChanged(() => KernelSize);
                    RaisePropertyChanged(() => DataView);
                }
            }
        }

        private void OnOpenCommand()
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Kernel files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if ((bool)dlg.ShowDialog())
            {
                var numbers = ReadKernelFromFile(dlg.FileName);
                dataTable = CreateDataTable(numbers.GetLength(0), numbers);
                dataTable.DefaultView.AllowNew = false;
                DataView = dataTable.DefaultView;
                dataTable.TableName = "file";
                KernelSize = dataTable.Rows.Count.ToString();
            }
        }
        private bool CanExecuteOpenCommand()
        {
            return true;
        }

        private void OnSaveCommand()
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Kernel files(*.txt) | *.txt | All Files(*.*) | *.* ",
                FileName = "kernel"
            };
            if ((bool)dlg.ShowDialog())
            {
                SaveKernelToFile(dlg.FileName);
            }
        }
        private bool CanExecuteSaveCommand()
        {
            return kernelSize > 0;
        }



        public string this[string property]
        {
            get
            {
                string result = null;
                if (property == "KernelSize")
                {
                    if (string.IsNullOrEmpty(KernelSize))
                        result = "Pole wymagane. Wprowadz Rozmiar";

                    if (kernelSize > 10 || kernelSize < 1)
                        result = "Wartość musi być z przedział <1,10>";

                }

                return result;
            }

        }
        public string Error
        {
            get
            {
                return string.Empty;
            }


        }

        private DataTable CreateDataTable(int size, double[,] array)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < size; i++)
            {
                dataTable.Columns.Add();
            }

            for (int i = 0; i < size; i++)
            {
                DataRow row = dataTable.NewRow();
                for (int j = 0; j < size; j++)
                {
                    row[j] = array != null ? string.Format("{0:0.0}", array[i, j]) : string.Format("{0:0.0}", 1.0);
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
        private double[,] GetKernel(DataTable dataTable)
        {
            double[,] result = new double[dataTable.Rows.Count, dataTable.Columns.Count];

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    double.TryParse(dataTable.Rows[i][j].ToString(), out result[i, j]);
                }
            }

            return result;
        }
        private double[,] ReadKernelFromFile(string filePath)
        {
            double[,] kernelArray = null;
            int i = 0;
            try
            {
                var lines = File.ReadAllLines(filePath);
                kernelArray = new double[lines.Length, lines.Length];
                foreach (var row in lines)
                {
                    int j = 0;
                    foreach (var col in row.Trim().Split(' '))
                    {
                        double.TryParse(col.Trim(), out kernelArray[i, j]);
                        j++;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot read kernel file or kernel data are incorrect  {ex.Message}");
            }

            return kernelArray;
        }
        private void SaveKernelToFile(string filePath)
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        sw.Write(dataTable.Rows[i][j].ToString());
                        sw.Write(" ");
                    }
                    sw.WriteLine();
                }

            }
            
        }







    }
}
