using FontCreator.Models;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FontCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Button[,] cells;

        readonly Brush enabledBrush = new SolidColorBrush(Colors.LawnGreen);
        readonly Brush disabledBrush = new SolidColorBrush(Colors.DarkGreen);

        PixelFont currentFont;

        int characterIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(Window),
                Keyboard.KeyUpEvent, new KeyEventHandler(OnKeyUp), true);

            InitUI();

            int w = 16;
            int h = 24;

            LoadFont(w, h);
        }

        private void LoadFont(int width, int height)
        {
            var fontName = $"font_{width}x{height}.txt";

            try
            {
                var text = LoadFontData(fontName);
                currentFont = ParseFontText(width, height, text);
                UpdateStatus($"{fontName} loaded");
            }
            catch
            {
                UpdateStatus($"{fontName} not found");
                UpdateStatus($"Creating new font");
                currentFont = new PixelFont(height, width, 32, 255);
            }

            SetGrid(width, height);
            UpdateGrid();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.B:
                    BtnBack_Click(sender, new RoutedEventArgs());
                    break;
                case Key.C:
                    BtnClear_Click(sender, new RoutedEventArgs());
                    break;
                case Key.E:
                    BtnEnd_Click(sender, new RoutedEventArgs());
                    break;
                case Key.N:
                    BtnNext_Click(sender, new RoutedEventArgs());
                    break;
                case Key.S:
                    BtnStart_Click(sender, new RoutedEventArgs());
                    break;
                case Key.Left:
                    ShiftLeft();
                    break;
                case Key.Right:
                    ShiftRight();
                    break;
                case Key.Up:
                    ShiftUp();
                    break;
                case Key.Down:
                    ShiftDown();
                    break;
            }
        }

        void InitUI()
        {
            btnBack.Click += BtnBack_Click;
            btnEnd.Click += BtnEnd_Click;
            btnNext.Click += BtnNext_Click;
            btnStart.Click += BtnStart_Click;

            btnClear.Click += BtnClear_Click;
            btnSaveFont.Click += BtnSave_Click;
            btnLoadFont.Click += BtnLoad_Click;
            btnOpenFont.Click += BtnOpen_Click;
            btnCreateFont.Click += BtnCreate_Click;

            txtPreview.Text = string.Empty;

            for (int i = 0; i < 255; i++)
            {
                if (i == 0)
                    i += 32;

                if (i == 127)
                    i += 33;

                txtPreview.Text += (char)i;
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            int w = int.Parse(txtFontWidth.Text);
            int h = int.Parse(txtFontHeight.Text);

            var fontName = $"font_{w}x{h}.txt";

            currentFont = new PixelFont(h, w, 32, 255);

            UpdateStatus($"{fontName} created");

            SetGrid(w, h);
            UpdateGrid();

        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var data = File.ReadAllText(openFileDialog.FileName);

                var currentFont = FontClassLoader.LoadFont(Path.GetFileName(openFileDialog.FileName), data);

                Console.WriteLine("File loaded");
            }

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            currentFont.Save(true);

            UpdateStatus($"saved");
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            int w = int.Parse(txtFontWidth.Text);
            int h = int.Parse(txtFontHeight.Text);

            LoadFont(w, h);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            currentFont.Characters[characterIndex].Clear();
            UpdateGrid();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            characterIndex = 0;
            UpdateGrid();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (characterIndex < currentFont.Count - 1)
            {
                characterIndex++;
            }

            UpdateGrid();
        }

        private void BtnEnd_Click(object sender, RoutedEventArgs e)
        {
            characterIndex = currentFont.Count - 1;
            UpdateGrid();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (characterIndex > 0)
                characterIndex--;

            UpdateGrid();
        }

        void ShiftLeft()
        {
            currentFont.GetCharacter(characterIndex).ShiftLeft();
            UpdateGrid();
        }

        void ShiftRight()
        {
            currentFont.GetCharacter(characterIndex).ShiftRight();
            UpdateGrid();
        }

        void ShiftUp()
        {
            currentFont.GetCharacter(characterIndex).ShiftUp();
            UpdateGrid();
        }

        void ShiftDown()
        {
            currentFont.GetCharacter(characterIndex).ShiftDown();
            UpdateGrid();
        }

        void UpdateGrid()
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j].Background = currentFont.GetCharacter(characterIndex).IsPixelSet(i, j) ? enabledBrush : disabledBrush;
                }
            }

            int offset = 32;
            if (characterIndex > 126 - 32)
                offset += 33;

            txtAscii.Text = $"Ascii: {offset + characterIndex}";
            txtChar.Text = ((char)(offset + characterIndex)).ToString();
        }

        PixelFont ParseFontText(int width, int height, string text)
        {
            var font = new PixelFont();

            var reader = new StringReader(text);

            string line;

            int index = 32;

            while (true)
            {
                line = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(line)) { break; }

                var c = new Character();
                c.ParseCharData(width, height, line);
                c.AsciiValue = index;

                font.Add(c);

                Console.WriteLine(c.GetLineText());

                index++;

                if (index == 127)
                {
                    index += 33;
                }
            }

            return font;
        }

        void SetGrid(int columns, int rows)
        {
            if (cells != null)
            {
                foreach (var cell in cells)
                {
                    gridPixel.Children.Remove(cell);
                }
            }

            gridPixel.ColumnDefinitions.Clear();
            gridPixel.RowDefinitions.Clear();

            int count = Math.Max(rows, columns);

            for (int i = 0; i < count; i++)
            {
                gridPixel.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < count; j++)
            {
                gridPixel.ColumnDefinitions.Add(new ColumnDefinition());
            }

            cells = new Button[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var rect = new Button()
                    {
                        Background = disabledBrush,
                        Margin = new Thickness(1),
                    };

                    Grid.SetColumn(rect, x);
                    Grid.SetRow(rect, y);

                    cells[x, y] = rect;

                    // rect.PreviewMouseLeftButtonUp

                    rect.PreviewMouseLeftButtonDown += (s, a) =>
                    {
                        if (rect.Background == enabledBrush)
                        {
                            rect.Background = disabledBrush;
                        }
                        else
                        {
                            rect.Background = enabledBrush;
                        }
                        //should toggle 
                        bool currentState = currentFont.GetCharacter(characterIndex).IsPixelSet(Grid.GetColumn(rect), Grid.GetRow(rect));
                        currentFont.GetCharacter(characterIndex).SetPixel(Grid.GetColumn(rect), Grid.GetRow(rect), !currentState);
                    };

                    rect.MouseRightButtonDown += (s, a) =>
                    {
                        if (rect.Background == enabledBrush)
                        {
                            isDrawing = false;
                            rect.Background = disabledBrush;
                        }
                        else
                        {
                            isDrawing = true;
                            rect.Background = enabledBrush;
                        }
                        //lazy but it'll work ...
                        currentFont.GetCharacter(characterIndex).SetPixel(Grid.GetColumn(rect), Grid.GetRow(rect), isDrawing);
                    };

                    rect.MouseMove += (s, a) =>
                    {
                        if (a.RightButton == MouseButtonState.Pressed)
                        {
                            rect.Background = isDrawing ? enabledBrush : disabledBrush;
                            currentFont.GetCharacter(characterIndex).SetPixel(Grid.GetColumn(rect), Grid.GetRow(rect), isDrawing);

                        }
                    };

                    gridPixel.Children.Add(rect);
                }
            }
        }

        private void Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        bool isDrawing = true;

        string LoadFontData(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = $"FontCreator.{name}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void UpdateStatus(string text)
        {
            txtStatus.Text += text + "\r\n";
        }
    }
}
