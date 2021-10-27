using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Matrix
{
    public class MatrixOperations
    {
        //-------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Создание матрицы с заданным количесвом строк и столбцов
        /// </summary>
        /// <param name="rows">Количество строк</param>
        /// <param name="cols">Количество столбцов</param>
        /// <returns>Матрица с указаным количеством строк и столбцов</returns>
        public static double[][] Create(int rows, int cols)
        {
            double[][] result = new double[rows][];
            try
            {
                for (int i = 0; i < rows; ++i)
                {
                    result[i] = new double[cols];
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Matrix_Create(): " + e.Message);
            }
            return result;
        }
        public static double[][] Zeros(int rows, int cols)
        {
            double[][] result = new double[rows][];
            try
            {
                for (int i = 0; i < rows; ++i)
                {
                    result[i] = new double[cols];
                    for (int j = 0; j < cols; j++)
                        result[i][j] = 0;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Matrix_Create(): " + e.Message);
            }
            return result;
        }
        //-----------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Произведение матриц
        /// </summary>
        /// <param name="matrixA">Первая матрица в произведение</param>
        /// <param name="matrixB">Вторая матрица в произведение</param>
        /// <returns>Итоговая матрица произведения</returns>
        public static double[][] Product(double[][] matrixA,
                                        double[][] matrixB)
        {
            int aRows = matrixA.Length; int aCols = matrixA[0].Length;
            int bRows = matrixB.Length; int bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Неподходящие матрицы для произведения");
            double[][] result = Create(aRows, bCols);
            try
            {
                for (int i = 0; i < aRows; ++i)
                    for (int j = 0; j < bCols; ++j)
                        for (int k = 0; k < aCols; ++k)
                            result[i][j] += matrixA[i][k] * matrixB[k][j];
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Matrix_Product(): " + e.Message);
            }
            return result;
        }
        //--------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Транспорация матрицы
        /// </summary>
        /// <param name="matrix">Матрица которую транспонируют</param>
        /// <returns>Транспонированая матрица</returns>
        public static double[][] Transporation(double[][] matrix)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;
            double[][] result = Create(cols, rows);
            try
            {
                for (int i = 0; i < cols; ++i)
                    for (int j = 0; j < rows; ++j)
                        result[i][j] = matrix[j][i];
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Matrix_Transporation(): " + e.Message);
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Создание матрицы миноров
        /// </summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <returns>Матрица миноров, подсчитанная из исходной матрицы</returns>
        public static double[][] Minor(double[][] matrix)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;
            if (rows != cols)
                throw new Exception("Матрица не квадратного типа");
            double[][] result = Create(matrix.Length, matrix.Length);
            try
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                    {
                        double[][] minor = Delete_Row_and_col(matrix, i, j);
                        result[i][j] = Math.Pow(-1, 2 + i + j) * Determinant(minor);
                    }
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Minor_Matrix(): " + e.Message);
            }
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Удаляет из матрицы определенные ряд и колонну 
        /// </summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <param name="row">Номер ряда</param>
        /// <param name="col">Номер колонны</param>
        /// <returns>Исходная матрица без выбранного ряда и колонны</returns>
        public static double[][] Delete_Row_and_col(double[][] matrix, int row, int col)
        {
            if ((row < 0 || row >= matrix.Length) && (col < 0 || col >= matrix.Length))
                throw new Exception("Не правильно указан номер ряда или строки");
            double[][] result = Create(matrix.Length - 1, matrix.Length - 1);
            try
            {
                int Rows = 0;
                int Cols;
                for (int i = 0; i < matrix.Length - 1; ++i)
                {
                    Cols = 0;
                    for (int j = 0; j < matrix.Length - 1; ++j)
                    {
                        if (Rows == row)
                            Rows++;
                        if (Cols == col)
                            Cols++;
                        result[i][j] = matrix[Rows][Cols];
                        Cols++;
                    }
                    Rows++;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Delete_Row_and_col(): " + e.Message);
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Отобразить матрицу
        /// </summary>
        /// <param name="matrix">Матрица для отображения</param>
        public static void Draw_Matrix(double[][] matrix)
        {
            try
            {
                for (int i = 0; i < matrix.Length; i++)
                {
                    for (int j = 0; j < matrix[0].Length; j++)
                        Console.Write(matrix[i][j] + " ");
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Draw_Matrix(): " + e.Message);
            }
        }
        //---------------------------------------------------------------------------------------------------
        public static double DeterminantDI(double[][] matrix)
        {
            double det = 1;
            //определяем переменную EPS
            const double EPS = 1E-9;
            //размерность матрицы
            int n = matrix.Length;
            double[][] b = new double[1][];
            b[0] = new double[n];
            //проходим по строкам
            for (int i = 0; i < n; ++i)
            {
                //присваиваем k номер строки
                int k = i;
                //идем по строке от i+1 до конца
                for (int j = i + 1; j < n; ++j)
                    //проверяем
                    if (Math.Abs(matrix[j][i]) > Math.Abs(matrix[k][i]))
                        //если равенство выполняется то k присваиваем j
                        k = j;
                //если равенство выполняется то определитель приравниваем 0 и выходим из программы
                if (Math.Abs(matrix[k][i]) < EPS)
                {
                    det = 0;
                    break;
                }
                //меняем местами a[i] и a[k]
                b[0] = matrix[i];
                matrix[i] = matrix[k];
                matrix[k] = b[0];
                //если i не равно k
                if (i != k)
                    //то меняем знак определителя
                    det = -det;
                //умножаем det на элемент a[i][i]
                det *= matrix[i][i];
                //идем по строке от i+1 до конца
                for (int j = i + 1; j < n; ++j)
                    //каждый элемент делим на a[i][i]
                    matrix[i][j] /= matrix[i][i];
                //идем по столбцам
                for (int j = 0; j < n; ++j)
                    //проверяем
                    if ((j != i) && (Math.Abs(matrix[j][i]) > EPS))
                        //если да, то идем по k от i+1
                        for (k = i + 1; k < n; ++k)
                            matrix[j][k] -= matrix[i][k] * matrix[j][i];
            }
            return det;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Подсчет определителя матрицы
        /// </summary>
        /// <param name="matrix">Матрица, определитель которой надо найти</param>
        /// <returns>Определитель исходной матрицы</returns>
        public static double Determinant(double[][] matrix)
        {
            //Проверка на кваратную матрицу
            int rows = matrix.Length;
            int cols = matrix[0].Length;
            if (rows != cols)
                throw new Exception("Матрица не квадратного типа");
            //Новая матрица для расчетов
            double[][] NewMatrix = Create(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    NewMatrix[i][j] = matrix[i][j];
            //Определитель
            double Determinant = 1;
            //Счетчик переставления строчек
            double s = 0;
            //Расчет определителя
            try
            {
                //Если матрица имеет болше одного ряда
                if (rows > 1)
                {
                    for (int i = 0; i < cols - 1; i++)
                    {
                        for (int j = i + 1; j < rows; j++)
                        {
                            //Перемещение ряда на низ если в рассматриваемой позиции уже ноль
                            if (NewMatrix[i][i] == 0)
                            {
                                int t = j + 1;
                                int index = j + 1;
                                for (int k = j + 1; k < rows; k++, t++)
                                    if (NewMatrix[k][i] != 0)
                                    {
                                        index = k;
                                        break;
                                    }
                                if (t == rows)
                                    break;
                                NewMatrix = Replace_Rows(NewMatrix, i, index);
                                s++;
                            }
                            // Получение строки с нулями под диагональю
                            for (int f = cols - 1; f >= 0; f--)
                            {
                                double a = NewMatrix[i][f];
                                double b = NewMatrix[i][i];
                                double c = NewMatrix[j][i];
                                NewMatrix[j][f] -= a / b * c;
                            }

                        }
                    }
                    //Умножение значений по диагонали для определения детерминанта
                    for (int i = 0; i < rows; i++)
                        Determinant *= NewMatrix[i][i];
                }
                //Корректировка знака определителя из-за количества перестановок
                Determinant *= Math.Pow(-1, s);
                //Если матрица состоит из одного ряда
                if (rows == 1)
                    Determinant = matrix[0][0];
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Calculate_Determinant(): " + e.Message);
            }
            return Determinant;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Обоюдная замена двух рядов матрицы
        /// </summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <param name="row1">Первый ряд для замены</param>
        /// <param name="row2">Второй ряд для замены</param>
        /// <returns>Матрица с замененными рядами</returns>
        public static double[][] Replace_Rows(double[][] matrix, int row1, int row2)
        {

            double[][] result = Create(matrix.Length, matrix[0].Length);
            try
            {
                for (int i = 0; i < matrix.Length; i++)
                    for (int j = 0; j < matrix.Length; j++)
                        result[i][j] = matrix[i][j];
                double[] Row1 = matrix[row1];
                double[] Row2 = matrix[row2];
                result[row1] = Row2;
                result[row2] = Row1;
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Replace_Rows(): " + e.Message);
            }
            return result;
        }
        //-------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Получение обратной матрицы
        /// </summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <returns>Обратная матрица</returns>
        public static double[][] Inverted(double[][] matrix)
        {
            double[][] result;
            try
            {
                //double det = Determinant(matrix);
                double det = DeterminantDI(matrix);
                result = Inverted(matrix, det);
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Inverted_Matrix(): " + e.Message);
            }
            return result;
        }
        //-------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Получение обратной матрицы
        /// </summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <param name="determinant">Определитель матрицы</param>
        /// <returns>Обратная матрица</returns>
        public static double[][] Inverted(double[][] matrix, double determinant)
        {
            //Проверка определителя
            if (determinant == 0)
                throw new Exception("Определитель матрицы равен нулю, значит матрица вырожденная");
            if (matrix.Length != matrix[0].Length)
                throw new Exception("Матрица не квадратного типа");
            //Результирующая матрица
            double[][] result = Create(matrix.Length, matrix[0].Length);
            try
            {
                //Создание сшитой исходной матрицы с единичной
                double[][] DoubleMatrix = Create(matrix.Length, matrix[0].Length * 2);
                for (int i = 0; i < matrix.Length; i++)
                    for (int j = 0; j < matrix[0].Length; j++)
                        DoubleMatrix[i][j] = matrix[i][j];
                for (int i = matrix[0].Length; i < matrix[0].Length * 2; i++)
                    DoubleMatrix[i - matrix[0].Length][i] = 1;
                //Рассчет обратной матрицы
                if (matrix.Length > 1)
                {
                    for (int col = 0; col < matrix.Length - 1; col++)
                        for (int row = col + 1; row < matrix[0].Length; row++)
                        {
                            if (DoubleMatrix[row][col] == 0)
                                continue;
                            for (int f = matrix[0].Length * 2 - 1; f >= 0; f--)
                                DoubleMatrix[row][f] -= DoubleMatrix[col][f] / DoubleMatrix[col][col] * DoubleMatrix[row][col];
                        }
                    for (int col = matrix.Length - 1; col > 0; col--)
                        for (int row = col - 1; row >= 0; row--)
                        {
                            if (DoubleMatrix[row][col] == 0)
                                continue;
                            for (int f = matrix[0].Length * 2 - 1; f >= 0; f--)
                                DoubleMatrix[row][f] -= DoubleMatrix[col][f] / DoubleMatrix[col][col] * DoubleMatrix[row][col];
                        }
                    for (int i = 0; i < matrix.Length; i++)
                    {
                        for (int j = matrix[0].Length * 2 - 1; j > 0; j--)
                            DoubleMatrix[i][j] /= DoubleMatrix[i][i];
                    }
                    //Заполнение результируещей матрицы
                    for (int i = 0; i < matrix.Length; i++)
                        for (int j = 0; j < matrix[0].Length; j++)
                            result[i][j] = DoubleMatrix[i][j + matrix[0].Length];
                }
                else
                    result[0][0] = Math.Pow(matrix[0][0], -1);
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Invert_Matrix(): " + e.Message);
            }
            return result;
        }
        //---------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Умножение матрицы на число
        /// </summary>
        /// <param name="matrix">Исходная матрица</param>
        /// <param name="num">Число, на которое умножается матрица</param>
        /// <returns>Произведение матрицы и числа</returns>
        public static double[][] Multiplication(double[][] matrix, double num)
        {
            double[][] result = Create(matrix.Length, matrix[0].Length);
            try
            {
                for (int i = 0; i < matrix.Length; i++)
                    for (int j = 0; j < matrix[0].Length; j++)
                    {
                        result[i][j] = matrix[i][j] * num;
                    }
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Matrix_Multiplication(): " + e.Message);
            }
            return result;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Создание квадратной еденичной матрицы
        /// </summary>
        /// <param name="N">Порядок матрицы</param>
        /// <returns>Еденичная матрица порядка N</returns>
        public static double[][] CreateIdentityMatrix(int N)
        {
            double[][] UnitMatrix = Create(N, N);
            try
            {
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        UnitMatrix[i][j] = 0;
                for (int i = 0; i < N; i++)
                    UnitMatrix[i][i] = 1;
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Create_Unit_Matrix(): " + e.Message);
            }
            return UnitMatrix;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Суммирование двух матриц
        /// </summary>
        /// <param name="MatrixA">Первая матрица</param>
        /// <param name="MatrixB">Вторая матрица</param>
        /// <returns>Матрица суммы</returns>
        public static double[][] Sum(double[][] MatrixA, double[][] MatrixB)
        {
            if (MatrixA.Length != MatrixB.Length || MatrixA[0].Length != MatrixB[0].Length)
                throw new Exception("Размеры матриц не равны");
            double[][] result = Create(MatrixA.Length, MatrixA[0].Length);
            try
            {
                for (int i = 0; i < MatrixA.Length; i++)
                    for (int j = 0; j < MatrixA[0].Length; j++)
                        result[i][j] = MatrixA[i][j] + MatrixB[i][j];
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Matrix_Sum(): " + e.Message);
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Вычетание из матрицы MatrixA матрицы MatrixB
        /// </summary>
        /// <param name="MatrixA">Первая матрица</param>
        /// <param name="MatrixB">Вторая матрица</param>
        /// <returns>Матрица разности</returns>
        public static double[][] Difference(double[][] MatrixA, double[][] MatrixB)
        {
            if (MatrixA.Length != MatrixB.Length || MatrixA[0].Length != MatrixB[0].Length)
                throw new Exception("Размеры матриц не равны");
            double[][] result = Create(MatrixA.Length, MatrixA[0].Length);
            try
            {
                for (int i = 0; i < MatrixA.Length; i++)
                    for (int j = 0; j < MatrixA[0].Length; j++)
                        result[i][j] = MatrixA[i][j] - MatrixB[i][j];
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Matrix_Difference(): " + e.Message);
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Метод наименьших квадратов
        /// </summary>
        /// <param name="X">Матрица занчений Х</param>
        /// <param name="Y">Матрица значений Y</param>
        /// <returns>Матрица В из уравнения матриц Y = X*B</returns>
        public static double[][] LeastSquares(double[][] X, double[][] Y)
        {
            double[][] result;
            try
            {
                double[][] Xt = Transporation(X);
                double[][] XtY = Product(Xt, Y);
                double[][] XtX = Product(Xt, X);
                double[][] CopyXtX = XtX;
                double det = Determinant(CopyXtX);
                if (det == 0)
                {
                    double Lambda = 0.1;
                    double[][] UnitMatrix = CreateIdentityMatrix(CopyXtX.Length);
                    XtX = Sum(CopyXtX, Multiplication(UnitMatrix, Lambda));
                }
                det = Determinant(XtX);
                double[][] Inverted_XtX = Inverted(XtX, det);
                result = Product(Inverted_XtX, XtY);
                foreach (double[] i in result)
                {
                    i[0] = Math.Round(i[0], 15);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error TRegressionMatrix.Least_Squares(): " + e.Message);
            }
            return result;
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------
    }
}
