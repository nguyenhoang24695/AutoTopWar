using Common.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AutoTopWar.Action;
using AutoTopWar.Constants;
using AutoTopWar.Entity;
using AutoTopWar.Job;
using AutoTopWar.Common;
using AutoTopWar.Common.Utilities;
using System.Diagnostics;

namespace AutoTopWar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeneralEntity generalEntity;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            Log.Info("Open App");
            var configFilePath = Path.Combine(Environment.CurrentDirectory, Constant.CONFIG_FILE_PATH);
            if (FileUtil.CheckExistFile(configFilePath))
            {
                generalEntity = FileUtil.LoadJsonFile<GeneralEntity>(configFilePath);
                Open_Nox_Textbox.Text = generalEntity.EmulatorPath;

            }

        }

        private void Up_Tech_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Start Up Technical");
            SyncDataGridToEmulatorList();
            List<EmulatorEntity> choosenEmulator = GetChooseEmulator();
            Stack<int> emulatorIds = new Stack<int>(choosenEmulator.Select(ces => ces.Id).ToList());
            int threadNumber = int.Parse(Thread_Textbox.Text);
            if (choosenEmulator.Count > 0)
            {

                TechnicalJob tJ = new TechnicalJob();
                var t = new Thread(() =>
                {
                    while (emulatorIds.Count > 0)
                    {

                        Parallel.For(0, threadNumber, i =>
                        {
                            int id = emulatorIds.Pop();
                            int temp = id;
                            GlobalVariants.EMULATOR_LIST.Where(em => em.Id == temp).First().Status = Status.Running;
                            SyncEmulatorListToDataGrid();
                            tJ.UpTech(temp);
                            GlobalVariants.EMULATOR_LIST.Where(em => em.Id == temp).First().Status = Status.Stop;
                            SyncEmulatorListToDataGrid();
                        });
                        WindowAction.CloseAllNoxProcess();

                    }
                });
                t.Start();
            }
        }

        private void Quit_All_Nox_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                var runningEm = GlobalVariants.EMULATOR_LIST.FindAll(em => em.Status == Status.Running);
                WindowAction.CloseAllNoxProcess();
                runningEm.ForEach(rE => rE.Status = Status.Stop);

                // Xử lý invoke luồng giao diện
                Dispatcher.Invoke(() =>
                {
                    Emulator_GridData.ItemsSource = null;
                    Emulator_GridData.ItemsSource = GlobalVariants.EMULATOR_LIST;
                });
            });
            t.Start();
        }

        private void Open_Nox_Btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "Nox.exe";
            openFileDialog.Filter = "Exe Files (.exe)|*.exe";

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var folderPath = filePath.Replace(openFileDialog.SafeFileName, "");
                generalEntity = new GeneralEntity()
                {
                    EmulatorPath = filePath,
                    VMSPath = System.IO.Path.Combine(folderPath, Constant.VMS_FOLDER_PATH)
                };
                FileUtil.SaveJsonFile<GeneralEntity>(generalEntity, System.IO.Path.Combine(Environment.CurrentDirectory, Constant.CONFIG_FILE_PATH));
                Open_Nox_Textbox.Text = filePath;

                LoadEmulatorInfo();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Info("Window_Loaded event execution started");
            if (generalEntity != null)
            {
                
                LoadEmulatorInfo();
            }
        }

        private void Open_Emulator_Btn_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariants.EMULATOR_LIST = (List<EmulatorEntity>)Emulator_GridData.ItemsSource;
            var checkedEm = GlobalVariants.EMULATOR_LIST.FindAll(em => em.Checked);

            var checedId = checkedEm.Select(em => em.Id).ToArray();
            if (checedId.Length > 0)
            {
                new Thread(() => GeneralJob.OpenEmulator(checedId)).Start();
                checkedEm.ForEach(cE => cE.Status = Status.Running);
                Emulator_GridData.ItemsSource = null;
                Emulator_GridData.ItemsSource = GlobalVariants.EMULATOR_LIST;
            }
        }
        private void Load_Emulator_IP_Button_Click(object sender, RoutedEventArgs e)
        {
            var nonIpChosenEmulator = GetNonIpChooseEmulator();
            if (nonIpChosenEmulator.Count > 0)
            {
                new Thread(() =>
                {                    
                    nonIpChosenEmulator.Select(c => c.Id).ToList().ForEach(id =>
                    {
                        GlobalVariants.EMULATOR_LIST.Where(em => em.Id == id).First().Status = Status.Running;
                        SyncEmulatorListToDataGrid();
                        AndroidAction.GetEmulatorIp(id);
                        GlobalVariants.EMULATOR_LIST.Where(em => em.Id == id).First().Status = Status.Stop;
                        SyncEmulatorListToDataGrid();
                    });
                    FileUtil.SaveJsonFile(GlobalVariants.ID_IP_LIST, Constant.IP_CONFIG_FILE_PATH);
                        SyncEmulatorListToDataGrid();

                }).Start();
            }
        }

        private void SyncDataGridToEmulatorList()
        {
            GlobalVariants.EMULATOR_LIST = (List<EmulatorEntity>)Emulator_GridData.ItemsSource;
        }

        private void SyncEmulatorListToDataGrid()
        {
            Dispatcher.Invoke(() =>
            {
                Emulator_GridData.ItemsSource = null;
                Emulator_GridData.ItemsSource = GlobalVariants.EMULATOR_LIST;

            });
        }

        private List<EmulatorEntity> GetChooseEmulator()
        {
            return GlobalVariants.EMULATOR_LIST.FindAll(em => em.Checked).ToList();
        }
        private List<EmulatorEntity> GetNonIpChooseEmulator()
        {
            return GlobalVariants.EMULATOR_LIST.FindAll(em => em.Checked && (em.Ip == null || em.Ip == string.Empty)).ToList();
        }

        private void LoadEmulatorInfo()
        {
            Log.Info("Load Emulator Info");
            DirectoryInfo info = new DirectoryInfo(generalEntity.VMSPath);
            var listVMS = info.GetDirectories().OrderBy(p => p.CreationTime).ToList();
            int count = 0;
            foreach (var vMS in listVMS)
            {
                EmulatorEntity emulatorEntity = new EmulatorEntity();
                emulatorEntity.Id = count++;
                emulatorEntity.Name = Path.GetFileName(vMS.Name);
                emulatorEntity.Checked = false;
                GlobalVariants.EMULATOR_LIST.Add(emulatorEntity);
            }
            DataUtil.SyncEmulatorData();

            Emulator_GridData.ItemsSource = null;
            Emulator_GridData.ItemsSource = GlobalVariants.EMULATOR_LIST;
            Emulator_GridData.CanUserAddRows = false;
        }

        private void Main_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
