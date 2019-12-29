#include <stdlib.h>
#include <stdio.h>

//version boost_1_59_0
#include <boost/numeric/ublas/matrix.hpp>
#include <boost/numeric/ublas/io.hpp>
#include <boost/numeric/ublas/operations.hpp>
#include <boost/numeric/ublas/operation_blocked.hpp>
#include <boost/numeric/ublas/lu.hpp>

using namespace boost::numeric::ublas;

#define TEST1_EXPORTS

#ifdef TEST1_EXPORTS
#define TEST1_API __declspec(dllexport)
#else
#define TEST1_API __declspec(dllimport)
#endif

extern "C"
{
	TEST1_API void BoostLib_MatrixMul(int monoDim, double m1[], double m2[], double mRes[])
	{
		//il ne marche pas
		//matrix<double> bM1(monoDim, monoDim, (double&)m1);
		//matrix<double> bM2(monoDim, monoDim, (double&)m2);
		matrix<double> bM1(monoDim, monoDim);
		matrix<double> bM2(monoDim, monoDim);

		//ci sarà un metodo migliore...
		int indexRoot = 0;
		for(int row = 0; row < monoDim; row++, indexRoot+=monoDim)
			for(int col = 0; col < monoDim; col++)
			{
				bM1(row, col) = m1[indexRoot + col];
				bM2(row, col) = m2[indexRoot + col];
			}

		//matrix<double> bRes = prod(bM1, bM2); //troppo lenta

		matrix<double> bRes(monoDim, monoDim);
		if (monoDim == 1024)
		{
			//molto più veloce, ma richiede costante
			bRes = block_prod<matrix<double>, 1024>(bM1, bM2);
		}
		else
		{
			axpy_prod(bM1, bM2, bRes, true);
		}
		
		indexRoot = 0;
		for(int row = 0; row < monoDim; row++, indexRoot+=monoDim)
			for(int col = 0; col < monoDim; col++)
				mRes[indexRoot + col] = bRes(row, col);
	}

	TEST1_API int BoostLib_TestBase()
	{
		matrix<double> m (3, 3);
		for (unsigned i = 0; i < m.size1 (); ++ i)
			for (unsigned j = 0; j < m.size2 (); ++ j)
				m (i, j) = 3 * i + j;
		return (int)m.size2();
	}
}
  