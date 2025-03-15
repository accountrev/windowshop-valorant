using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Windows.Threading;
using Windows.ApplicationModel.VoiceCommands;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using System.Threading.Tasks;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Orientation = System.Windows.Controls.Orientation;
using FontFamily = System.Windows.Media.FontFamily;
using LibVLCSharp.Shared;

namespace Windowshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WPFGlobalsBridge viewModel;

        private LibVLC _libVLC;
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new WPFGlobalsBridge();
            DataContext = viewModel;

            StoreGrid.Visibility = Visibility.Visible;
            ItemViewGrid.Visibility = Visibility.Hidden;
            VideoGrid.Visibility = Visibility.Hidden;
            VideoControlsPanel.Visibility = Visibility.Hidden;

            #region Tray Icon Initialization
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new Icon(@"Resources\windowshop-preview.ico");
            ni.Visible = true;
            ni.Text = "Windowshop";





            ni.MouseClick += (object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ShowWindow();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    ni.ContextMenuStrip.Show();
                }
            };



            ni.ContextMenuStrip = new ContextMenuStrip();
            ni.ContextMenuStrip.Items.Add("Open Windowshop UI", null, delegate (object sender, EventArgs args)
            {
                ShowWindow();
            });
            ni.ContextMenuStrip.Items.Add("Quit", null, Icon_Exit);
            #endregion

            // LibVLCSharp initialization
            Core.Initialize();

            _libVLC = new LibVLC("--input-repeat=2");
            _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC);

            // Set the VideoView's MediaPlayer
            VideoView.MediaPlayer = _mediaPlayer;
        }


        // Minimize to system tray when application is minimized.
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized) Hide();

            

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            Hide();

            ShowMinimizedNoti();

            base.OnClosing(e);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            System.Windows.MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        private void Icon_Exit(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public WPFGlobalsBridge GetViewModel() => viewModel;

        public void UpdateGradients()
        {
            Item1Gradient.GradientStops.Clear();
            Item2Gradient.GradientStops.Clear();
            Item3Gradient.GradientStops.Clear();
            Item4Gradient.GradientStops.Clear();
            
            Item1Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00" + WindowshopGlobals.mainShopItems[0]["rarityColor"].ToString()), 0.3));
            Item1Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF" + WindowshopGlobals.mainShopItems[0]["rarityColor"].ToString()), 1.0));

            Item2Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00" + WindowshopGlobals.mainShopItems[1]["rarityColor"].ToString()), 0.3));
            Item2Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF" + WindowshopGlobals.mainShopItems[1]["rarityColor"].ToString()), 1.0));

            Item3Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00" + WindowshopGlobals.mainShopItems[2]["rarityColor"].ToString()), 0.3));
            Item3Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF" + WindowshopGlobals.mainShopItems[2]["rarityColor"].ToString()), 1.0));

            Item4Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00" + WindowshopGlobals.mainShopItems[3]["rarityColor"].ToString()), 0.3));
            Item4Gradient.GradientStops.Add(new GradientStop((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF" + WindowshopGlobals.mainShopItems[3]["rarityColor"].ToString()), 1.0));
        }

        public void ShowWindow()
        {
            if (Dispatcher.CheckAccess())
            {
                Show();
                WindowState = System.Windows.WindowState.Normal;
            }
            else
            {
                Dispatcher.Invoke(() => ShowWindow());
            }
        }

        private void ShowMinimizedNoti()
        {
            if (!WindowshopGlobals.alreadyShownMinimizedNoti)
            {
                new ToastContentBuilder()
                .AddText("Windowshop is now minimized")
                .AddText("Windowshop is now hidden in your taskbar and waiting for the next shop rotation. Click it to open the GUI again or right-click for options.")
                .Show();

                WindowshopGlobals.alreadyShownMinimizedNoti = true;
            }
        }

        private void EnterItemViewGrid(object sender, RoutedEventArgs e)
        {
            if (WindowshopGlobals.menuLoaded)
            {
                ItemViewerManager.Reset();

                var button = sender as System.Windows.Controls.Button;
                if (button != null)
                {
                    // Retrieve additional arguments from the Tag property
                    var argument = int.Parse(button.Tag as string);

                    WindowshopGlobals.itemSelected = argument;

                    ChromaList.Children.Clear();
                    LevelList.Children.Clear();

                    if (Util.AcquireChromaCountOfSkin(WindowshopGlobals.rawShopItems[WindowshopGlobals.itemSelected]) > 1)
                    {
                        for (int i =0; i < Util.AcquireChromaCountOfSkin(WindowshopGlobals.rawShopItems[WindowshopGlobals.itemSelected]); i++)
                        {
                            // Create a new Button
                            Button chromaButton = new Button
                            {
                                Width = 44,
                                Height = 44,
                                Margin = new Thickness(0, 0, 12, 0),
                                Content = new Image
                                {
                                    Source = WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["chromas"][i]
                                },
                            
                            };

                            chromaButton.Click += SelectItemChroma;
                            chromaButton.Tag = i.ToString();

                            // Add the button to the ChromaList StackPanel
                            ChromaList.Children.Add(chromaButton);
                        }
                    }

                    for (int i = 0; i < Util.AcquireLevelsCountOfSkin(WindowshopGlobals.rawShopItems[WindowshopGlobals.itemSelected]); i++)
                    {
                        Button levelButton = new Button
                        {
                            Width = 240,
                            Height = 50,
                            Background = Brushes.Transparent,
                            BorderThickness = new Thickness(0),
                            VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch,
                            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch
                        };

                        Border levelButtonBorder = new Border
                        {
                            Background = new SolidColorBrush(Color.FromArgb(96, 87, 87, 87))
                        };

                        StackPanel textStackPanel = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(8, 0, 0, 0)
                        };

                        TextBlock levelIndexText = new TextBlock
                        {
                            Foreground = Brushes.White,
                            FontSize = 12,
                            FontFamily = (FontFamily)FindResource("DINNextLight"),
                            Margin = new Thickness(0, 0, 0, 1)
                        };

                        TextBlock levelTypeText = new TextBlock
                        {
                            Foreground = new SolidColorBrush(Color.FromRgb(155, 200, 194)),
                            FontSize = 12,
                            FontFamily = (FontFamily)FindResource("DINNextRegular")
                        };

                        levelIndexText.Text = "LEVEL " + (i + 1).ToString();
                        levelTypeText.Text = WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["levels"][i];
                        levelTypeText.Text = levelTypeText.Text.ToUpper();

                        if (i > 0)
                        {
                            levelButton.Margin = new Thickness(0,8,0,0);
                        }

                        levelButton.Click += SelectItemLevel;
                        levelButton.Tag = i.ToString();


                        textStackPanel.Children.Add(levelIndexText);
                        textStackPanel.Children.Add(levelTypeText);
                        levelButtonBorder.Child = textStackPanel;
                        levelButton.Content = levelButtonBorder;

                        // Add the buttons to the LevelList StackPanel
                        LevelList.Children.Add(levelButton);
                    }



                    // set up stuff for the item in the ItemViewGrid
                    WindowshopGlobals.lookingAtChromas = false;
                    WindowshopGlobals.mainWindow.GetViewModel().RefreshItemViewMenu();

                    StoreGrid.Visibility = Visibility.Hidden;
                    ItemViewGrid.Visibility = Visibility.Visible;
                    VideoGrid.Visibility = Visibility.Hidden;
                    VideoControlsPanel.Visibility = Visibility.Hidden;
                }
            }
        }

        private void LeaveItemViewGrid(object sender, RoutedEventArgs e)
        {
            StoreGrid.Visibility = Visibility.Visible;
            ItemViewGrid.Visibility = Visibility.Hidden;
            VideoGrid.Visibility = Visibility.Hidden;
            VideoControlsPanel.Visibility = Visibility.Hidden;
        }

        private void SelectItemChroma(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var argument = int.Parse(button.Tag as string);

                
            WindowshopGlobals.chromaSelected = argument;
            WindowshopGlobals.lookingAtChromas = true;

            WindowshopGlobals.mainWindow.GetViewModel().RefreshItemViewMenu("chromas");
        }

        private void SelectItemLevel(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var argument = int.Parse(button.Tag as string);


            WindowshopGlobals.levelSelected = argument;
            WindowshopGlobals.lookingAtChromas = false;

            WindowshopGlobals.mainWindow.GetViewModel().RefreshItemViewMenu("levels");
        }

        public void CheckForVideo()
        {
            if (WindowshopGlobals.lookingAtChromas)
            {
                if (WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["videos"]["chromas"][WindowshopGlobals.chromaSelected] == null)
                {
                    WatchVideoButton.Visibility = Visibility.Hidden;
                }
                else
                {
                    WatchVideoButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["videos"]["levels"][WindowshopGlobals.levelSelected] == null)
                {
                    WatchVideoButton.Visibility = Visibility.Hidden;
                }
                else
                {
                    WatchVideoButton.Visibility = Visibility.Visible;
                }
            }
        }

        public void WatchVideo(object sender, RoutedEventArgs e)
        {
            if (WindowshopGlobals.lookingAtChromas)
                WindowshopGlobals.currentVideoUrl = WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["videos"]["chromas"][WindowshopGlobals.chromaSelected];
            else
                WindowshopGlobals.currentVideoUrl = WindowshopGlobals.mainShopItems[WindowshopGlobals.itemSelected]["videos"]["levels"][WindowshopGlobals.levelSelected];

            VideoGrid.Visibility = Visibility.Visible;
            VideoControlsPanel.Visibility = Visibility.Visible;

            var media = new Media(_libVLC, new Uri(WindowshopGlobals.currentVideoUrl));
            _mediaPlayer.Play(media);
        }

        public void CloseVideo(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            VideoGrid.Visibility = Visibility.Hidden;
            VideoControlsPanel.Visibility = Visibility.Hidden;
        }

        public void PauseVideo(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Pause();
        }

        public void PlayVideo(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Play();
        }

        protected override void OnClosed(EventArgs e)
        {
            _mediaPlayer.Dispose();
            _libVLC.Dispose();
            base.OnClosed(e);
        }
    }
}