using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace life
{
    /// <summary>
    /// Класс, предназначенный для хранения и преобразования изображений клеток.
    /// Есть клетки двух типов - нормальная и статичная.
    /// В исходном изображении хранится изображение клетки и пустого поля в таком виде:
    /// [изображение клетки][изображение поля].
    /// </summary>
    public class BitmapCellsStorage : IDisposable
    {
        /// <summary>
        /// Исходное изображение нормальной ячейки.
        /// </summary>
        private Bitmap sourceNormalCell;

        /// <summary>
        /// Исходное изображение статичной ячейки.
        /// </summary>
        private Bitmap sourceStaticCell;

        /// <summary>
        /// Преобразованные(маштабированные) изображения для разных типов клеток.
        /// </summary>
        private Bitmap normalCell;
        private Bitmap staticCell;
        private Bitmap noCell;

        private bool disposedValue;

        public BitmapCellsStorage(Bitmap nc, Bitmap sc) => Change(nc, sc);

        /// <summary>
        /// Возвращает изображение клетки указанного типа.
        /// </summary>
        /// <param name="cell">Тип клетки.</param>
        /// <returns>Изображение клетки. null - если изображения для данного типа нет.</returns>
        public Bitmap GetBitmap(StatusCell cell)
        {
            switch (cell)
            {
                case StatusCell.Yes:

                    return normalCell;

                case StatusCell.Static:

                    return staticCell;

                case StatusCell.No:

                    return noCell;
            }

            return null;
        }

        /// <summary>
        /// Освободить память (Bitmap bm), если bm не null.
        /// </summary>
        /// <param name="bm">Битмап для проверки и удаления.</param>
        private void DisposeBitmapIfNeed(Bitmap bm)
        {
            if (bm != null)
            {
                bm.Dispose();
            }
        }

        /// <summary>
        /// Изменяет исходные изображения ячеек и маштабирует их к текущему размеру.
        /// </summary>
        /// <param name="nc">Новое изображение для нормальных ячеек или null если не нужно менять.</param>
        /// <param name="sc">Новое изображение для статичных ячеек или null если не нужно менять.</param>
        public void Change(Bitmap nc, Bitmap sc)
        {
            if (nc != null)
            {
                sourceNormalCell = nc;

                if (normalCell != null)
                {
                    Size currentCellSize = new Size(normalCell.Width, normalCell.Height);

                    normalCell.Dispose();

                    normalCell = GetNewBitmapCell(sourceNormalCell, currentCellSize);

                    noCell.Dispose();

                    noCell = GetNewBitmapCell(sourceNormalCell, currentCellSize, sourceNormalCell.Width / 2);
                }
                else
                {
                    normalCell = nc.Clone(new Rectangle(0, 0, nc.Width / 2, nc.Height), nc.PixelFormat);

                    noCell = nc.Clone(new Rectangle(nc.Width / 2, 0, nc.Width / 2, nc.Height), nc.PixelFormat);
                }
            }

            if (sc != null)
            {
                sourceStaticCell = sc;

                if (staticCell != null)
                {
                    Size currentCellSize = new Size(staticCell.Width, staticCell.Height);

                    staticCell.Dispose();

                    staticCell = GetNewBitmapCell(sourceStaticCell, currentCellSize);
                }
                else
                {
                    staticCell = sc.Clone(new Rectangle(0, 0, sc.Width / 2, sc.Height), sc.PixelFormat);
                }
            }
        }

        /// <summary>
        /// Создаёт новое изображение ячейки заданного размера, 
        /// начиная с указанной позиции beginX в исходном изображении. 
        /// </summary>
        /// <param name="bm">Исходное изображение.</param>
        /// <param name="sz">Требуемый размер.</param>
        /// <param name="beginX">Позиция в исходном изображении.</param>
        /// <returns>Новое изображение заданного размера.</returns>
        private Bitmap GetNewBitmapCell(Bitmap bm, Size sz, int beginX = 0)
        {
            Bitmap tempBitmap = bm.Clone(new Rectangle(beginX, 0, bm.Width / 2, bm.Height), bm.PixelFormat);

            var ret = new Bitmap(tempBitmap, sz);

            tempBitmap.Dispose();

            return ret;
        }

        /// <summary>
        /// Меняет размер изображений.
        /// </summary>
        /// <param name="sz">Требуемый размер.</param>
        public void SetNewSize(Size sz)
        {
            DisposeBitmapIfNeed(normalCell);

            normalCell = GetNewBitmapCell(sourceNormalCell, sz);

            DisposeBitmapIfNeed(staticCell);

            staticCell = GetNewBitmapCell(sourceStaticCell, sz);

            DisposeBitmapIfNeed(noCell);

            noCell = GetNewBitmapCell(sourceNormalCell, sz, sourceNormalCell.Width / 2);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    normalCell.Dispose();
                    staticCell.Dispose();
                    noCell.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~BitmapCellsStorage()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
