using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLib
{
    public class Vector
    {
        private double[] _vector;

        public int Length { get { return _vector.Length; } }
        
        public double this[int index]
        {
            get
            {
                return _vector[index - 1];
            }

            set
            {
                _vector[index - 1] = value;
            }
        }

        public Vector(int CountOfElements)
        {
            _vector = new double[CountOfElements];
        }
        public Vector(double el_1, double el_2, double el_3)
        {
            _vector = new double[3] { el_1, el_2, el_3 };
        }
        public Vector(double[] values)
        {
            _vector = new double[values.Length];
            for(int i = 0; i < _vector.Length; i++)
            {
                _vector[i] = values[i];
            }
        }
        public static Vector Zero(int size)
        {
            var _result = new Vector(size);
            _result.Zero();
            return _result;
        }
        private void Zero()
        {
            for (int _row = 0; _row < _vector.Length; _row++)
            {
                _vector[_row] = 0.0;
            }
        }
        public Vector Dublicate()
        {
            Vector vector = Vector.Zero(_vector.Length);
            for (int i = 0; i < _vector.Length; i++)
            {
                vector[i+1] = _vector[i];
            }
            return vector;
        }
        public void Add(Vector vector)
        {
            if (Length != vector.Length)
                throw new InvalidOperationException("Cannot add matrices of different sizes.");
            for (int _row = 0; _row < _vector.Length; _row++)
            {
                _vector[_row] += vector[_row + 1];
            }
        }
        public void Sub(Vector vector)
        {
            if (Length != vector.Length)
                throw new InvalidOperationException("Cannot add matrices of different sizes.");
            for (int _row = 0; _row < _vector.Length; _row++)
            {
                _vector[_row] -= vector[_row + 1];
            }
        }
        public Matrix Diag()
        {
            Matrix matrix = new Matrix(_vector.Length);
            for (int i = 0; i < _vector.Length; i++)
            {
                matrix[i + 1, i + 1] = _vector[i];
            }
            return matrix;
        }
        public static Vector operator +(Vector left, Vector right)
        {
            Vector result = left.Dublicate();
            result.Add(right);
            return result;   
        }
        public static Vector operator -(Vector left, Vector right)
        {
            Vector result = left.Dublicate();
            result.Sub(right);
            return result;
        }


    }
}
