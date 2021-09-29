using AutoTopWar.Action;
using AutoTopWar.Common;
using AutoTopWar.Common.Utilities;
using AutoTopWar.Constants;
using AutoTopWar.Entity;
using AutoTopWar.Job;
using Common.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AutoTopWar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MainWindow));

        private static Thread realtime_Thread;

        private static Thread DFThread;
        private static bool isRunDF = false;

        public MainWindow()
        {
            InitializeComponent();
            Log.Info("Open App");
            var configFilePath = Path.Combine(Environment.CurrentDirectory, Constant.CONFIG_FILE_PATH);
            if (FileUtil.CheckExistFile(configFilePath))
            {
                GlobalVariants.GENERAL_CONFIG = FileUtil.LoadJsonFile<GeneralEntity>(configFilePath);
                Open_Nox_Textbox.Text = GlobalVariants.GENERAL_CONFIG.EmulatorPath;
            }
        }

        #region Event WPF

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
                Thread t = new Thread(() =>
                {
                    ReadOnlyDataGrid();
                    CheckEmulatorRunning(true);
                    int count = 0;

                    while (true)
                    {
                        if (emulatorIds.Count > 0)
                        {

                            if (count < threadNumber)
                            {
                                count++;
                                new Thread(() =>
                                {

                                    int id = emulatorIds.Pop();
                                    int temp = id;
                                    var emulator = GlobalVariants.EMULATOR_LIST.Where(em => em.Id == temp).First();
                                    emulator.Status = Status.Connected;
                                    emulator.Job = Constant.UP_TECH_JOB;
                                    SyncEmulatorListToDataGrid();
                                    tJ.UpTech(temp);
                                    // Sau khi chạy xong thì kill emulator task bằng pid
                                    WindowAction.CloseNoxWithPid(temp);
                                    emulator.Status = Status.Disconnected;
                                    emulator.Job = string.Empty;
                                    SyncEmulatorListToDataGrid();
                                    count--;

                                }).Start();
                            }
                        }
                        if (count == 0)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    StopCheckEmulatorRunning();
                    EditableDataGrid();
                });
                t.Start();
            }
        }

        private void Rally_Btn_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Start Rally");
            SyncDataGridToEmulatorList();
            List<EmulatorEntity> choosenEmulator = GetChooseEmulator();
            Stack<int> emulatorIds = new Stack<int>(choosenEmulator.Select(ces => ces.Id).ToList());
            int threadNumber = int.Parse(Thread_Textbox.Text);
            if (choosenEmulator.Count > 0)
            {
                int level = int.Parse(Rally_Level_Textbox.Text);
                int type = int.Parse(Rally_Type_Textbox.Text);
                WorldJob wJ = new WorldJob();
                var t = new Thread(() =>
                {
                    ReadOnlyDataGrid();
                    CheckEmulatorRunning(true);
                    int count = 0;
                    while (true)
                    {
                        if (emulatorIds.Count > 0)
                        {

                            if (count < threadNumber)
                            {
                                count++;
                                new Thread(() =>
                                {
                                    int id = emulatorIds.Pop();
                                    int temp = id;
                                    var emulator = GlobalVariants.EMULATOR_LIST.Where(em => em.Id == temp).First();
                                    emulator.Status = Status.Connected;
                                    emulator.Job = Constant.RALLY_JOB;
                                    SyncEmulatorListToDataGrid();
                                    wJ.Rally(temp, level, type);
                                    // Sau khi chạy xong thì kill emulator task bằng pid
                                    WindowAction.CloseNoxWithPid(temp);
                                    emulator.Status = Status.Disconnected;
                                    emulator.Job = string.Empty;
                                    SyncEmulatorListToDataGrid();
                                    count--;
                                }).Start();
                            }

                        }
                        if (count == 0)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    StopCheckEmulatorRunning();
                    EditableDataGrid();
                });
                t.Start();
            }
        }

        private void Refresh_Emulator_Table_Button_Click(object sender, RoutedEventArgs e)
        {
            CheckEmulatorRunning(false);
        }

        private void Main_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
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
                        GlobalVariants.EMULATOR_LIST.Where(em => em.Id == id).First().Status = Status.Connected;
                        SyncEmulatorListToDataGrid();
                        AndroidAction.GetEmulatorIp(id);
                        GlobalVariants.EMULATOR_LIST.Where(em => em.Id == id).First().Status = Status.Disconnected;
                        SyncEmulatorListToDataGrid();
                    });
                    FileUtil.SaveJsonFile(GlobalVariants.ID_IP_LIST, Constant.IP_CONFIG_FILE_PATH);
                    SyncEmulatorListToDataGrid();
                }).Start();
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
                checkedEm.ForEach(cE => cE.Status = Status.Connected);
                Emulator_GridData.ItemsSource = null;
                Emulator_GridData.ItemsSource = GlobalVariants.EMULATOR_LIST;
            }
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
                GlobalVariants.GENERAL_CONFIG = new GeneralEntity()
                {
                    EmulatorPath = filePath,
                    VMSPath = System.IO.Path.Combine(folderPath, Constant.VMS_FOLDER_PATH)
                };
                FileUtil.SaveJsonFile<GeneralEntity>(GlobalVariants.GENERAL_CONFIG, System.IO.Path.Combine(Environment.CurrentDirectory, Constant.CONFIG_FILE_PATH));
                Open_Nox_Textbox.Text = filePath;

                LoadEmulatorInfo();
            }
        }

        private void Quit_All_Nox_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                var runningEm = GlobalVariants.EMULATOR_LIST.FindAll(em => em.Status == Status.Connected);
                WindowAction.CloseAllNoxProcess();
                runningEm.ForEach(rE => rE.Status = Status.Disconnected);

                // Xử lý invoke luồng giao diện
                Dispatcher.Invoke(() =>
                {
                    Emulator_GridData.ItemsSource = null;
                    Emulator_GridData.ItemsSource = GlobalVariants.EMULATOR_LIST;
                });
            });
            t.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Info("Window_Loaded event execution started");
            if (GlobalVariants.GENERAL_CONFIG != null)
            {
                LoadEmulatorInfo();
            }
            CheckEmulatorRunning(false);
        }

        #endregion Event WPF

        #region Method

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
            try
            {
                Log.Info("Load Emulator Info");
                DirectoryInfo info = new DirectoryInfo(GlobalVariants.GENERAL_CONFIG.VMSPath);
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
            }
            catch (Exception)
            {

            }
            finally
            {
                Emulator_GridData.ItemsSource = null;
                Emulator_GridData.ItemsSource = GlobalVariants.EMULATOR_LIST;
                Emulator_GridData.CanUserAddRows = false;

            }

        }

        private void CheckEmulatorRunning(bool isRealTime)
        {
            realtime_Thread = new Thread(() =>
            {
                do
                {
                    SyncDataGridToEmulatorList();
                    var listDevice = AndroidAction.getDeviceList();
                    GlobalVariants.EMULATOR_LIST.ForEach(em => em.Status = Status.Disconnected);
                    GlobalVariants.EMULATOR_LIST.Where(em => listDevice.Contains(em.Ip)).ToList().ForEach(em => em.Status = Status.Connected);

                    Dispatcher.Invoke(() =>
                    {
                        SyncEmulatorListToDataGrid();
                    });
                    if (isRealTime)
                    {
                        Thread.Sleep(1000);
                    }
                } while (isRealTime);
            });

            realtime_Thread.Start();
        }

        private void StopCheckEmulatorRunning()
        {
            if (realtime_Thread.IsAlive)
            {
                realtime_Thread.Abort();
            }
        }

        private void ReadOnlyDataGrid()
        {
            Log.Info("Set DateGrid is readOnly for realtime");
            Dispatcher.Invoke(() =>
            {
                Emulator_GridData.IsReadOnly = true;
            });
        }

        private void EditableDataGrid()
        {
            Log.Info("Enable dataGrid");
            Dispatcher.Invoke(() =>
            {
                Emulator_GridData.IsReadOnly = false;
            });
        }

        private void SyncDataGridToEmulatorList()
        {
            GlobalVariants.EMULATOR_LIST = (List<EmulatorEntity>)Emulator_GridData.ItemsSource;
        }

        #endregion Method

        private void Select_All_Emulator_Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariants.EMULATOR_LIST.ForEach(em => em.Checked = true);
            SyncEmulatorListToDataGrid();
        }

        private void Deselect_All_Emulator_Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariants.EMULATOR_LIST.ForEach(em => em.Checked = false);
            SyncEmulatorListToDataGrid();
        }

        private void Log_Button_Click(object sender, RoutedEventArgs e)
        {
            Action.AndroidAction.ExecuteCommand(string.Format(Constant.OPEN_LOG_FILE_CMD, DateTime.Now.ToString("dd-MM-yyyy")));
        }

        private void Test_Button_Click(object sender, RoutedEventArgs e)
        {
            var rs = KAutoHelper.NoxMultiIni.GetNoxMultiIni();
            MessageBox.Show(rs.ToString());
        }

        private void Run_Dark_Force_Click(object sender, RoutedEventArgs e)
        {
            isRunDF = !isRunDF;
            if (isRunDF)
            {
                this.Status_TextBlock.Text = "Running";
                //string deviceID = "LGH8734d476d4b";
                string deviceID = KAutoHelper.ADBHelper.GetDevices()[0];

                DFThread = new Thread(() =>
                {
                    while (true)
                    {
                        Dispatcher.Invoke(() =>
                            {
                                this.Status_Queue_TextBlock.Text = "";

                            });

                        if (AndroidAction.ExistImageInstant(deviceID, "pic/search/overArmy"))
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 87.6, 35.2);
                            continue;
                        }

                        if (AndroidAction.ExistImageInstant(deviceID, "pic/world_queue"))
                        {
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 12.7, 91.6);
                            Thread.Sleep(1000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.3, 71.6);
                            Thread.Sleep(2000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.0, 46.3);
                            Thread.Sleep(1000);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.0, 46.3);
                            Thread.Sleep(1000);
                            Dispatcher.Invoke(() =>
                            {
                                if (DarkForce_RB.IsChecked.Value)
                                {
                                    KAutoHelper.ADBHelper.TapByPercent(deviceID, 40.9, 39.4);

                                }
                                else
                                {
                                    //KAutoHelper.ADBHelper.TapByPercent(deviceID, 70.7, 39.2);
                                    AndroidAction.ClickImageInstant(deviceID, "pic/search/new_rally");

                                }

                            });

                            Thread.Sleep(2000);
                            if (!AndroidAction.ExistImageInstant(deviceID, "pic/search/battle"))
                            {
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 75.7, 52.9);
                                Thread.Sleep(1000);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 49.7, 66.7);
                                Thread.Sleep(1000);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 91.0, 33.3);
                                Thread.Sleep(1000);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.0, 46.3);
                                Thread.Sleep(1000);
                                KAutoHelper.ADBHelper.TapByPercent(deviceID, 29.3, 37.9);
                                Thread.Sleep(2000);
                            }

                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 45.6, 82.1);
                            Thread.Sleep(500);
                            KAutoHelper.ADBHelper.TapByPercent(deviceID, 50.7, 42.3);
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {

                                this.Status_Queue_TextBlock.Text = "Full";
                            });
                            Thread.Sleep(2000);
                        }
                    }
                });
                DFThread.Name = "DF_Thread";
                DFThread.Start();
            }
            else
            {
                this.Status_TextBlock.Text = "Not Running";
                DFThread.Abort();
            }


        }

        private void Main_Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
    }
}