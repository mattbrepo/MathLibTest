#include <stdlib.h>
#include <stdio.h>

//version eigen-eigen-b30b87236a1b
#include <Eigen/Dense>
#include <Eigen/QR>

using namespace Eigen;

#define TEST1_EXPORTS

#ifdef TEST1_EXPORTS
#define TEST1_API __declspec(dllexport)
#else
#define TEST1_API __declspec(dllimport)
#endif

extern "C"
{
	TEST1_API void EigenLib_MatrixMul(int monoDim, double m1[], double m2[], double mRes[])
	{
		MatrixXd eM1 = Map<MatrixXd>(m1, monoDim, monoDim);
		MatrixXd eM2 = Map<MatrixXd>(m2, monoDim, monoDim);
		
		MatrixXd eRes = eM1 * eM2;
		
		int indexRoot = 0;
		for(int row = 0; row < monoDim; row++, indexRoot+=monoDim)
			for(int col = 0; col < monoDim; col++)
				mRes[indexRoot + col] = eRes(col, row); //giusto così evidentemente
	}

}
  