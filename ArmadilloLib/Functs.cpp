#include <stdlib.h>
#include <stdio.h>

#include <armadillo>

using namespace std;
using namespace arma;

#define TEST1_EXPORTS

#ifdef TEST1_EXPORTS
#define TEST1_API __declspec(dllexport)
#else
#define TEST1_API __declspec(dllimport)
#endif

extern "C"
{

	//
	//
	// note: check %% in config.hpp
	//
	//


	TEST1_API void ArmadilloLib_MatrixMul(int monoDim, double m1[], double m2[], double mRes[])
	{
		mat A(m1, monoDim, monoDim);
		mat B(m2, monoDim, monoDim);
		
		mat Res = A * B;

		int indexRoot = 0;
		for(int row = 0; row < monoDim; row++, indexRoot+=monoDim)
			for(int col = 0; col < monoDim; col++)
				mRes[indexRoot + col] = Res(col, row); // this is correct
	}

	//LAPACK is needed
	//TEST1_API double ArmadilloLib_CalcDeterminant(int monoDim, double m[])
	//{
	//	mat mArm(m, monoDim, monoDim);
	//	double res = det(mArm);
	//	return res;
	//}

	TEST1_API int ArmadilloLib_TestBase()
	{
		mat A(2,3);
		return A.n_rows;
	}
}
  