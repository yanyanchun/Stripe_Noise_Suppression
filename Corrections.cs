using System;

namespace MTL_TFT_RCN
{
    /// <summary>
    /// Methods for RCN correction without dummy pixels information.
    /// </summary>
    public static class RCN_NoDummyCorrections
    {
        /// <summary>
        /// Gets the medians of the diff's by rows of the image, filters them, substract from the image (with bias correction) and return the corrected copy of the image.
        /// </summary>
        /// <param name="InputImage">Image for correction, two-dimensional array of 16-bits unsigned, first dimension is rows, second is columns (UInt16[rows,columns]).</param>
        /// <param name="FilterOrder">Filter order for the moving average.</param>
        /// <param name="BiasShifting">Value which will be subtracted in bias correction process.</param>
        /// <returns></returns>
        public static UInt16[,] MediansSubtracting(UInt16[,] InputImage, byte FilterOrder, int BiasShifting)
        {
            //Variables.
            int Rows = InputImage.GetLength(0);
            int Cols = InputImage.GetLength(1);
            UInt16[,] OutputImage = new UInt16[Rows, Cols];
            Int32[] Diffs = new Int32[Cols];
            Int32[] Medians = new Int32[Rows];
            Int32[] FilteredMedians = new Int32[Rows];

            //Image copying.
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    OutputImage[i, j] = InputImage[i, j];

            //Medians calculating and substractring.
            for (int i = 1; i < Rows; i++)
            {
                //Diffs calculating.
                for (int j = 0; j < Cols; j++)
                {
                    Diffs[j] = OutputImage[i, j] - OutputImage[i - 1, j];
                }
                //Get the median.
                Array.Sort(Diffs);
                Medians[i] = Diffs[Cols / 2];
                //Median subtracting.
                for (int j = 0; j < Cols; j++)
                {
                    OutputImage[i, j] = (UInt16)(OutputImage[i, j] - Medians[i]);
                }
            }

            //Medians filtering.
            for (int i = 0; i < Rows; i++)
            {
                FilteredMedians[i] = 0;
                //Filter with smoothy entry.
                int CurrentOrder = (i < FilterOrder) ? i : FilterOrder;
                for (int k = 0; k < (CurrentOrder + 1); k++) FilteredMedians[i] += Medians[i - k];
                FilteredMedians[i] /= (CurrentOrder + 1);
            }

            //Bias correction.
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    OutputImage[i, j] = (UInt16)(OutputImage[i, j] + FilteredMedians[i] - BiasShifting);
            return OutputImage;
        }
    }
}
