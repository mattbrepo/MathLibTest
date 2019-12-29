#include <stdlib.h>
#include <stdio.h>
#include "opencv2/core/core.hpp"

#define TEST1_EXPORTS

#ifdef TEST1_EXPORTS
#define TEST1_API __declspec(dllexport)
#else
#define TEST1_API __declspec(dllimport)
#endif

extern "C"
{
	TEST1_API void OpenCVLib_MatrixMul(int monoDim, double m1[], double m2[], double mRes[])
	{
		char no = 'N';
		char tr = 'T';

		int m, n, k;
		m = n = k = monoDim;

		double alpha = 1.0; 
		double beta = 0;

		cv::Mat cvM1(monoDim, monoDim, CV_64F, m1);
		cv::Mat cvM2(monoDim, monoDim, CV_64F, m2);
		
		cv::Mat cvRes = cvM1 * cvM2;
		int indexRoot = 0;
		for(int row = 0; row < monoDim; row++, indexRoot+=monoDim)
			for(int col = 0; col < monoDim; col++)
				mRes[indexRoot + col] = cvRes.data[col, row];
	}

	TEST1_API void OpenCVLib_MatrixMulSimple(int monoDim, double m1[], double m2[], double mRes[])
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