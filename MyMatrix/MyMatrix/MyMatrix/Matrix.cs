using System.Diagnostics;
using System.Text;

namespace MatrixLib
{
    [DebuggerDisplay("Matrix = {ToString()}")]
    public partial class Matrix
    {
        private double[,] _matrix;

        public int Rows { get { return _matrix.GetLength(0); } }
        public int Columns { get { return _matrix.GetLength(1); } }

        public bool IsSquare => (Rows == Columns);

        public double this[int row, int column]
        {
            get
            {
                return _matrix[row - 1, column - 1];
            }

            set
            {
                _matrix[row - 1, column - 1] = value;
            }
        }

        public Matrix(int Rows, int Columns)
        {
            _matrix = new double[Rows, Columns];
        }
        public Matrix(int m) : this(m, m) { }


    }
}