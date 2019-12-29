#include <stdlib.h>
#include <stdio.h>
#include "cblas_f77.h"

//
// presa qui (http://www.netlib.org/blas/) e ricompilata col gFortran
// 
// NB: per il funzionamento della blas è inoltre necessario libgcc_s_dw2-1.dll, libquadmath-0.dll
// 

#define TEST1_EXPORTS

#ifdef TEST1_EXPORTS
#define TEST1_API __declspec(dllexport)
#else
#define TEST1_API __declspec(dllimport)
#endif

extern "C"
{
	TEST1_API void BLASDirect_MatrixMul(int monoDim, double m1[], double m2[], double mRes[])
	{
		char no = 'N';
		char tr = 'T';

		int m, n, k;
		m = n = k = monoDim;

		double alpha = 1.0; 
		double beta = 0;

		F77_dgemm(&no, &no, &m, &n, &k, &alpha, m1, &k, m2, &n, &beta, mRes, &n);
	}

	TEST1_API void BLASDirect_MatrixMulSimple(int monoDim, double m1[], double m2[], double mRes[])
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