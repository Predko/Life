using life.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Collections;

namespace life
{
    public partial class LifeForm : Form
    {

        /// <summary>
        /// Максимальная высота панели управления и настройки(верхняя панель).
        /// </summary>
        private int MaxFlpHeight
        {
            get => Math.Max(flpGameCantrols.Top + flpGameCantrols.GetPreferredSize(Size.Empty).Height,
                            flpSettingField.Top + flpSettingField.GetPreferredSize(Size.Empty).Height);
        }

        /// <summary>
        /// Форма настройки нового игрового поля.
        /// </summary>
        private SettingField settingField;

        /// <summary>
        /// Игровое поле.
        /// </summary>
        private Field field;
        private int fieldWidth;
        private int fieldHeight;

        private LogOfSteps steps;

        public BitmapCellsStorage BitmapCells { get; private set; }

        private Bitmap bitmap;
        public Graphics bitmapGraphics;

        /// <summary>
        /// размер клетки в пикселах.
        /// </summary>
        private int cellSize = 25;

        private int moveCounter;

        /// <summary>
        /// Счётчик игровых ходов
        /// При изменении обновляет информацию на экране в lbCount
        /// </summary>
        public int MoveCounter
        {
            get => moveCounter;
            set
            {
                moveCounter = value;

                lbCount.Text = $"Step: {moveCounter,6:d}";
            }
        }

        /// <summary>
        /// Таймер для установки времени хода.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Инициализация параметров игры.
        /// </summary>
        private void InitialazeLifeForm()
        {
            lbCount.Text = $"Step: {0,5:d}";

            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            flpGameCantrols.DoubleBuffered(true);

            timer = new Timer();

            hsbTimer.Value = 300;

            SetSpeedGame(hsbTimer.Value);

            timer.Tick += TimerNext_Tick;

            KeyUp += LifeForm_KeyUp;

            btnPreviousStep.Enabled = false;

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            InitField();

            settingField = new SettingField(field, cellsComboBox,
                                            staticCellsComboBox,
                                            CellsComboBox_SelectedIndexChanged,
                                            StaticCellsComboBox_SelectedIndexChanged,
                                            CellsComboBox_DrawItem);
        }

        /// <summary>
        /// Событие для корректировки размеров формы при изменении параметров экрана.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            SetMaxFormSize();

            panelField.ClientSize = new Size(ClientSize.Width, ClientSize.Height - MaxFlpHeight);

            int dx = (int)Math.Floor((decimal)(panelField.ClientSize.Width) / fieldWidth);

            int dy = (int)Math.Floor((decimal)(panelField.ClientSize.Height) / fieldHeight);

            cellSize = Math.Min(dx, dy);

            BitmapCells.SetNewSize(new Size(cellSize, cellSize));

            SetSizeFormAndField();

            InitGraphics();

            field.Draw(bitmapGraphics, BitmapCells);
        }

        /// <summary>
        /// Инициализация графики для отрисовки игрового поля.
        /// </summary>
        private void InitGraphics()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }

            if (bitmapGraphics != null)
            {
                bitmapGraphics.Dispose();
            }

            bitmap = new Bitmap(panelField.ClientSize.Width, panelField.ClientSize.Height);

            bitmapGraphics = Graphics.FromImage(bitmap);
        }

        /// <summary>
        /// Устанавливает скорость игры, изменением интервала таймера.
        /// Изменяет значение скорости в lblSpeedGame.
        /// </summary>
        /// <param name="value">Текущее положение ползунка hsbTimer.Value</param>
        private void SetSpeedGame(int value)
        {
            timer.Interval = (value < 10) ? 500 : 510 - value;

            lblSpeedGame.Text = $"Скорость {(value / 10),2:d}";
        }

        /// <summary>
        /// Инициализация игрового поля
        /// </summary>
        private void InitField()
        {
            SetMaxFormSize();

            panelField.ClientSize = new Size(ClientSize.Width, ClientSize.Height - MaxFlpHeight);

            fieldWidth = (int)Math.Floor((decimal)(panelField.ClientSize.Width) / cellSize);

            fieldHeight = (int)Math.Floor((decimal)(panelField.ClientSize.Height) / cellSize);

            BitmapCells = new BitmapCellsStorage((Bitmap)cellsComboBox.SelectedItem, (Bitmap)staticCellsComboBox.SelectedItem);

            field = new Field(fieldWidth, fieldHeight, new CellArray(fieldWidth, fieldHeight));

            steps = new LogOfSteps(field);

            field.SetLog(steps);

            field.SettingCells();

            SetSizeFormAndField();

            InitGraphics();

            field.PrepareField();

            field.DrawAll(bitmapGraphics, BitmapCells);
        }

        /// <summary>
        /// Устанавливает максимальные размеры формы
        /// </summary>
        private void SetMaxFormSize()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            Size = new Size(workingArea.Width - 10, workingArea.Height - 10);
        }

        /// <summary>
        /// Корректирует размер рабочей области формы в соответствии с размерами поля
        /// </summary>
        private void SetSizeFormAndField()
        {
            Rectangle fieldRectangle = new Rectangle(0, 0, cellSize * fieldWidth, cellSize * fieldHeight);

            panelField.ClientSize = new Size()
            {
                Width = fieldRectangle.Width,
                Height = fieldRectangle.Height
            };

            // корректируем размер формы, если элементы управления не помещаются
            const int addWidth = 10; // запас ширины панели, для счётчика ходов

            int maxWidthFlps = flpGameCantrols.GetPreferredSize(Size.Empty).Width +
                               flpSettingField.GetPreferredSize(Size.Empty).Width + addWidth;

            // изменяем размер слиентской области формы на величину изменения panelField
            ClientSize = new Size()
            {
                Width = Math.Max(panelField.ClientSize.Width, maxWidthFlps),
                Height = panelField.ClientSize.Height + MaxFlpHeight
            };

            StartPosition = FormStartPosition.CenterScreen;
        }

        /// <summary>
        /// Загрузка игрового поля с изменением размера ячейки
        /// </summary>
        private void LoadField()
        {
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            Block block = new Block();

            Size size = block.LoadFromFile(openFileDialog.FileName);

            field.SetField(block);

            ResizeField(size.Width, size.Height);
        }

        /// <summary>
        /// Создаёт новое игровое поле заданного размера, плотности и границей.
        /// </summary>
        /// <param name="dx">Ширина.</param>
        /// <param name="dy">Высота.</param>
        /// <param name="density">Плотность.</param>
        /// <param name="isBorderCell">Наличие границы из статичных ячеек.</param>
        public void NewField(int dx, int dy, float density, bool isBorderCell)
        {
            field.SettingCells(dx, dy, density, isBorderCell);

            field.PrepareField();

            field.DrawAll(bitmapGraphics, BitmapCells);

            ResizeField(dx, dy);
        }

        /// <summary>
        /// Инициализация игрового поля с заданными размерами
        /// </summary>
        /// <param name="dx">ширина поля, в ячейках</param>
        /// <param name="dy">высота поля, в ячейках</param>
        private void ResizeField(int dx, int dy)
        {
            fieldWidth = dx;
            fieldHeight = dy;

            SetMaxFormSize();

            dx = (int)Math.Floor((decimal)(panelField.ClientSize.Width) / fieldWidth);

            dy = (int)Math.Floor((decimal)(panelField.ClientSize.Height) / fieldHeight);

            cellSize = Math.Min(dx, dy);

            BitmapCells.SetNewSize(new Size(cellSize, cellSize));

            SetSizeFormAndField();

            field.DrawAll(bitmapGraphics, BitmapCells);

            btnPreviousStep.Enabled = false;

            MoveCounter = 0;
        }
    }
}
