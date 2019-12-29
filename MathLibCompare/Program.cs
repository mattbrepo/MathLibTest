using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace MathLibTest
{
    class Program
    {
        private const int MONO_DIM = 1024;
        private const int RANDOM_METHOND = 1;
        
        private const bool MATRIX_FROM_R = false;
        private const bool M_TIMES_M = false;
        private const bool SAVE_MATRIX = false;
        private const bool PRINT_MATRIX = false;
        private const bool TEST_ALL = false;
        
        //--------------------------------------------------------------------------------------------------------------------------- BLASDirect.dll
        [DllImport("BLASDirect.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BLASDirect_MatrixMul(int monoDim, double[] m1, double[] m2, double[] mRes);

        [DllImport("BLASDirect.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BLASDirect_MatrixMulSimple(int monoDim, double[] m1, double[] m2, double[] mRes);

        //--------------------------------------------------------------------------------------------------------------------------- ArmadilloLib.dll
        [DllImport("ArmadilloLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ArmadilloLib_MatrixMul(int monoDim, double[] m1, double[] m2, double[] mRes);

        //[DllImport("ArmadilloLib.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern double ArmadilloLib_CalcDeterminant(int monoDim, double[] m);

        //--------------------------------------------------------------------------------------------------------------------------- BoostLib.dll
        [DllImport("BoostLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void BoostLib_MatrixMul(int monoDim, double[] m1, double[] m2, double[] mRes);

        //--------------------------------------------------------------------------------------------------------------------------- EigenLib.dll
        [DllImport("EigenLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EigenLib_MatrixMul(int monoDim, double[] m1, double[] m2, double[] mRes);

        //--------------------------------------------------------------------------------------------------------------------------- OpenBLASLib.dll
        [DllImport("OpenBLASLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenBLASLib_MatrixMul(int monoDim, double[] m1, double[] m2, double[] mRes);

        //--------------------------------------------------------------------------------------------------------------------------- OpenBLASLib.dll
        [DllImport("OpenCVLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void OpenCVLib_MatrixMul(int monoDim, double[] m1, double[] m2, double[] mRes);

        static void Main(string[] args)
        {
            string tests = args.Length == 0 ? "soc" : args[0];

            //-------------------------------------------------------- init
            double[] m1, m2;
            if (MATRIX_FROM_R)
            {
                if (!MatrixUtils.Instance.LoadMatrix("m1R.csv", MONO_DIM, out m1)) throw new Exception("WTF m1R");
                if (!MatrixUtils.Instance.LoadMatrix("m2R.csv", MONO_DIM, out m2)) throw new Exception("WTF m2R");
            }
            else
            {
                m1 = new double[MONO_DIM * MONO_DIM];
                MatrixUtils.Instance.SetRandom(MONO_DIM, m1, RANDOM_METHOND);

                m2 = new double[m1.Length];
                if (M_TIMES_M) Array.Copy(m1, m2, m1.Length);
                else MatrixUtils.Instance.SetRandom(MONO_DIM, m2, RANDOM_METHOND);

                if (SAVE_MATRIX)
                {
                    MatrixUtils.Instance.SaveMatrix("m1.csv", MONO_DIM, m1);
                    MatrixUtils.Instance.SaveMatrix("m2.csv", MONO_DIM, m2);
                }
            }

            if (PRINT_MATRIX)
            {
                Console.WriteLine("----- m1");
                MatrixUtils.Instance.PrintMatrix(MONO_DIM, m1);
                Console.WriteLine("----- m2");
                MatrixUtils.Instance.PrintMatrix(MONO_DIM, m2);
            }

            Console.WriteLine("----- Begin: " + MONO_DIM);
            Console.WriteLine();

            double[] mResSimple = null;
            if (tests.Contains('s') || TEST_ALL)
            {
                TestBase("Simple in C# (optimized)", MONO_DIM, m1, m2, out mResSimple, "mResSimple", MatrixUtils.Instance.MatMulOptimize);
            }

            if (tests.Contains('+') || TEST_ALL)
            {
                double[] mResSimpleCPP;
                TestBase("Simple in C++           ", MONO_DIM, m1, m2, out mResSimpleCPP, "mResSimpleCPP", BLASDirect_MatrixMulSimple, mResSimple);
            }

            if (tests.Contains('b') || TEST_ALL)
            {
                double[] mResBLASNetLib;
                TestBase("BLAS NetLib             ", MONO_DIM, m1, m2, out mResBLASNetLib, "mResBLASNetLib", BLASDirect_MatrixMul, mResSimple);
            }

            if (tests.Contains('a') || TEST_ALL)
            {
                double[] mResArmadillo;
                TestBase("Armadillo               ", MONO_DIM, m1, m2, out mResArmadillo, "mResArmadillo", ArmadilloLib_MatrixMul, mResSimple);
            }

            if (tests.Contains('u') || TEST_ALL)
            {
                double[] mResBoost;
                TestBase("Boost                   ", MONO_DIM, m1, m2, out mResBoost, "mResBoost", BoostLib_MatrixMul, mResSimple);
            }

            if (tests.Contains('e') || TEST_ALL)
            {
                double[] mResEigen;
                TestBase("Eigen                   ", MONO_DIM, m1, m2, out mResEigen, "mResEigen", EigenLib_MatrixMul, mResSimple);
            }

            if (tests.Contains('o') || TEST_ALL)
            {
                double[] mResOpenBLAS;
                TestBase("OpenBLAS                ", MONO_DIM, m1, m2, out mResOpenBLAS, "mResOpenBLAS", OpenBLASLib_MatrixMul, mResSimple);
            }

            if (tests.Contains('c') || TEST_ALL)
            {
                double[] mResOpenCV;
                TestBase("OpenCV                  ", MONO_DIM, m1, m2, out mResOpenCV, "mResOpenCV", OpenCVLib_MatrixMul, mResSimple);
            }

            Console.WriteLine();
            Console.WriteLine("----- End");
            Console.ReadLine();
        }

        static void TestBase(string testName, int monoDim, double[] m1, double[] m2, out double[] mRes, string matrixFileName, Action<int, double[], double[], double[]> act, double[] mReference = null)
        {
            DateTime ta = DateTime.Now;

            //init matrice risultante (inclusa nell'analisi dei tempi)
            mRes = new double[monoDim * monoDim];
            MatrixUtils.Instance.SetZero(mRes);

            //operazione
            act(monoDim, m1, m2, mRes);

            DateTime tb = DateTime.Now;
            double diffTime = (tb - ta).TotalMilliseconds;

            string msg = testName + ": " + diffTime.ToString("0.0");

            if (mReference != null)
            {
                //verifica della matrice risultante con la matrice di riferimento
                //if (!MatrixUtils.Instance.AreEqual(mRes, mReference)) 
                double diff = MatrixUtils.Instance.GetMaxDiff(mRes, mReference);
                if (diff > MatrixUtils.MAX_DIFF) msg += " ===> max_diff: " + diff.ToString("0.0");
            }

            if (SAVE_MATRIX) MatrixUtils.Instance.SaveMatrix(matrixFileName + ".csv", monoDim, mRes);

            Console.WriteLine(msg);
            if (PRINT_MATRIX) MatrixUtils.Instance.PrintMatrix(monoDim, mRes); 
        }
    }
}
