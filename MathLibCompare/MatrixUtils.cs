using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace MathLibTest
{
  class MatrixUtils
  {
    public static double MAX_DIFF = 0.00001; //Double.Epsilon;

    #region singleton
    private static MatrixUtils _instance;
    
    public static MatrixUtils Instance
    {
      get
      {
        if (_instance == null) _instance = new MatrixUtils();
        return _instance;
      }
    }

    private MatrixUtils()
    {
    } 
    #endregion

    /// <summary>
    /// Algoritmo base
    /// </summary>
    /// <param name="monoDim"></param>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <param name="mRes"></param>
    public void MatMulSimple(int monoDim, double[] m1, double[] m2, double[] mRes)
    {
      for (int j = 0; j < monoDim; j++)
      {
        for (int k = 0; k < monoDim; k++)
        {
          double temp = 0;
          for (int m = 0; m < monoDim; m++)
          {
            int index1 = j * monoDim + m;
            int index2 = m * monoDim + k;
            temp = temp + m1[index1] * m2[index2];
          }
          int indexRes = j * monoDim + k;
          mRes[indexRes] = temp;
        }
      }
    }

    /// <summary>
    /// Algoritmo base
    /// </summary>
    /// <param name="monoDim"></param>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <param name="mRes"></param>
    public void MatMulOptimize(int monoDim, double[] m1, double[] m2, double[] mRes)
    {
      double temp;
      int index1, index2, index1root, indexRes;

      indexRes = index1root = 0;
      for (int j = 0; j < monoDim; j++, index1root += monoDim)
      {
        for (int k = 0; k < monoDim; k++)
        {
          temp = 0;
          index2 = k;
          for (int m = 0; m < monoDim; m++, index2 += monoDim)
          {
            index1 = index1root + m;
            temp = temp + m1[index1] * m2[index2];
          }
          mRes[indexRes++] = temp;
        }
      }
    }

    /// <summary>
    /// Parallelizzato
    /// </summary>
    /// <param name="monoDim"></param>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <param name="mRes"></param>
    public void MatMulParallel(int monoDim, double[] m1, double[] m2, double[] mRes)
    {
      Parallel.For(0, monoDim, j =>
      {
        Parallel.For(0, monoDim, k =>
        {
          double temp = 0;
          for (int m = 0; m < monoDim; m++)
          {
            int index1 = j * monoDim + m;
            int index2 = m * monoDim + k;
            temp = temp + m1[index1] * m2[index2];
          }
          int indexRes = j * monoDim + k;
          mRes[indexRes] = temp;
        });
      });
    }

    #region real matrix
    public void MatMulWithRealMat(int monoDim, double[] m1, double[] m2, out double[] mRes)
    {
      double[,] m1RealMat = Vect2Mat(m1, monoDim, monoDim);
      double[,] m2RealMat = Vect2Mat(m2, monoDim, monoDim);
      double[,] mResRealMat = new double[monoDim, monoDim];

      MatMulWithRealMat(m1RealMat, m2RealMat, mResRealMat);

      mRes = new double[monoDim * monoDim];
      mRes = Mat2Vect(mResRealMat);
    }

    private static double[] Mat2Vect(double[,] mat)
    {
      double[] vect;
      int n, m;

      n = mat.GetLength(0);
      m = mat.GetLength(1);

      vect = new double[n * m];
      for (int i = 0, k = 0; i < n; i++)
        for (int j = 0; j < m; j++)
          vect[k++] = mat[i, j];
      return vect;
    }

    private static double[,] Vect2Mat(double[] vect, int n, int m)
    {
      double[,] mat;

      if (n * m != vect.Length)
        return null;
      mat = new double[n, m];
      for (int i = 0, k = 0; i < n; i++)
        for (int j = 0; j < m; j++)
          mat[i, j] = vect[k++];
      return mat;
    }

    private void MatMulWithRealMat(double[,] m1, double[,] m2, double[,] mRes)
    {
      int n, m, l;
#if PARAM_CHECK
      if(m1 == null || m2 == null || mRes == null)
        return false;
#endif
      n = m1.GetLength(0);
      m = m1.GetLength(1);
      l = m2.GetLength(1);
#if DIM_CHECK
      if(m != m2.GetLength(0) || n != mRes.GetLength(0) || l != mRes.GetLength(1))
        return false;  
#endif
      for (int i = 0; i < n; i++)
        for (int j = 0; j < l; j++)
        {
          mRes[i, j] = 0; // pleonastico
          for (int k = 0; k < m; k++)
            mRes[i, j] += m1[i, k] * m2[k, j];
        }
    }
    #endregion

    #region utils

        public void PrintMatrix(int monoDim, double[] m)
        {
            CultureInfo ci = CultureInfo.GetCultureInfo("en-GB");
            for (int j = 0; j < monoDim; j++)
            {
                string line = "";
                for (int k = 0; k < monoDim; k++)
                {
                    int index1 = j * monoDim + k;
                    line += m[index1].ToString("0.00", ci) + ",";
                }
                Console.WriteLine(line);
            }
            Console.WriteLine();
        }

    public void SaveMatrix(string fileName, int monoDim, double[] m)
    {
      if (File.Exists(fileName)) File.Delete(fileName);
      CultureInfo ci = CultureInfo.GetCultureInfo("en-GB");
      using (StreamWriter sw = new StreamWriter(fileName))
      {
        for (int j = 0; j < monoDim; j++)
        {
          string line = "";
          for (int k = 0; k < monoDim; k++)
          {
            int index1 = j * monoDim + k;
            line += m[index1].ToString("0.0000", ci) + ",";
          }
          sw.WriteLine(line.Substring(0, line.Length - 1));
        }
      }
    }

        public bool LoadMatrix(string fileName, int monoDim, out double[] m)
        {
            m = null;
            if (!File.Exists(fileName)) return false;
            m = new double[monoDim * monoDim];

            CultureInfo ci = CultureInfo.GetCultureInfo("en-GB");
            using (StreamReader sw = new StreamReader(fileName))
            {
                int row = 0;
                int baseIndex = 0;
                while (!sw.EndOfStream)
                {
                    string line = sw.ReadLine();
                    if (string.IsNullOrEmpty(line)) break;
                    string[] values = line.Split(new char[] { ',' });

                    if (values.Length != monoDim) return false;

                    for (int col = 0; col < monoDim; col++)
                    {
                        m[baseIndex + col] = double.Parse(values[col], ci);
                    }

                    baseIndex += monoDim;
                    row++;
                }
            }
            return true;
        }

    public bool AreEqual(double[] m1, double[] m2)
    {
      if (m1.Length != m2.Length) return false;
      
      for (int i = 0; i < m1.Length; i++)
      {
#if DEBUG
        double a = m1[i];
        double b = m2[i];
        double d = Math.Abs(a - b);
                if (d > MAX_DIFF) return false;
#else
        if (Math.Abs(m1[i] - m2[i]) > MAX_DIFF) return false;
#endif
      }

      return true;
    }

    public double GetMaxDiff(double[] m1, double[] m2)
    {
      return m1.Zip(m2, (item1, item2) => Math.Abs(item1 - item2)).Max();
    }

    public void SetRandom(int monoDim, double[] m, int method)
    {
      Random random = new Random();
      for (int j = 0; j < monoDim; j++)
      {
                //Random random2 = new Random(random.Next());
        for (int k = 0; k < monoDim; k++)
        {
          int index = j * monoDim + k;
                    if (method == 1) m[index] = random.NextDouble();
                    else if (method == 2) m[index] = random.NextDouble() - 0.5;
                    else if (method == 3) m[index] = (random.NextDouble() + 1);
                    else m[index] = index;

        }
      }
    }

    public void SetZero(double[] m)
    {
      for (int i = 0; i < m.Length; i++)
        m[i] = 0;
    } 
    #endregion

  }
}
