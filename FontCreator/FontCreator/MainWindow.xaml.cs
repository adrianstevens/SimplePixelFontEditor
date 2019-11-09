using FontCreator.Models;
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

        Brush enabledBrush = new SolidColorBrush(Colors.LawnGreen);
        Brush disabledBrush = new SolidColorBrush(Colors.DarkGreen);

        PixelFont currentFont;

        int characterIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            EventManager.RegisterClassHandler(typeof(Window),
                Keyboard.KeyUpEvent, new KeyEventHandler(OnKeyUp), true);

            int w = 12;
            int h = 20;

            SetGrid(w, h);

            var text = LoadFontData($"font_{w}x{h}.txt");

            currentFont = ParseFontText(w, h, text);

            InitUI();

            UpdateGrid();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.B)
            {
                BtnBack_Click(sender, new RoutedEventArgs());
            }
            else if(e.Key == Key.N)
            {
                BtnNext_Click(sender, new RoutedEventArgs());
            }
            else if (e.Key == Key.Up)
            {
                ShiftUp();
            }
            else if (e.Key == Key.Down)
            {
                ShiftDown();
            }
            else if (e.Key == Key.Left)
            {
                ShiftLeft();
            }
            else if (e.Key == Key.Right)
            {
                ShiftRight();
            }
            else if (e.Key == Key.C)
            {
                BtnClear_Click(sender, new RoutedEventArgs());
            }
        }

        void InitUI()
        {
            btnBack.Click += BtnBack_Click;
            btnEnd.Click += BtnEnd_Click;
            btnNext.Click += BtnNext_Click;
            btnStart.Click += BtnStart_Click;

            btnClear.Click += BtnClear_Click;
            btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            currentFont.Save();
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
                for(int j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j].Background = currentFont.GetCharacter(characterIndex).IsPixelSet(i, j) ? enabledBrush : disabledBrush;
                }
            }

            txtChar.Text = ((char)(32 + characterIndex)).ToString();
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

                if (string.IsNullOrWhiteSpace(line))
                    break;

                var c = new Character();
                c.ParseCharData(width, height, line);
                c.AsciiValue = index;
                index++;

                font.Add(c);

                Console.WriteLine(c.GetLineText());
            }

            return font;
        }

        void SetGrid(int columns, int rows)
        {
            if(cells != null)
            {
                foreach (var cell in cells)
                {
                    gridPixel.Children.Remove(cell);
                }
            }

            gridPixel.ColumnDefinitions.Clear();
            gridPixel.RowDefinitions.Clear();

            for(int i = 0; i < rows; i++)
            {
                gridPixel.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < columns; j++)
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

                    rect.MouseRightButtonDown += (s, a) =>
                    {
                        if(rect.Background == enabledBrush)
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
    }
}
