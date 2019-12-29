#include <stdlib.h>
#include <stdio.h>
#include "cblas.h"

#define TEST1_EXPORTS

#ifdef TEST1_EXPORTS
#define TEST1_API __declspec(dllexport)
#else
#define TEST1_API __declspec(dllimport)
#endif

extern "C"
{
	TEST1_API void OpenBLASLib_MatrixMul(int monoDim, double m1[], double m2[], double mRes[])
	{
		char no = 'N';
		char tr = 'T';

		int m, n, k;
		m = n = k = monoDim;

		double alpha = 1.0; 
		double beta = 0;

		cblas_dgemm(CblasColMajor, CblasNoTrans, CblasTrans, m, n, k, alpha, m1, monoDim, m2, monoDim, beta, mRes, monoDim);
	}

	TEST1_API void OpenBLASLib_MatrixMulSimple(int monoDim, double m1[], double m2[], double mRes[])
	{
		for(int j = 0; j < monoDim; j++)
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
}